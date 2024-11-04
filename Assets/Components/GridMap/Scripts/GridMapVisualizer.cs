using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.GridMap;
using RosMessageTypes.Geometry;

public class GridMapData
{
    public string[] layers;
    public string[] basic_layers;
    public float[][][] data;
    public Mesh mesh;

    public Gradient gradient;

    public GridMapData(string[] layers, string[] basic_layers, Float32MultiArrayMsg[] data, Gradient gradient)
    {
        this.layers = layers;
        this.basic_layers = basic_layers;
        this.data = new float[data.Length][][];
        this.gradient = gradient;

        for (int i = 0; i < data.Length; i++)
        {
            MultiArrayLayoutMsg layout = data[i].layout;
            // Assuming column then row as 0, and 1 dimensions
            this.data[i] = new float[layout.dim[1].size][];
            for (int j = 0; j < layout.dim[1].size; j++)
            {
                this.data[i][j] = new float[layout.dim[0].size];
                for (int k = 0; k < layout.dim[0].size; k++)
                {
                    this.data[i][j][k] = data[i].data[j * layout.dim[0].size + k];
                }
            }
        }

        mesh = new Mesh();
        mesh.name = "GridMapMesh";

        int width = (int)data[0].layout.dim[0].size;
        int height = (int)data[0].layout.dim[1].size;

        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uv = new Vector2[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
            float elevation = this.data[0][y][x];
            if (float.IsNaN(elevation))
                elevation = 0;

            vertices[y * width + x] = new Vector3(x, elevation, y);
            uv[y * width + x] = new Vector2((float)x / (width - 1), (float)y / (height - 1));
            }
        }

        int triangleIndex = 0;
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
            int bottomLeft = y * width + x;
            int bottomRight = bottomLeft + 1;
            int topLeft = bottomLeft + width;
            int topRight = topLeft + 1;

            triangles[triangleIndex++] = bottomLeft;
            triangles[triangleIndex++] = topLeft;
            triangles[triangleIndex++] = bottomRight;

            triangles[triangleIndex++] = bottomRight;
            triangles[triangleIndex++] = topLeft;
            triangles[triangleIndex++] = topRight;
            }
        }
   
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void Update(Float32MultiArrayMsg[] data)
    {
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < data.Length; i++)
        {
            MultiArrayLayoutMsg layout = data[i].layout;
            // Assuming column then row as 0, and 1 dimensions
            for (int j = 0; j < layout.dim[1].size; j++)
            {
                for (int k = 0; k < layout.dim[0].size; k++)
                {

                    float elevation = data[i].data[j * layout.dim[0].size + k];
                    this.data[i][j][k] = elevation;

                    bool valid = true;

                    if(float.IsNaN(elevation))
                    {
                        valid = false;
                        elevation = 0;
                    }

                    // Update the y value of the vertex
                    vertices[j * layout.dim[0].size + k] = new Vector3(k, elevation, j);

                    // Update the color of the vertex
                    if (!valid)
                    {
                        colors[j * layout.dim[0].size + k] = Color.clear;
                    } else {
                        colors[j * layout.dim[0].size + k] = gradient.Evaluate(elevation/10);                                            
                    }
                }
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    public override string ToString()
    {
        string result = "GridMapData: layers=\n";
        for(int i = 0; i < layers.Length; i++)
        {
            result += "    " + layers[i] + " " + data[0].Length + "x" + data[0][0].Length + "\n";
        }   


        return result;
    }

}

public class GridMapVisualizer : MonoBehaviour
{

    public string topic = "/elevation_mapping/elevation_map_recordable";
    public float scale = 1.0f;
    public float gradientScale = 1.0f;
    public Gradient gradient;
    ROSConnection _ros;

    private bool _updated = false;
    private GridMapData _data;
    // Start is called before the first frame update
    void Start()
    {
        _ros = ROSConnection.GetOrCreateInstance();

        _ros.Subscribe<GridMapMsg>(topic, OnGridMapMessage);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_updated)
        {
            for(int i =0; i<_data.data[0].Length; i++)
            {
                for(int j = 0; j<_data.data[0][0].Length; j++)
                {
                    if (float.IsNaN(_data.data[0][i][j]))
                    {
                        continue;
                    }
                    // Debug.DrawRay(new Vector3(i, 0, j) * scale, new Vector3(0, _data.data[0][i][j], 0), gradient.Evaluate(_data.data[0][i][j])/gradientScale);
                }
            }
        }
        
    }

    void OnGridMapMessage(GridMapMsg message)
    {
        if (_data == null)
        {
            _data = new GridMapData(message.layers, message.basic_layers, message.data, gradient);
            _updated = true;
            // spawn the mesh
            GetComponent<MeshFilter>().mesh = _data.mesh;

        }
        else
        {
            _data.Update(message.data);
        }

        transform.localScale = new Vector3((float)message.info.length_x/10, 1, (float)message.info.length_y/10);

        PoseMsg pose = message.info.pose;

        transform.localPosition = new Vector3(-(float)pose.position.y, (float)pose.position.z, (float)pose.position.x);

        if(transform.parent == null)
        {
            GameObject root = GameObject.Find(message.info.header.frame_id);
            if (root == null)
            {
                Debug.LogWarning("Parent not found");
            } else {
                transform.parent = root.transform;
            }
        }
        
    }
}

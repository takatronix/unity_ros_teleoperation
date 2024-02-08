using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;

public class LidarDrawer : MonoBehaviour
{
    public Material lidar_material;
    public Material rgbd_material;


    GraphicsBuffer _meshTriangles;
    GraphicsBuffer _meshVertices;
    GraphicsBuffer _ptData;

    public float offset = 4.5f;
    public float scale = 1.0f;
    public int maxPts = 1_000_000;
    public int displayPts = 10;
    public int sides = 3;
    private RenderParams renderParams;
    public string topic = "/lidar/point_cloud";
    public bool rgbd = false;

    private int _LidarDataSize = 4*4;
    private ROSConnection _ros;
    private int _displayPts;
    private Mesh mesh;
    private LidarSpawner lidarSpawner;

    void Start()
    {
        _LidarDataSize = rgbd ? 4*6 : 4*4;
        mesh = _MakePolygon(sides);

        _meshTriangles = new GraphicsBuffer(GraphicsBuffer.Target.Structured, mesh.triangles.Length, 4);
        _meshTriangles.SetData(mesh.triangles);
        _meshVertices = new GraphicsBuffer(GraphicsBuffer.Target.Structured, mesh.vertices.Length, 12);
        _meshVertices.SetData(mesh.vertices);
        _ptData = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxPts, _LidarDataSize);
        _displayPts = displayPts;


        renderParams = new RenderParams(rgbd ? rgbd_material : lidar_material);
        renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 100);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(-offset, 0, 0)));
        renderParams.matProps.SetFloat("_PointSize", scale);
        renderParams.matProps.SetBuffer("_LidarData", _ptData);
        renderParams.matProps.SetInt("_BaseVertexIndex", (int)mesh.GetBaseVertex(0));
        renderParams.matProps.SetBuffer("_Positions", _meshVertices);

        _ros = ROSConnection.GetOrCreateInstance();
        _ros.Subscribe<PointCloud2Msg>(topic, OnPointcloud);

        lidarSpawner = GetComponent<LidarSpawner>();
        // lidarSpawner.PointCloudGenerated += OnPointcloud;
    }

    private void OnValidate() {
        if(renderParams.matProps != null)
        {
            renderParams.matProps.SetFloat("_PointSize", scale);
        }
    }

    private void OnDestroy() {
        _meshTriangles?.Dispose();
        _meshTriangles = null;
        _meshVertices?.Dispose();
        _meshVertices = null;
        _ptData?.Dispose();
        _ptData = null;
    }

    private void Update() {
        Graphics.RenderPrimitivesIndexed(renderParams, MeshTopology.Triangles, _meshTriangles, _meshTriangles.count, (int)mesh.GetIndexStart(0),_displayPts);
    }

    public void OnPointcloud(PointCloud2Msg pointCloud)
    {
        if(pointCloud.data.Length == 0) return;
        if(pointCloud.data.Length / pointCloud.point_step > maxPts)
        {
            Debug.LogWarning("Point cloud too large, truncating");
        }
        else
        {
            _ptData.SetData(_ExtractXYZI(pointCloud, rgbd));
        }
    }

    private byte[] _ExtractXYZI(PointCloud2Msg data, bool rgbd = false)
    {
        // Assumes x, y, z, intensity are the first 4 fields
        int numPts = (int)(data.data.Length / data.point_step);
        numPts = Mathf.Min(numPts, maxPts);
        byte[] outData = new byte[numPts * 4 * (4 + (rgbd ? 2 : 0))];
        _displayPts = Mathf.Min(numPts, displayPts);

        for(int i = 0; i < numPts; i++)
        {
            int inIdx = (int)(i * data.point_step);
            int outIdx = i * 4 * (4 + (rgbd ? 2 : 0));
            for(int j = 0; j < 4; j++)
            {
                if(j == 3 && rgbd)
                {
                    // convert the reinterpret_cast<float&> to int, then extract the rgb bytes
                    int intensity = System.BitConverter.ToInt32(data.data, inIdx + (int)data.fields[3].offset);
                    ushort r = (ushort)(intensity >> 16 & 0xff);
                    ushort g = (ushort)(intensity >> 8 & 0xff);
                    ushort b = (ushort)(intensity & 0xff);

                    // convert to floats
                    float rf = r / 255.0f;
                    float gf = g / 255.0f;
                    float bf = b / 255.0f;


                    // write to outData
                    System.BitConverter.GetBytes(rf).CopyTo(outData, outIdx + j * 4);
                    System.BitConverter.GetBytes(gf).CopyTo(outData, outIdx + j * 4 + 4);
                    System.BitConverter.GetBytes(bf).CopyTo(outData, outIdx + j * 4 + 8);


                }
                else
                {
                    // copy over the 4 bytes of the float
                    for(int k = 0; k < 4; k++)
                    {
                        outData[outIdx + j * 4 + k] = data.data[inIdx + j * 4 + k];
                    }
                }
            }
        }
        return outData;
    }

    private static Mesh _MakePolygon(int sides)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[sides];
        int[] triangles = new int[(sides - 2) * 3];
        for(int i = 0; i < sides; i++)
        {
            float angle = 2 * Mathf.PI * i / sides;
            vertices[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        }
        for(int i = 0; i < sides - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.UploadMeshData(false);
        return mesh;
    }
}

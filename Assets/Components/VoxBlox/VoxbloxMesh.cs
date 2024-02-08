using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Voxblox;

public class VoxbloxMesh : MonoBehaviour
{
    public string topic = "/voxblox_node/mesh";
    private ROSConnection _ros;
    private Mesh mesh;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        _ros = ROSConnection.GetOrCreateInstance();
        _ros.Subscribe<MeshMsg>(topic, UpdateMesh);
    }

    void UpdateMesh(MeshMsg tmpMesh)
    {
        Debug.Log("Updating mesh: " + tmpMesh.mesh_blocks.Length + " blocks.");

        MeshBlockMsg[] blocks = tmpMesh.mesh_blocks;
        float block_edge_length = tmpMesh.block_edge_length;

        float point_conv_factor = 2f/System.UInt16.MaxValue;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();
        // add current mesh to the list
        vertices.AddRange(mesh.vertices);
        triangles.AddRange(mesh.triangles);
        colors.AddRange(mesh.colors);

        foreach(MeshBlockMsg block in blocks)
        {
            for(int i=0; i<block.x.Length; i++)
            {
                float x = ((float)block.x[i] * point_conv_factor + (float)block.index[0]) * block_edge_length;
                float y = ((float)block.y[i] * point_conv_factor + (float)block.index[1]) * block_edge_length;
                float z = ((float)block.z[i] * point_conv_factor + (float)block.index[2]) * block_edge_length;
                vertices.Add(new Vector3(x, y, z));
                
                // Color color = new Color(block.r[i], block.g[i], block.b[i]);
                // Debug.Log(color);
                // colors.Add(color);
                triangles.Add(i);
            }
            Debug.Log("color at 0: " + block.r[0] + " " + block.g[0] + " " + block.b[0]);
        }


        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

}

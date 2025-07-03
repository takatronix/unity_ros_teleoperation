using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;


public class LidarUtils 
{
    public static Mesh MakePolygon(int sides)
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

    public static byte[] ExtractData(PointCloud2Msg data, int maxPts, VizType vizType, out int numPts)
    {

        /**
        For different data type the order is 
        Lidar: x, y, z, intensity
        RGBD: x, y, z, rgb0
        RGBD Mesh: ??
        Splat: x, y, z, rgba, scalex, scaley, scalez, rot0, rot1, rot2, rot3, nx, ny, nz, fc_dc_0, fc_dc_1, fc_dc_2, opacity
        */

        // Just in case...
        if(maxPts < 1) maxPts = 1;
        int decmiator = 1;

        int data_size = vizType.GetSize();
        
        numPts = (int)(data.data.Length / data.point_step);

        if(numPts > maxPts)
        {
            decmiator = Mathf.CeilToInt((float)numPts / maxPts);
            numPts = numPts / decmiator;
        }

        byte[] outData = new byte[numPts * data_size];

        // For each point...
        for(int i = 0; i < numPts; i++)
        {
            // Grab the point at the decimated index
            int inIdx = (int)(i * data.point_step * (decmiator));
            int outIdx = i * data_size;

            // For each field in the point...
            for(int j = 0; j < vizType.GetFieldCount(); j++)
            {
                if (j >= data.fields.Length)
                {
                    // Debug.LogWarning($"LidarUtils: Field index {j} out of bounds for fields count {data.fields.Length}. Filling with 0.");

                    // If we are missing the last field (ie xyz only) then fill with 0
                    for (int k = 0; k < 4; k++)
                    {
                        outData[outIdx + j * 4 + k] = 0;
                    }
                    continue;

                }
                // Copy the 4 bytes in the float
                for (int k = 0; k < 4; k++)
                {
                    outData[outIdx + j * 4 + k] = data.data[inIdx + (int)data.fields[j].offset + k];
                }
            }
        }
        return outData;
    }
}

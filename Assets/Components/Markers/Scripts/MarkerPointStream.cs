using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;


public class MarkerPointStream : MonoBehaviour, IMarkerViz
{

    public Material point_material;

    GraphicsBuffer _meshTriangles;
    GraphicsBuffer _meshVertices;
    GraphicsBuffer _ptData;

    public float scale = .01f;
    public int maxPts = 100_000;
    public int sides = 3;
    private RenderParams renderParams;
    private Mesh mesh;
    public bool _enabled = true;
    public int _numPts = 0;



    // Start is called before the first frame update
    void Awake()
    {
        mesh = LidarUtils.MakePolygon(sides);


        _meshTriangles = new GraphicsBuffer(GraphicsBuffer.Target.Structured, mesh.triangles.Length, 4);
        _meshTriangles.SetData(mesh.triangles);
        _meshVertices = new GraphicsBuffer(GraphicsBuffer.Target.Structured, mesh.vertices.Length, 12);
        _meshVertices.SetData(mesh.vertices);
        _ptData = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxPts, 4);


        renderParams = new RenderParams(point_material);

        renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 100);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(0, 0, 0)));
        renderParams.matProps.SetFloat("_PointSize", scale);
        renderParams.matProps.SetBuffer("_PointData", _ptData);
        renderParams.matProps.SetInt("_BaseVertexIndex", (int)mesh.GetBaseVertex(0));
        renderParams.matProps.SetBuffer("_Positions", _meshVertices);

        LocalKeyword rgbdKeyword = new LocalKeyword(renderParams.material.shader, "COLOR_RGB");
        LocalKeyword intensityKeyword = new LocalKeyword(renderParams.material.shader, "COLOR_INTENSITY");
        LocalKeyword zKeyword = new LocalKeyword(renderParams.material.shader, "COLOR_Z");

        renderParams.material.SetKeyword(rgbdKeyword, true);
        renderParams.material.SetKeyword(intensityKeyword, false);
        renderParams.material.SetKeyword(zKeyword, false);


    }


    private void Update()
    {
        if (_enabled)
        {
            renderParams.matProps.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
            Graphics.RenderPrimitivesIndexed(renderParams, MeshTopology.Triangles, _meshTriangles, _meshTriangles.count, (int)mesh.GetIndexStart(0), _numPts);
        }
    }



    private void OnDestroy()
    {
        _meshTriangles?.Dispose();
        _meshTriangles = null;
        _meshVertices?.Dispose();
        _meshVertices = null;
        _ptData?.Dispose();
        _ptData = null;
    }


    public void SetData(PoseMsg pose, Vector3Msg scale, ColorRGBAMsg[] colors, PointMsg[] points)
    {
        if (points == null || points.Length == 0)
        {
            _ptData.SetData(new Vector4[0]);
            return;
        }

        Vector4[] pointData = new Vector4[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pt = points[i].From<FLU>();
            float color = colors.Length == 1 ? PackRGBA(colors[0]) : PackRGBA(colors[i]);
            pointData[i] = new Vector4(pt.x, pt.y, pt.z, color);
        }

        _ptData.SetData(pointData);
        _numPts = points.Length;
    }

    public static float PackRGBA(ColorRGBAMsg color)
    {
        // Pack a ColorRGBAMsg (r,g,b,a in 0-1 range) into a float
        int r = Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255);
        int g = Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255);
        int b = Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255);
        int a = 255; // Fixed alpha
        int packed = (r << 16) | (g << 8) | (b);
        return System.BitConverter.Int32BitsToSingle(packed);
    }


    private void OnValidate()
    {
        if (renderParams.matProps != null)
        {
            renderParams.matProps.SetFloat("_PointSize", scale);
        }
    }
    
    
    public void OnSizeChange(float size)
    {
        scale = size / 10f;
        renderParams.matProps.SetFloat("_PointSize", scale);
    }



}

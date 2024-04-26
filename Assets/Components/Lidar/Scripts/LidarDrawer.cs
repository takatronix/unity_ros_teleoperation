using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LidarDrawer))]
public class LidarDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        

        LidarDrawer myScript = (LidarDrawer)target;
        if(GUILayout.Button("Toggle Enabled"))
        {
            myScript.ToggleEnabled();
        }
    }
}
#endif

public class LidarDrawer : MonoBehaviour
{

    public enum VizType
    {
        Lidar,
        RGBD,
        RGBDMesh
    }
    public Material lidar_material;
    public Material rgbd_material;
    public Material rgbd_mesh_material;



    GraphicsBuffer _meshTriangles;
    GraphicsBuffer _meshVertices;
    GraphicsBuffer _ptData;

    public float scale = 1.0f;
    public int maxPts = 1_000_000;
    public int displayPts = 10;
    public int sides = 3;
    private RenderParams renderParams;
    public string topic = "/lidar/point_cloud";
    public VizType vizType = VizType.Lidar;

    private int _LidarDataSize = 4*4;
    private ROSConnection _ros;
    private Mesh mesh;
    private LidarSpawner _lidarSpawner;
    private bool _enabled = false;
    private GameObject _parent;
    private bool _missingParent = false;
    private bool rgbd = false;
    private int _numPts = 0;

    public GameObject p;



    void Awake()
    {
        _ros = ROSConnection.GetOrCreateInstance();

        rgbd = vizType == VizType.RGBD || vizType == VizType.RGBDMesh;
        _LidarDataSize = 4 * (4 + (rgbd ? 2 : 0));
        mesh = LidarUtils.MakePolygon(sides);

        _meshTriangles = new GraphicsBuffer(GraphicsBuffer.Target.Structured, mesh.triangles.Length, 4);
        _meshTriangles.SetData(mesh.triangles);
        _meshVertices = new GraphicsBuffer(GraphicsBuffer.Target.Structured, mesh.vertices.Length, 12);
        _meshVertices.SetData(mesh.vertices);
        _ptData = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxPts, _LidarDataSize);

        switch(vizType)
        {
            case VizType.Lidar:
                renderParams = new RenderParams(lidar_material);
                break;
            case VizType.RGBD:
                renderParams = new RenderParams(rgbd_material);
                break;
            case VizType.RGBDMesh:
                renderParams = new RenderParams(rgbd_mesh_material);
                break;
        }

        renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 100);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(0, 0, 0)));
        renderParams.matProps.SetFloat("_PointSize", scale);
        renderParams.matProps.SetBuffer("_LidarData", _ptData);
        renderParams.matProps.SetInt("_BaseVertexIndex", (int)mesh.GetBaseVertex(0));
        renderParams.matProps.SetBuffer("_Positions", _meshVertices);


        if((_lidarSpawner = GetComponent<LidarSpawner>()) != null)
        {
            _lidarSpawner.PointCloudGenerated += OnPointcloud;
        }
    }

    public bool CleanTF(string name)
    {
        GameObject target = GameObject.Find(name);

        if(target == null)
        {
            return false;
        }


        Debug.Log("Cleaning " + name + " " + target);

        List<GameObject> children = new List<GameObject>();

        // check if this is connected to root
        int count = 0;
        while(target.transform.parent != null)
        {
            count++;
            children.Add(target);
            target = target.transform.parent.gameObject;
            if(target.name == "odom")
            {
                children.Clear();
                Debug.Log("Connected to root");
                return true;
            }
            if(count > 1000)
            {
                Debug.LogWarning("Too many iterations");
                return false;
            }
        }

        foreach(GameObject child in children)
        {
            Destroy(child);
        }
        return false;
    }


    void UpdatePose(string frame)
    {
        if(!CleanTF(frame))
        {
            return;
        }
        p = GameObject.Find(frame);
        _parent = GameObject.Find(frame);
        if(_parent == null) return;

        transform.parent = _parent.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(-90, 90, 0);
        transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnValidate() {
        if(renderParams.matProps != null)
        {
            renderParams.matProps.SetFloat("_PointSize", scale);
        }
        if(displayPts > maxPts)
        {
            displayPts = maxPts;
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
        if(_enabled)
        {
            renderParams.matProps.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
            Graphics.RenderPrimitivesIndexed(renderParams, MeshTopology.Triangles, _meshTriangles, _meshTriangles.count, (int)mesh.GetIndexStart(0), _numPts);
        }
    }

    public void OnPointcloud(PointCloud2Msg pointCloud)
    {
        if(_parent == null || _parent.name != pointCloud.header.frame_id)
        {
            UpdatePose(pointCloud.header.frame_id);
        }
        if(pointCloud.data.Length == 0) return;

        _ptData.SetData(LidarUtils.ExtractXYZI(pointCloud, displayPts, rgbd, out _numPts));
    }

    public void OnTopicChange(string topic)
    {
        if(this.topic != null)
        {
            _ros.Unsubscribe(this.topic);
        }
        this.topic = topic;
        _ros.Subscribe<PointCloud2Msg>(topic, OnPointcloud);
        Debug.Log("Subscribed to " + topic);
    }

    public void OnSizeChange(float size)
    {
        scale = size/10f;
        renderParams.matProps.SetFloat("_PointSize", scale);
    }

    public void ToggleEnabled()
    {
        _enabled = !_enabled;
        if(!_enabled)
        {
            _ros.Unsubscribe(topic);
            _parent = null;
        } else
        {
            _ros.Subscribe<PointCloud2Msg>(topic, OnPointcloud);
        }
    }

    
}

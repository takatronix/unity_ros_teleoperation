using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.Visualization;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;


public class MarkerStream : SensorStream
{
    // This class is used to manage the visualization of markers in Unity.
    private static int nextId = 0;
    private int _id;
    public string topicName = "visualization_marker";
    public float pointSize = 0.1f; // Default point size for point markers

    [Header("Marker Prefabs")]
    public GameObject arrowPrefab;
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject cylinderPrefab;
    public GameObject lineStripPrefab;
    public GameObject lineListPrefab;
    public GameObject cubeListPrefab;
    public GameObject sphereListPrefab;
    public GameObject pointsPrefab;

    private Dictionary<string, GameObject> _namespaces = new Dictionary<string, GameObject>();

    private ROSConnection _ros;

    private delegate void UpdatePointSize(float size);
    private UpdatePointSize _updatePointSize;


    public enum MarkerType
    {
        Arrow,
        Cube,
        Sphere,
        Cylinder,
        Line_strip,
        Line_list,
        Cube_list,
        Sphere_list,
        Points,
        Text_view_facing,
        Mesh_resource,
        Triangle_list
    }

    public enum MarkerAction
    {
        Add_modify,     // 0
        Deprecated,    // 1
        Delete,        // 2
        Delete_all,   // 3
    }

    void Awake()
    {
        _id = nextId++;
        // Initialize ROS connection
        _ros = ROSConnection.GetOrCreateInstance();
    }
    // Start is called before the first frame update
    void Start()
    {
        // If we are the first instance subscribe to visualization_marker
        if (topicName != null && _id == 0)
        {
            // Subscribe to the marker topic
            _ros.Subscribe<MarkerMsg>(topicName, OnMarker);
        }

        _namespaces = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool Validate(MarkerMsg msg)
    {
        if (msg.id == (int)MarkerType.Text_view_facing || msg.id == (int)MarkerType.Mesh_resource || msg.id == (int)MarkerType.Triangle_list)
        {
            // Possibly log unsupported
            return false;
        }


        return true;
    }

    void OnMarker(MarkerMsg msg)
    {
        // Handle the received marker message
        // Get the marker type name from the enum
        string markerTypeName = System.Enum.GetName(typeof(MarkerType), msg.type);
        // Debug.Log($"Received marker with ID: {msg.id}, Type: {markerTypeName}");

        if (!Validate(msg))
        {
            return;
        }

        GameObject markerObject = null;
        if (msg.action == (int)MarkerAction.Delete || msg.action == (int)MarkerAction.Delete_all)
        {
            // If the action is delete, remove the marker
            if (_namespaces.TryGetValue(msg.ns, out markerObject))
            {
                Destroy(markerObject);
                _namespaces.Remove(msg.ns);
            }
            return; // Skip further processing for delete actions
        }

        // else we want to add or modify the marker
        if (!_namespaces.TryGetValue(msg.ns, out markerObject))
        {
            switch (msg.type)
            {
                case (int)MarkerType.Arrow:
                    markerObject = Instantiate(arrowPrefab);
                    break;
                case (int)MarkerType.Cube:
                    markerObject = Instantiate(cubePrefab);
                    break;
                case (int)MarkerType.Sphere:
                    markerObject = Instantiate(spherePrefab);
                    break;
                case (int)MarkerType.Cylinder:
                    markerObject = Instantiate(cylinderPrefab);
                    break;
                case (int)MarkerType.Line_strip:
                    markerObject = Instantiate(lineStripPrefab);
                    break;
                case (int)MarkerType.Line_list:
                    markerObject = Instantiate(lineListPrefab);
                    break;
                case (int)MarkerType.Cube_list:
                    markerObject = Instantiate(cubeListPrefab);
                    break;
                case (int)MarkerType.Sphere_list:
                    markerObject = Instantiate(sphereListPrefab);
                    break;
                case (int)MarkerType.Points:
                    markerObject = Instantiate(pointsPrefab);
                    _updatePointSize += markerObject.GetComponent<MarkerPointStream>().OnSizeChange;
                    break;
                default:
                    Debug.LogWarning($"Unsupported marker type: {markerTypeName}");
                    return; // Skip unsupported types
            }
            markerObject.name = msg.ns;
            // markerObject.transform.SetParent(msg.header.frame_id != "" ? GameObject.Find(msg.header.frame_id).transform : transform);

            if (msg.lifetime.sec > 0)
            {
                Debug.Log($"Marker {msg.ns} will be destroyed after {msg.lifetime} seconds");
            }

            _namespaces[msg.ns] = markerObject;

        }

        if (msg.colors.Length == 0)
        {
            // If no colors are provided, use a default color
            msg.colors = new ColorRGBAMsg[] { msg.color };
        }

        markerObject.GetComponent<IMarkerViz>().SetData(
            msg.pose,
            msg.scale,
            msg.colors,
            msg.points
        );

        if (markerObject.transform.parent == null || markerObject.transform.parent.name != msg.header.frame_id)
        {
            // If the marker object has a parent, check if it is under the root
            Transform parentTransform = GameObject.Find(msg.header.frame_id)?.transform;
            if (parentTransform != null)
            {
                markerObject.transform.SetParent(parentTransform);
            }
            else
            {
                // If the parent frame is not found, set it to the root
                markerObject.transform.SetParent(GameObject.FindWithTag("root")?.transform);
            }
            markerObject.transform.localPosition = msg.pose.position.From<FLU>();
            markerObject.transform.localRotation = msg.pose.orientation.From<FLU>();
            Vector3 scale = msg.scale.From<FLU>();
            scale.x *= -1;
            markerObject.transform.localScale = scale;
        }

    }

    // Implementation of abstract method ToggleTrack
    public override void ToggleTrack(int trackId)
    {
        // Add your logic here
        Debug.Log($"Toggling track with ID: {trackId}");
    }

    // Implementation of abstract method Serialize
    public override string Serialize()
    {
        // Add your serialization logic here
        Debug.Log("Serializing MarkerStream");
        return "{}"; // Example return value
    }

    // Implementation of abstract method Deserialize
    public override void Deserialize(string data)
    {
        // Add your deserialization logic here
        Debug.Log($"Deserializing MarkerStream with data: {data}");
    }

    public void OnSizeChange(float size)
    {
        // Update the point size for all markers that support it
        if (_updatePointSize != null)
        {
            _updatePointSize(size);
        }
    }

    public void OnValidate()
    {
        if (pointSize < 0)
        {
            pointSize = 0f; // Reset to default value
        }
        _updatePointSize(pointSize);
    }
}

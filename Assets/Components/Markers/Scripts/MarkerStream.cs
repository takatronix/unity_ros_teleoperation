using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.Visualization;
using UnityEngine;

public class MarkerStream : SensorStream
{
    public enum MarkerType
    {
        Cube,
        Sphere,
        Arrow,
        Line,
        Text
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMarker(MarkerMsg msg)
    {
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
}

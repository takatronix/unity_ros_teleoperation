using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ServiceStream))]
public class ServiceStreamEditor : SensorStreamEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ServiceStream myScript = (ServiceStream)target;
        if (GUILayout.Button("Subscribe to Service"))
        {
            myScript.SubscribeToService();
        }
        if (GUILayout.Button("Trigger Service"))
        {
            myScript.TriggerService();
        }
    }
}
#endif

[System.Serializable]
public class ServiceData: ISensorData
{
}

public class ServiceStream : SensorStream
{
    public string topic = "/service_name";
    public TextMeshProUGUI topicText;
    public TMPro.TMP_InputField topicInputField;
    private ROSConnection _ros;


    // Start is called before the first frame update
    void Start()
    {
        topicText.text = topic;
        topicInputField.text = topic;

        _ros = ROSConnection.GetOrCreateInstance();
    }

    public void OnTopicChanged(string newTopic)
    {
        topic = newTopic;
        topicText.text = topic;
        topicInputField.text = topic;
        Debug.Log($"Topic changed to: {topic}");
    }

    public void SubscribeToService()
    {
        topic = topicInputField.text;
        topicText.text = topic;
        Debug.Log($"Subscribing to service: {topic}");
        _ros.RegisterRosService<EmptyRequest, EmptyResponse>(topic);
    }

    void OnApplicationQuit()
    {
      Debug.Log("Unregistering service: " + topic);  
    }

    public void TriggerService()
    {
        Debug.Log($"Triggering service: {topic}");
        _ros.SendServiceMessage<EmptyResponse>(topic, new EmptyRequest(), ServiceCallback);
    }

    private void ServiceCallback(EmptyResponse response)
    {
        Debug.Log($"Service response received: {response}");
    }

    public override string Serialize()
    {
        Debug.Log("Serializing ServiceStream");
        ServiceData data = new ServiceData();
        data.position = transform.localPosition;
        data.rotation = transform.localRotation;
        data.scale = transform.localScale;
        data.topicName = topic;
        return JsonUtility.ToJson(data);
    }

    public override void Deserialize(string data)
    {
        try
        {
            Debug.Log("Deserializing ServiceStream");
            ServiceData sensorData = JsonUtility.FromJson<ServiceData>(data);
            transform.localPosition = sensorData.position;
            transform.localRotation = sensorData.rotation;
            transform.localScale = sensorData.scale;
            topic = sensorData.topicName;
            topicText.text = topic;
            topicInputField.text = topic;

            SubscribeToService();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to deserialize ServiceStream data: {e.Message}");
        }
    }

    public override void ToggleTrack(int trackId)
    {
        // Add your logic here
        Debug.Log($"Toggling track with ID: {trackId}");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine.XR.Interaction.Toolkit;
using RosMessageTypes.Behaviortree;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PosePublisher))]
public class PosePublisherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PosePublisher myScript = (PosePublisher)target;
        if (GUILayout.Button("Cancel"))
        {
            // myScript.Cancel();
        }
        if (GUILayout.Button("Publish"))
        {
            myScript.LastSelected(new SelectExitEventArgs());
        }
    }
}
#endif

public class PosePublisher : MonoBehaviour
{
    public string poseTopic;
    public string missionTopic;
    public TMPro.TMP_InputField missionTopicInput;
    // public string cancelTopic;
    // public TMPro.TMP_InputField cancelTopicInput;
// 
    public string frame_id = "odom";
    public GameObject arrow;
    public bool debug = false;

    private ROSConnection ros;
    private PoseStampedMsg poseMsg;
    private ActivateMissionRequest missionRequest;

    private Vector3 start;
    private GameObject arrowInstance;

    private XRRayInteractor interactor;

    private bool _enabled = true;
    private bool _sent = false;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        poseMsg = new PoseStampedMsg();
        poseMsg.header.frame_id = frame_id;
        poseMsg.pose = new PoseMsg();

        missionRequest = new ActivateMissionRequest();
        missionRequest.desired_mission = new ActiveMissionMsg();
        missionRequest.desired_mission.mission = ActiveMissionMsg.TARGET;
        missionRequest.target_pose_mission = new TargetMsg();
        missionRequest.target_pose_mission.target = poseMsg;
        missionRequest.target_pose_mission.target_mode = TargetMsg.BASE;
        missionRequest.target_pose_mission.base_mode = TargetMsg.BASE_PID_CONTROL;
        missionRequest.target_pose_mission.desired_motion_state = "walk";
        missionRequest.target_pose_mission.max_linear_velocity = 0.0;
        missionRequest.target_pose_mission.max_angular_velocity = 0.0;
        missionRequest.target_pose_mission.target_threshold_position = 0.0;
        missionRequest.target_pose_mission.target_threshold_orientation = 0.0;


        // try to get mission and cancel topic from player prefs
        if (PlayerPrefs.HasKey("missionTopic"))
        {
            missionTopic = PlayerPrefs.GetString("missionTopic");
        }
        // if (PlayerPrefs.HasKey("cancelTopic"))
        // {
        //     cancelTopic = PlayerPrefs.GetString("cancelTopic");
        // }

        missionTopicInput.text = missionTopic;
        // cancelTopicInput.text = cancelTopic;


        ros.RegisterPublisher<PoseStampedMsg>(poseTopic);
        // ros.RegisterRosService<ActivateMissionRequest, ActivateMissionResponse>(missionTopic);
        // ros.RegisterRosService<TriggerRequest, TriggerResponse>(cancelTopic);
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    void Update()
    {
        if(_enabled && arrowInstance != null && interactor != null)
        {
            //point arrow at interactor position
            Vector3 end;
            interactor.TryGetHitInfo(out end, out _, out _, out _);
            arrowInstance.transform.LookAt(end);
        }
    }

    public void Confirm()
    {
        if(!_enabled || _sent) return;

        // publish pose
        ros.Send(poseTopic, poseMsg);

        // publish mission request
        ros.SendServiceMessage<ActivateMissionResponse>(missionTopic, missionRequest, (response) => Debug.Log(response.success));

        Debug.Log("published pose");

        start = Vector3.zero;
        interactor = null;
        _sent = true;
    }


    public void FirstSelected(SelectEnterEventArgs args)
    {
        if(!_enabled) return;

        Vector3 tmp;
        ((XRRayInteractor)args.interactor).TryGetHitInfo(out tmp, out _, out _, out _);

        start = tmp;//transform.parent.InverseTransformPoint(tmp);
    
        interactor = (XRRayInteractor)args.interactor;

        if (arrowInstance == null)
        {
            arrowInstance = Instantiate(arrow, start, Quaternion.identity, transform.parent);

        }else
        {
            arrowInstance.transform.position = start;
        }
    }

    public void LastSelected(SelectExitEventArgs args)
    {
        Vector3 end;
        ((XRRayInteractor)args.interactor).TryGetHitInfo(out end, out _, out _, out _);
        if (debug)
        {
            Debug.DrawLine(start, end, Color.red, 10);
        }

        start = transform.parent.InverseTransformPoint(start);
        end = transform.parent.InverseTransformPoint(end);

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, end - start);
        poseMsg.pose.position = (PointMsg)start.To<FLU>();
        poseMsg.pose.orientation = rotation.To<FLU>();

        _sent = false;

    }

    // public void Cancel()
    // {
    //     ros.SendServiceMessage<TriggerResponse>(cancelTopic, new TriggerRequest(), (response) => Debug.Log(response.success));
    // }


    public void OnMissionTopic(string topic)
    {
        missionTopic = topic;
        ros.RegisterRosService<ActivateMissionRequest, ActivateMissionResponse>(missionTopic);
        Debug.Log("Mission topic set to: " + topic);

        //write to player prefs
        PlayerPrefs.SetString("missionTopic", topic);
    }

    // public void OnCancelTopic(string topic)
    // {
    //     cancelTopic = topic;
    //     ros.RegisterRosService<TriggerRequest, TriggerResponse>(cancelTopic);
    //     Debug.Log("Cancel topic set to: " + topic);

    //     //write to player prefs
    //     PlayerPrefs.SetString("cancelTopic", topic);
    // }
}

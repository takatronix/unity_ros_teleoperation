using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using UnityEngine.InputSystem;
using RosMessageTypes.Sensor;

public class JoystickManager : MonoBehaviour
{
    public string joyTopic = "/quest/joystick";
    public string leftControllerPoseTopic = "/quest/left_controller/pose";
    public string rightControllerPoseTopic = "/quest/right_controller/pose";

    public InputActionReference joystickXY;
    public InputActionReference joystickXYClick;
    public InputActionReference joystickZR;
    public InputActionReference joystickZRClick;
    public InputActionReference controllerX;
    public InputActionReference controllerY;
    public InputActionReference controllerA;
    public InputActionReference controllerB;
    public InputActionReference controllerTriggerL;
    public InputActionReference controllerTriggerR;
    public InputActionReference controllerGripL;
    public InputActionReference controllerGripR;

    // Controller pose input actions
    public InputActionReference leftControllerPosition;
    public InputActionReference leftControllerRotation;
    public InputActionReference rightControllerPosition;
    public InputActionReference rightControllerRotation;

    private ROSConnection _ros;
    private Transform _root;

    private JoyMsg _joyMsg;
    private bool _enabled = false;
    private int leftHandState = 0; // 0 = not tracked, 1 = tracked, 2 = hand tracked
    private int rightHandState = 0; // 0 = not tracked, 1 = tracked, 2 = hand tracked
    private bool _loggedOnce = false;

    public string worldFrame = "quest_origin";

    void Start()
    {
        Debug.Log("[JoystickManager] Start called");
        _ros = ROSConnection.GetOrCreateInstance();

        // Find root transform for coordinate conversion
        var rootObj = GameObject.FindWithTag("root");
        if(rootObj != null)
        {
            _root = rootObj.transform;
            worldFrame = rootObj.name;
            Debug.Log("[JoystickManager] Found root: " + _root.name);
        }
        else
        {
            Debug.LogWarning("[JoystickManager] Could not find GameObject with tag 'root', using world coordinates");
        }

        _joyMsg = new JoyMsg();
        _joyMsg.header.frame_id = worldFrame;

        _ros.RegisterPublisher<JoyMsg>(joyTopic);
        _ros.RegisterPublisher<PoseStampedMsg>(leftControllerPoseTopic);
        _ros.RegisterPublisher<PoseStampedMsg>(rightControllerPoseTopic);
        Debug.Log("[JoystickManager] Registered publishers for " + joyTopic + ", " + leftControllerPoseTopic + ", " + rightControllerPoseTopic);

        // Check if input actions are assigned
        if(joystickXY == null || joystickXY.action == null) Debug.LogError("[JoystickManager] joystickXY is NULL!");
        if(controllerTriggerR == null || controllerTriggerR.action == null) Debug.LogError("[JoystickManager] controllerTriggerR is NULL!");

        // Check controller pose inputs
        if(leftControllerPosition == null || leftControllerPosition.action == null) Debug.LogWarning("[JoystickManager] leftControllerPosition is NULL - controller pose will not be published");
        if(rightControllerPosition == null || rightControllerPosition.action == null) Debug.LogWarning("[JoystickManager] rightControllerPosition is NULL - controller pose will not be published");
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
        Debug.Log("[JoystickManager] SetEnabled: " + enabled);
    }

    void Update()
    {
        if(_enabled)
        {
            if(!_loggedOnce)
            {
                Vector2 testXY = joystickXY.action.ReadValue<Vector2>();
                float testTrigger = controllerTriggerR.action.ReadValue<float>();
                Debug.Log("[JoystickManager] Update running, publishing to " + joyTopic + " | joystickXY=" + testXY + " | triggerR=" + testTrigger);
                _loggedOnce = true;
            }

            // Publish joystick data
            Vector2 xy = joystickXY.action.ReadValue<Vector2>();
            Vector2 zr = joystickZR.action.ReadValue<Vector2>();

            _joyMsg.axes = new float[] {xy.x, xy.y, zr.x, zr.y, controllerTriggerL.action.ReadValue<float>(), controllerTriggerR.action.ReadValue<float>(), controllerGripL.action.ReadValue<float>(), controllerGripR.action.ReadValue<float>()};
            _joyMsg.buttons = new int[] {
                controllerX.action.ReadValue<float>() > 0.5f ? 1 : 0,
                controllerA.action.ReadValue<float>() > 0.5f ? 1 : 0,
                controllerB.action.ReadValue<float>() > 0.5f ? 1 : 0,
                controllerY.action.ReadValue<float>() > 0.5f ? 1 : 0,
                0,
                0,
                0,
                0,
                controllerTriggerL.action.ReadValue<float>() > 0.5f ? 1 : 0,
                controllerTriggerR.action.ReadValue<float>() > 0.5f ? 1 : 0,
                controllerGripL.action.ReadValue<float>() > 0.5f ? 1 : 0,
                controllerGripR.action.ReadValue<float>() > 0.5f ? 1 : 0,
                joystickXYClick.action.ReadValue<float>() > 0.5f ? 1 : 0,
                joystickZRClick.action.ReadValue<float>() > 0.5f ? 1 : 0,
                0,
                leftHandState,
                rightHandState
            };
            _ros.Publish(joyTopic, _joyMsg);

            // Publish controller poses
            PublishControllerPose(leftControllerPosition, leftControllerRotation, leftControllerPoseTopic);
            PublishControllerPose(rightControllerPosition, rightControllerRotation, rightControllerPoseTopic);
        }
    }

    void PublishControllerPose(InputActionReference positionAction, InputActionReference rotationAction, string topic)
    {
        if(positionAction == null || positionAction.action == null) return;
        if(rotationAction == null || rotationAction.action == null) return;

        Vector3 position = positionAction.action.ReadValue<Vector3>();
        Quaternion rotation = rotationAction.action.ReadValue<Quaternion>();

        // Transform to root frame if available
        if(_root != null)
        {
            position = _root.InverseTransformPoint(position);
            rotation = Quaternion.Inverse(_root.rotation) * rotation;
        }

        PoseStampedMsg poseMsg = new PoseStampedMsg();
        poseMsg.header = new HeaderMsg();
        poseMsg.header.frame_id = worldFrame;
        poseMsg.pose.position = position.To<FLU>();
        poseMsg.pose.orientation = rotation.To<FLU>();

        _ros.Publish(topic, poseMsg);
    }
}

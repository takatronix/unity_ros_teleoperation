using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public PoseManager poseManager;
    public NvbloxMesh nvbloxMesh;
    public Image axisIcon;
    public Sprite unlockedIcon;
    public Sprite lockedIcon;
    
    public Sprite streamOnIcon;
    public Sprite streamOffIcon;
    public Image streamIcon;

    public bool startStreaming = false;

    private bool _lockedPose = true;

    private PosePublisher _posePublisher;
    private JoystickManager _joystickManager;
    private Streamer _streamer;
    void Start()
    {
        if (poseManager == null)
        {
            poseManager = FindObjectOfType<PoseManager>();
        }
        poseManager?.SetLocked(_lockedPose);
        axisIcon.sprite = _lockedPose ? lockedIcon : unlockedIcon;

        _joystickManager = GetComponent<JoystickManager>();
        _joystickManager.SetEnabled(false);

        _posePublisher = GetComponent<PosePublisher>();
        _posePublisher.SetEnabled(false);

        _streamer = FindObjectOfType<Streamer>();
        if (_streamer == null)
        {
            Debug.LogWarning("No Streamer found in scene");
        } else {
            if (startStreaming)
            {
                _streamer.enabled = true;
                streamIcon.sprite = streamOnIcon;
            }
            else
            {
                _streamer.enabled = false;
                streamIcon.sprite = streamOffIcon;
            }
            Debug.Log($"Streaming to topic {_streamer.topic}");
        }
    }

    public void ToggleStream()
    {
        if(_streamer != null){
            _streamer.enabled = !_streamer.enabled;
            streamIcon.sprite = _streamer.enabled ? streamOnIcon : streamOffIcon;
        }
    }

    public void ChangeMode(int modes)
    {
        switch (modes)
        {
            case 0:
                _joystickManager.SetEnabled(false);
                _posePublisher.SetEnabled(false);
                break;
            case 1:
                _joystickManager.SetEnabled(false);
                _posePublisher.SetEnabled(true);
                break;
            case 2:
                _joystickManager.SetEnabled(true);
                _posePublisher.SetEnabled(false);
                break;
        }
    }

    public void ToggleNvblox()
    {
        nvbloxMesh?.ToggleEnabled();
    }

    public void Recenter()
    {
        Vector3 position = Camera.main.transform.position;
        position += Camera.main.transform.forward*2;
        position.y = 0.5f;

        poseManager.BaseToLocation(position);
    }

    public void TogglePoseLock()
    {
        _lockedPose = !_lockedPose;
        poseManager?.SetLocked(_lockedPose);
        axisIcon.sprite = _lockedPose ? lockedIcon : unlockedIcon;

    }

}

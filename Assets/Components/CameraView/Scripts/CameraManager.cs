using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : SensorManagerEditor
{
}

#endif

public class CameraManager : SensorManager
{
    public Sprite untracked;
    public Sprite tracked;
    private bool _allTracking = false;

}


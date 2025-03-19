using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : SensorManagerEditor
{
}

#endif

public class AudioManager : SensorManager
{
    public Sprite untracked;
    public Sprite tracked;
    private bool _allTracking = false;

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ServiceManager))]
public class ServiceManagerEditor : SensorManagerEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ServiceManager serviceManager = (ServiceManager)target;
    }
}
#endif

public class ServiceManager : SensorManager
{
}

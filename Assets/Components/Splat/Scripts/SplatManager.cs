using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(SplatManager))]
public class SplatManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplatManager splatManager = (SplatManager)target;
        if(GUILayout.Button("Create Data"))
        {
            splatManager.CreateData();
        }
    }
}

#endif

public class SplatManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateData()
    {

    }
}

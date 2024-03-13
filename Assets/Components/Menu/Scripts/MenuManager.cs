using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MenuManager))]
public class MenuManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MenuManager menuManager = (MenuManager)target;
        if (GUILayout.Button("Red"))
        {
            menuManager.Red();
        }
        if (GUILayout.Button("Green"))
        {
            menuManager.Green();
        }
    }
}
#endif

public class MenuManager : MonoBehaviour
{
    private Material _leftEnd;
    private Material _rightEnd;

    private void Awake() 
    {
        _leftEnd = GetComponent<Renderer>().materials[1];
        _rightEnd = GetComponent<Renderer>().materials[2];        
    }

    public void ConnectionColor(Color c)
    {
        _leftEnd.color = c;
        _rightEnd.color = c;
    }
}

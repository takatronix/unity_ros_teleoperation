using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MenuTemplate))]
public class MenuTemplateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MenuTemplate myScript = (MenuTemplate)target;
        if (GUILayout.Button("Setup Rows"))
        {
            myScript.SetupRows();
        }
        if (GUILayout.Button("Toggle Menu"))
        {
            myScript.ToggleMenu();
        }
    }
}
#endif

public class MenuTemplate : MonoBehaviour
{
    public GameObject menu;

    public SensorManager[] managers;

    void Start()
    {
        SetupRows();
    }

    public void SetupRows()
    {
        managers = FindObjectsOfType<SensorManager>();
        float offset = 0;
        foreach (SensorManager manager in managers)
        {
            // Make a child of this object's rect transform
            manager.transform.SetParent(menu.transform);
            manager.transform.localPosition = Vector3.zero;
            manager.transform.localRotation = Quaternion.identity;
            manager.transform.localScale = Vector3.one;

            // Move the manager above the last row
            manager.transform.localPosition += Vector3.up * offset;
            offset += manager.GetComponent<RectTransform>().sizeDelta.y + 0.1f;
        }
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }
}

using UnityEditor;
using UnityEngine;

public class ResetPlayerPrefs
{
    [MenuItem("Tools/Reset All PlayerPrefs")]
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs have been reset!");
    }

    [MenuItem("Tools/Reset Robot Settings Only")]
    public static void ResetRobotSettings()
    {
        PlayerPrefs.DeleteKey("startRobotIndex");
        PlayerPrefs.DeleteKey("rootFrame");
        PlayerPrefs.Save();
        Debug.Log("Robot settings have been reset!");
    }
}

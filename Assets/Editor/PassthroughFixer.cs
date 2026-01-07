using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Editor utility to fix Passthrough setup for Meta Quest 3.
/// Adds required AR components to Main Camera.
/// </summary>
public class PassthroughFixer : EditorWindow
{
    [MenuItem("Tools/Fix Passthrough Setup")]
    public static void FixPassthrough()
    {
        // Find main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            // Try to find any camera with MainCamera tag
            GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraObj != null)
            {
                mainCamera = cameraObj.GetComponent<Camera>();
            }
        }

        if (mainCamera == null)
        {
            EditorUtility.DisplayDialog("Passthrough Fixer",
                "Main Camera not found! Please ensure your scene has a camera tagged as 'MainCamera'.",
                "OK");
            return;
        }

        bool modified = false;

        // Add ARCameraManager if missing
        ARCameraManager cameraManager = mainCamera.GetComponent<ARCameraManager>();
        if (cameraManager == null)
        {
            cameraManager = mainCamera.gameObject.AddComponent<ARCameraManager>();
            Debug.Log("PassthroughFixer: Added ARCameraManager to " + mainCamera.name);
            modified = true;
        }
        cameraManager.enabled = true;

        // Add ARCameraBackground if missing
        ARCameraBackground cameraBackground = mainCamera.GetComponent<ARCameraBackground>();
        if (cameraBackground == null)
        {
            cameraBackground = mainCamera.gameObject.AddComponent<ARCameraBackground>();
            Debug.Log("PassthroughFixer: Added ARCameraBackground to " + mainCamera.name);
            modified = true;
        }

        // Set camera clear flags for passthrough
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0, 0, 0, 0);

        // Check for ARSession
        ARSession arSession = Object.FindObjectOfType<ARSession>();
        if (arSession == null)
        {
            GameObject arSessionObj = new GameObject("AR Session");
            arSession = arSessionObj.AddComponent<ARSession>();
            arSessionObj.AddComponent<ARInputManager>();
            Debug.Log("PassthroughFixer: Created AR Session");
            modified = true;
        }

        // Mark scene as dirty
        if (modified)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        EditorUtility.DisplayDialog("Passthrough Fixer",
            "Passthrough setup complete!\n\n" +
            "- ARCameraManager: Added to " + mainCamera.name + "\n" +
            "- ARCameraBackground: Added to " + mainCamera.name + "\n" +
            "- Camera clear flags: Solid Color (transparent)\n" +
            "- AR Session: " + (arSession != null ? "Ready" : "Missing") + "\n\n" +
            "Don't forget to save the scene!",
            "OK");
    }
}

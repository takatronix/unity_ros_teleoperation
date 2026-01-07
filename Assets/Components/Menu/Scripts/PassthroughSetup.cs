using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Automatically sets up AR components required for Meta Quest passthrough.
/// Attach this to the XR Origin or Main Camera.
/// </summary>
public class PassthroughSetup : MonoBehaviour
{
    [SerializeField] private bool enablePassthroughOnStart = true;

    private ARCameraManager arCameraManager;
    private ARCameraBackground arCameraBackground;

    void Awake()
    {
        SetupPassthrough();
    }

    void SetupPassthrough()
    {
        // Find the main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("PassthroughSetup: Main camera not found!");
            return;
        }

        // Add ARCameraManager if not present
        arCameraManager = mainCamera.GetComponent<ARCameraManager>();
        if (arCameraManager == null)
        {
            arCameraManager = mainCamera.gameObject.AddComponent<ARCameraManager>();
            Debug.Log("PassthroughSetup: Added ARCameraManager to main camera");
        }

        // Add ARCameraBackground if not present
        arCameraBackground = mainCamera.GetComponent<ARCameraBackground>();
        if (arCameraBackground == null)
        {
            arCameraBackground = mainCamera.gameObject.AddComponent<ARCameraBackground>();
            Debug.Log("PassthroughSetup: Added ARCameraBackground to main camera");
        }

        // Find or create ARSession
        ARSession arSession = FindObjectOfType<ARSession>();
        if (arSession == null)
        {
            GameObject arSessionObj = new GameObject("AR Session");
            arSession = arSessionObj.AddComponent<ARSession>();
            arSessionObj.AddComponent<ARInputManager>();
            Debug.Log("PassthroughSetup: Created AR Session");
        }

        // Set camera clear flags for passthrough
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0, 0, 0, 0);

        // Enable passthrough
        if (enablePassthroughOnStart)
        {
            arCameraManager.enabled = true;
            Debug.Log("PassthroughSetup: Passthrough enabled");
        }
    }

    public void SetPassthroughEnabled(bool enabled)
    {
        if (arCameraManager != null)
        {
            arCameraManager.enabled = enabled;
        }
    }
}

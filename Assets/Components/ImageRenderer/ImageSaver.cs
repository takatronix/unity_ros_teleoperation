using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ImageSaver))]
public class ImageSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ImageSaver myScript = (ImageSaver)target;
        if(GUILayout.Button("Parse File"))
        {
            myScript.ParseFile();
        }
        if(GUILayout.Button("Move to Frame 0"))
        {
            myScript.MoveToFrame(0);
        }
        if(GUILayout.Button("Move to Frame 1"))
        {
            myScript.MoveToFrame(1);
        }
        if(GUILayout.Button("Render"))
        {
            myScript.Render();
        }
    }
}
#endif

public class ImageSaver : MonoBehaviour
{
    // Load text file
    public TextAsset textFile;
    private int _width, _height;
    private RenderTexture _renderTexture;

    private Vector3[] _positions;
    private Quaternion[] _rotations;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        ParseFile();
    }

    public void MoveToFrame(int i)
    {
        if(i < 0 || i >= _positions.Length)
        {
            Debug.LogError("Invalid frame index: " + i);
            return;
        }

        transform.position = _positions[i];
        transform.rotation = _rotations[i];
    }

    public void Render()
    {
        if(_camera)
        {
        // Save the rendered image to a folder
        string folderPath = "/home/maximum";
        string fileName = "rendered_image.png";
        string filePath = System.IO.Path.Combine(folderPath, fileName);
        RenderTexture.active = _renderTexture;
        Texture2D image = new Texture2D(_width, _height);
        image.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
        image.Apply();
        byte[] bytes = image.EncodeToPNG();
        System.IO.File.WriteAllBytes(filePath, bytes);
        Debug.Log("Image saved to: " + filePath);
            
        }
    }

    public void ParseFile()
    {
        // Reads the transforms.json file
        TransformData data = JsonConvert.DeserializeObject<TransformData>(textFile.text);

        _width = data.w;
        _height = data.h;


        if (_camera)
        {
            _renderTexture = new RenderTexture(_width, _height, 24);
            _camera.targetTexture = _renderTexture;
        }

        _positions = new Vector3[data.frames.Length];
        _rotations = new Quaternion[data.frames.Length];

        Debug.Log("Parsed " + _positions.Length + " frames.");


        for (int i = 0; i < data.frames.Length; i++)
        {
            Frame frame = data.frames[i];
            float[][] matrix = frame.transform_matrix;
            Matrix4x4 m = new Matrix4x4();
            m.SetRow(0, new Vector4(matrix[0][0], matrix[0][2], matrix[0][1], matrix[0][3])); // Swap Y and Z coordinates
            m.SetRow(1, new Vector4(matrix[2][0], matrix[2][2], matrix[2][1], matrix[2][3])); // Swap Y and Z coordinates
            m.SetRow(2, new Vector4(matrix[1][0], matrix[1][2], matrix[1][1], matrix[1][3])); // Swap Y and Z coordinates
            m.SetRow(3, new Vector4(matrix[3][0], matrix[3][2], matrix[3][1], matrix[3][3])); // Swap Y and Z coordinates

            Vector3 position = m.GetColumn(3);
            Quaternion rotation = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));

            _positions[i] = position;
            _rotations[i] = rotation;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Std;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ImageView))]
public class ImageViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ImageView imageView = (ImageView)target;
        if (GUILayout.Button("Click"))
        {
            imageView.OnClick();
        }
        if (GUILayout.Button("Select First Item"))
        {
            imageView.OnSelect(1);
        }
    }
}
#endif

public class ImageView : MonoBehaviour
{
    public Dropdown dropdown;
    public GameObject topMenu;
    public RawImage _uiImage;
    public CameraManager manager;
    public TMPro.TextMeshProUGUI name;
    public Sprite untracked;
    public Sprite tracked;

    public string topicName;

    private Texture2D _texture2D;

    private int _lastSelected = 0;

    public bool _tracking = false;
    private GameObject _frustrum;
    private Image _icon;
    ROSConnection ros;


    public bool CleanTF(string name)
    {
        GameObject target = GameObject.Find(name);

        List<GameObject> children = new List<GameObject>();

        // check if this is connected to root
        int count = 0;
        while(target.transform.parent != null)
        {
            count++;
            children.Add(target);
            target = target.transform.parent.gameObject;
            if(target.name == "odom")
            {
                children.Clear();
                Debug.Log("Connected to root");
                return true;
            }
            if(count > 100)
            {
                Debug.LogError("Looping too much");
                return false;
            }
        }

        foreach(GameObject child in children)
        {
            Destroy(child);
        }
        return false;
    }


    void UpdatePose(string frame)
    {
        if(!CleanTF(frame))
        {
            return;
        }
        GameObject _parent = GameObject.Find(frame);
        if(_parent == null) return;

        transform.parent = _parent.transform;
        transform.localPosition = new Vector3(0.1f, 0.2f, 0);
        transform.localRotation = Quaternion.Euler(-90, 90, 180);
        // transform.localScale = new Vector3(-1, 1, 1);
    }

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        dropdown.onValueChanged.AddListener(OnSelect);

        dropdown.gameObject.SetActive(false);
        topMenu.SetActive(false);

        ros.GetTopicAndTypeList(UpdateTopics);
        name.text = "None";

        _icon = topMenu.transform.Find("Track/Image/Image").GetComponent<Image>();
        _frustrum = transform.Find("Frustrum").gameObject;
    }

    private void OnDestroy() {
        ros.Unsubscribe(topicName);
    }

    void UpdateTopics(Dictionary<string, string> topics)
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        options.Add("None");
        foreach (var topic in topics)
        {
            if (topic.Value == "sensor_msgs/Image" || topic.Value == "sensor_msgs/CompressedImage")
            {
                // issue with depth images at the moment
                if (topic.Key.Contains("depth")) continue;
                options.Add(topic.Key);
            }
        }
        dropdown.AddOptions(options);

        dropdown.value = Mathf.Min(_lastSelected, options.Count - 1);
    }

    public void Clear()
    {
        manager.Remove(gameObject);
    }

    public void ToggleTrack()
    {
        _tracking = !_tracking;

        _icon.sprite = _tracking ? tracked : untracked;
        dropdown.gameObject.SetActive(false);
        topMenu.SetActive(false);
    }

    public void OnClick()
    {
        ros.GetTopicAndTypeList(UpdateTopics);
        dropdown.gameObject.SetActive(!dropdown.gameObject.activeSelf);
        topMenu.gameObject.SetActive(dropdown.gameObject.activeSelf);
    }

    public void OnSelect(int value)
    {
        if (value == _lastSelected) return;
        _lastSelected = value;
        if (topicName != null)
            ros.Unsubscribe(topicName);

        name.text = dropdown.options[value].text;

        if (value == 0)
        {
            topicName = null;
            // set texture to grey
            _uiImage.texture = null;
            
            dropdown.gameObject.SetActive(false);
            topMenu.SetActive(false);
            return;
        }

        topicName = dropdown.options[value].text;

        if (topicName.EndsWith("compressed"))
        {
            ros.Subscribe<CompressedImageMsg>(topicName, OnCompressed);
        }
        else
        {
            ros.Subscribe<ImageMsg>(topicName, OnImage);
        }
        dropdown.gameObject.SetActive(false);
        topMenu.SetActive(false);

    }


    void SetupTex(int width = 2, int height = 2)
    {
        if (_texture2D == null)
        {
            _texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
            _uiImage.texture = _texture2D;
            _uiImage.color = Color.white;
        }
    }

    void Resize()
    {
        if (_texture2D == null) return;

        float aspectRatio = (float)_texture2D.width/(float)_texture2D.height;
        float height = _uiImage.rectTransform.sizeDelta.y;
        float width = height * aspectRatio;
        
        _uiImage.rectTransform.sizeDelta = new Vector2(width,_uiImage.rectTransform.sizeDelta.y);
    }

    void ParseHeader(HeaderMsg header)
    {

        if (_tracking)
        {
            // If we are tracking to the TF, update the parent
            if(header.frame_id != null && (transform.parent == null || header.frame_id != transform.parent.name))
            {
                _frustrum.SetActive(true);
                // If the parent is not the same as the frame_id, update the parent
                UpdatePose(header.frame_id);
            }

        } else if (transform.parent != null && transform.parent.name != "odom")
        {
            _frustrum.SetActive(false);
            // Otherwise, set the parent to the odom frame but keep the current position
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            UpdatePose("odom");
            transform.position = pos;
            transform.rotation = rot;
        }
    }

    void OnCompressed(CompressedImageMsg msg)
    {
        SetupTex();
        ParseHeader(msg.header);

        try
        {
            ImageConversion.LoadImage(_texture2D, msg.data);

            _texture2D.Apply();

            // debayer the image using bilinear interpolation for rggb format

            // Color[] pixels = _texture2D.GetPixels();
            // int width = _texture2D.width;
            // int height = _texture2D.height;

            // for(int i = 0; i < pixels.Length; i++)
            // {
            //     int x = i % width;
            //     int y = i / width;

            //     // check for the r, g, g, b pattern and interpolate
            //     // check for out of bounds issues
            //     if (x < width - 1 && y < height - 1)
            //     {
            //         if (x % 2 == 0 && y % 2 == 0)
            //         {
            //             // red
            //             pixels[i] = (pixels[i] + pixels[i + 1] + pixels[i + width] + pixels[i + width + 1]) / 4;
            //         }
            //         else if (x % 2 == 1 && y % 2 == 0)
            //         {
            //             // green
            //             pixels[i] = (pixels[i - 1] + pixels[i] + pixels[i + width - 1] + pixels[i + width]) / 4;
            //         }
            //         else if (x % 2 == 0 && y % 2 == 1)
            //         {
            //             // green
            //             pixels[i] = (pixels[i - width] + pixels[i - width + 1] + pixels[i] + pixels[i + 1]) / 4;
            //         }
            //         else if (x % 2 == 1 && y % 2 == 1)
            //         {
            //             // blue
            //             pixels[i] = (pixels[i - width - 1] + pixels[i - width] + pixels[i - 1] + pixels[i]) / 4;
            //         }
            //     }
            // }

            // _texture2D.SetPixels(pixels);


            // _texture2D.Apply();

            Resize();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    void OnImage(ImageMsg msg)
    {
        SetupTex((int)msg.width, (int)msg.height);
        ParseHeader(msg.header);

        try
        {

            _texture2D.LoadRawTextureData(msg.data);
            _texture2D.Apply();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
        Resize();
    }

}

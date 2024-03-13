using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject imagePrefab;
    public TMPro.TextMeshProUGUI Count;
    public Sprite untracked;
    public Sprite tracked;
    private List<GameObject> imgs;
    private ROSConnection ros;
    private bool _allTracking = false;
    private Image _icon;

    private void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        menu.SetActive(false);
        imgs = new List<GameObject>();
        Count.text = imgs.Count.ToString();
        _icon = menu.transform.Find("Track/Image/Image").GetComponent<Image>();
    }

    public void Remove(GameObject img) 
    {
        imgs.Remove(img);
        Count.text = imgs.Count.ToString();
        Destroy(img);
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }

    public void AddImage()
    {
        GameObject img = Instantiate(imagePrefab, transform.position + (transform.right * 0.5f), Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up));
        img.GetComponent<ImageView>().manager = this;
        img.GetComponent<ImageView>()._tracking = _allTracking;
        imgs.Add(img);
        Count.text = imgs.Count.ToString();
    }

    public void TrackAll()
    {
        _allTracking = !_allTracking;
        foreach (GameObject img in imgs)
        {
            img.GetComponent<ImageView>()._tracking = _allTracking;
        }
        _icon.sprite = _allTracking ? tracked : untracked;
    }
    public void ClearAll()
    {
        foreach (GameObject img in imgs)
        {
            Destroy(img);
        }
        imgs.Clear();
        Count.text = imgs.Count.ToString();
    }

}

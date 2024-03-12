using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Robotics.ROSTCPConnector;

public class CameraManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject imagePrefab;
    public TMPro.TextMeshProUGUI Count;
    private List<GameObject> imgs;
    private ROSConnection ros;

    public void Awake()
    {
        imgs = new List<GameObject>();
    }

    private void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        menu.SetActive(false);

        Count.text = imgs.Count.ToString();
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
        GameObject img = Instantiate(imagePrefab, transform.position - transform.right, Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up));
        img.GetComponent<ImageView>().manager = this;
        imgs.Add(img);
        Count.text = imgs.Count.ToString();
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

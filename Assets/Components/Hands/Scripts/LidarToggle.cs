using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarToggle : MonoBehaviour
{
    public GameObject vizSuite;

    public void ToggleLidar()
    {
        vizSuite.SetActive(!vizSuite.activeSelf);
    }
}

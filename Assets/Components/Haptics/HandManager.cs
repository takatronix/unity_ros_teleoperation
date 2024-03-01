using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.SDK2;

public class HandManager : MonoBehaviour
{

    public static HandManager Instance { get; private set;}
    public float duration = 0.5f;

    private bool started = false;
    private long lastTime = 0;
    private int[] leftHand;
    private int[] rightHand;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void CheckDevices()
    {
        Debug.Log("PlayRainbow");
        List<HapticDevice> devices = BhapticsLibrary.GetDevices();

        foreach (HapticDevice device in devices)
        {
            Debug.Log("Device: " + device.DeviceName);
        }
        started = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        CheckDevices();
        leftHand = new int[6];
        rightHand = new int[6];
        
    }

    // Update is called once per frame
    void Update()
    {
        if(started)
        {
            if(Time.time - lastTime > duration)
            {
                lastTime = (long)Time.time;
                BhapticsLibrary.PlayMotors(
                    (int)Bhaptics.SDK2.PositionType.GloveR,
                    rightHand,
                    (int)(1000*duration)
                );
                BhapticsLibrary.PlayMotors(
                    (int)Bhaptics.SDK2.PositionType.GloveL,
                    leftHand,
                    (int)(1000*duration)
                );
            }
        }
        
    }

    public void UpdateValue(bool isRight, int index, int value)
    {
        if(isRight)
        {
            rightHand[index] = value;
        }
        else
        {
            leftHand[index] = value;
        }
    }
}

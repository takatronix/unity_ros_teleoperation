using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.AudioCommon;
using System;

public class AudioReceiver : MonoBehaviour
{
    private ROSConnection ros;
    private AudioSource audioSource;
    private List<float> audioBuffer = new List<float>();
    private string audioDataTopic = "/audio/audio";
    private int sampleRate = 16000;
    private int channelCount = 1;


    // Start is called before the first frame update
    void Start()
    {   
        // Creates audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.0f;

        ros = ROSConnection.GetOrCreateInstance(); 
        ros.Subscribe<AudioDataMsg>(audioDataTopic, ReceiveAudioMessage);
    }

    // Update is called once per frame
    void Update()
    {
        if (audioBuffer.Count > 0 && !audioSource.isPlaying)
        {
            PlayBufferedAudio();
        }

    }
    
   
    // Callback function for receiving audio data
    void ReceiveAudioMessage(AudioDataMsg msg)
    {
        byte[] byteData = msg.data;
        float[] floatData = new float[byteData.Length / 2];
        for (int i = 0; i < floatData.Length; i += 1)
        {
            short sample = (short)(byteData[i * 2] | (byteData[i * 2 + 1] << 8));
            floatData[i] = sample / 32768.0f;
        }

        lock (audioBuffer)
        {
            audioBuffer.AddRange(floatData);
        }
    }

    // Play audio from buffer

    void PlayBufferedAudio()
    {
        lock (audioBuffer)
        {
            AudioClip clip = AudioClip.Create("Audio", audioBuffer.Count, channelCount, sampleRate, false);
            float[] floatData = audioBuffer.ToArray();
            clip.SetData(floatData, 0);
            audioSource.clip = clip;
            audioSource.Play();
            audioBuffer.Clear();
        }
    }


    // OnDestroy is called when the object is destroyed
    void OnDestroy()
    {
        if (ros != null)
        {   
            ros.Unsubscribe(audioDataTopic);
            ros.Disconnect();
        }
    }
}

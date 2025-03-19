using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorStream : MonoBehaviour
{
    /// <summary>
    /// Reference back to the manager that spawned this sensor
    /// </summary>
    public SensorManager manager;

    /// <summary>
    /// Sets what tracking mode we should be in, changes based on the sensor
    /// </summary>
    public abstract void ToggleTrack(int mode);

    /// <summary>
    /// Converts the state of this sensor into a string so that it can be reinitialized later
    /// </summary>
    public abstract string Serialize();

    /// <summary>
    /// Converts a string into the state of this sensor
    /// </summary>
    public abstract void Deserialize(string data);
}

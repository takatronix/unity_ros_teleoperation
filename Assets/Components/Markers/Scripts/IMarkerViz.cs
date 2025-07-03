using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public interface IMarkerViz
{
    public void SetData(PoseMsg pose, Vector3Msg scale, ColorRGBAMsg[] colors, PointMsg[] points);
}

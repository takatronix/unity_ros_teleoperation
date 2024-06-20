// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Collections;
using System.Collections.Generic;
public class Frame
{
    public float[][] transform_matrix { get; set; }

    public override string ToString()
    {
        string res = "";
        foreach(var row in transform_matrix)
        {
            foreach(var item in row)
                res += item + " ";
            res += "\n";
        }

        return res;
    }
}

public class TransformData
{
    public double cx { get; set; }
    public double cy { get; set; }
    public int w { get; set; }
    public int h { get; set; }
    public string camera_model { get; set; }
    public Frame[] frames { get; set; }

    public override string ToString()
    {
        string res = "";
        res += "Size: " + w + "x"+h+"\n";
        res += "Frames: "+frames.Length + "\n";
        res += "Frame 0: "+frames[0];

        return res;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Splat", menuName = "Splat Object", order = 1)]
public class SplatObject : ScriptableObject
{
    public GraphicsBuffer positions;
    public GraphicsBuffer shData;
    

}

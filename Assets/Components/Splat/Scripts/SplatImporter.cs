using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.AssetImporters;
using System.IO;

[ScriptedImporter(1, new[] { "splat", "ply" })]
public class SplatImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        // check if the file is a ply file
        
    }
}

#endif
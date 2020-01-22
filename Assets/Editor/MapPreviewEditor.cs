using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapPreview mapPreview = (MapPreview)target;
        // Adding auto-generating with changing Map Parameters
        if (DrawDefaultInspector())
        {
            if (mapPreview.autoUpdate)
            {
                mapPreview.DrawMapInEditor();
            }
        }
        // Adding Generate Button
        if (GUILayout.Button("Generate"))
        {
            mapPreview.DrawMapInEditor();
        }
    }
}

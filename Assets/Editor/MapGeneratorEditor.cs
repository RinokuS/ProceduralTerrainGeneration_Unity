using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        // Добавляем авто-генерацию, при изменении параметров карты
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }
        // Добавляем кнопочку для генерации карты
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}

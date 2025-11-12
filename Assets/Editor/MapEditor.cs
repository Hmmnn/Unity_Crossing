
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapManager mapGen = (MapManager)target;
        DrawDefaultInspector();

        //if (GUILayout.Button("Generate"))
        //    mapGen.CreateMap();
    }
}
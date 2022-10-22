using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class ClearDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Clear All Game Save Data"))
        {
            SaveSystem.clearData();
        }
    }
}
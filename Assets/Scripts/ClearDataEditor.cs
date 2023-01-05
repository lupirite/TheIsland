using System.Collections;
using UnityEditor;
using UnityEngine;

// Comment out before build
///*
[CustomEditor(typeof(GameManager))]
public class ClearDataEditor : Editor
{
    // Comment out before build
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Clear All Game Save Data"))
        {
            SaveSystem.clearData();
        }
    }
}
//*/
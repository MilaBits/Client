using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConnectableGroup))]
public class ConnectableGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ConnectableGroup connectableGroup = (ConnectableGroup) target;
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Update Grid"))
        {
            connectableGroup.UpdateGridContent();
        }
    }
}
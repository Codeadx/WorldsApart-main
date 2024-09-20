using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TopCube))]
public class TopEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TopCube topCube = (TopCube)target;
        if (DrawDefaultInspector()) {
            if(topCube.autoUpdate) {
                topCube.DrawMapInEditor();
           }
        }
    }

}

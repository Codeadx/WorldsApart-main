using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FrameworkRefactored))]
public class FrameworkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FrameworkRefactored framework = (FrameworkRefactored)target;
        if (DrawDefaultInspector()) {
            if(framework.autoUpdate) {
                framework.UpdateInEditor();
           }
        }
    }

}

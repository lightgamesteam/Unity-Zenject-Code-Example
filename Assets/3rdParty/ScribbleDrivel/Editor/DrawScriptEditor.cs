using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LylekGames
{
    [CustomEditor(typeof(DrawScript))]
    public class DrawScriptEditor : Editor
    {
        DrawScript myDrawScript;
        
        public override void OnInspectorGUI()
        {
            DrawScript myDrawScript = (DrawScript)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Reset to Default Settings"))
            {
                myDrawScript.GetDefaultSettings();
            }
        }
    }
}

using UnityEngine;
using UnityEditor;

namespace Module.PeriodicTable
{
    [CustomEditor(typeof(PeriodicTableView))]
    public class TableBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PeriodicTableView myScript = (PeriodicTableView) target;

            var styleСarefully = new GUIStyle(GUI.skin.button);
            styleСarefully.normal.textColor = Color.red;

            if (GUILayout.Button("Build Periodic Table"))
            {
                myScript.BuildTable();
            }

            if (GUILayout.Button("Clean Periodic Table", styleСarefully))
            {
                myScript.ClearTable();
            }
        }
    }
}
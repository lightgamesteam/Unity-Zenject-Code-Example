using UnityEditor;
using UnityEngine;

namespace Module.PeriodicTable
{
    [CustomPropertyDrawer(typeof(Description))]
    public class PointEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            Rect contentPosition = EditorGUI.PrefixLabel(position, label);
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.width *= 0.5f;
            contentPosition.height *= 1.1f;
            EditorGUI.indentLevel = 0;


            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("Name"), GUIContent.none);
            contentPosition.x += contentPosition.width;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("Value"), GUIContent.none);




            /*EditorGUI.BeginChangeCheck();
            string color = EditorGUI.TextField(contentPosition, GUIContent.none, Y.stringValue);
            if (EditorGUI.EndChangeCheck())
                Y.stringValue = color;
            EditorGUI.EndProperty();*/
        }
    }
}
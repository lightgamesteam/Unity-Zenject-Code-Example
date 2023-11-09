using System.Globalization;
using Module.Core.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        string result;
        switch (property.propertyType) {
            case SerializedPropertyType.Integer:
                result = property.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                result = property.boolValue.ToString();
                break;
            case SerializedPropertyType.Float:
                result = property.floatValue.ToString(CultureInfo.InvariantCulture);
                break;
            case SerializedPropertyType.String:
                result = property.stringValue;
                break;
            case SerializedPropertyType.Enum:
                result = property.enumNames[property.enumValueIndex];
                break;
            default:
                result = property.objectReferenceValue == null? "no connection" : property.objectReferenceValue.ToString();
                break;
        }

        EditorGUI.LabelField(position, label.text, result, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
    }
}

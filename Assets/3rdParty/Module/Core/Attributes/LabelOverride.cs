using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Module.Core.Attributes {
    public class LabelOverride : PropertyAttribute {
        public string Label;

        public LabelOverride(string label) {
            Label = label;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LabelOverride))]
    public class LabelOverrideDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            try {
                var propertyAttribute = attribute as LabelOverride;
                if (propertyAttribute == null) {
                    return;
                }
                if (IsArray(property) == false) {
                    label.text = propertyAttribute.Label;
                } else {
                    UnityEngine.Debug.LogWarningFormat("{0}(\"{1}\") doesn't support arrays ",
                        typeof(LabelOverride).Name, propertyAttribute.Label
                    );
                }
                EditorGUI.PropertyField(position, property, label);
            } catch (System.Exception ex) {
                UnityEngine.Debug.LogException(ex);
            }
        }

        private static bool IsArray(SerializedProperty property) {
            var path = property.propertyPath;
            var dotIndex = path.IndexOf('.');
            if (dotIndex == -1) {
                return false;
            }

            var propertyName = path.Substring(0, dotIndex);
            var propertyResult = property.serializedObject.FindProperty(propertyName);
            return propertyResult.isArray;
            //CREDITS: https://answers.unity.com/questions/603882/serializedproperty-isnt-being-detected-as-an-array.html
        }
    }
#endif
}
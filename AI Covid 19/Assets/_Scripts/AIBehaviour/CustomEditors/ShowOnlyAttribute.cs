using Covid19.Utils;
using UnityEditor;
using UnityEngine;

namespace Covid19.AIBehaviour.CustomEditors
{
    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, prop, label);
            GUI.enabled = true;
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Covid19.Learn.Custom_Editors.Editor
{
    [CustomEditor(typeof(MyComponent))]
    public class MyCustomScriptEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MyComponent component = target as MyComponent;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Nume");
            component.numar = EditorGUILayout.IntField(component.numar);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.PrefixLabel("Position");
            component.position = EditorGUILayout.Vector3Field("Position", component.position);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("This is up to date", MessageType.Warning);
            if (GUILayout.Button("Generate Object"))
                component.InstantiateGo();
            base.OnInspectorGUI();

        }
    }
}

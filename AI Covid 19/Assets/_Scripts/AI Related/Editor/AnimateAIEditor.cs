using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AnimateAI))]
public class AnimateAIEditor : Editor
{
    bool foldout;
    bool toggle;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        foldout = EditorGUILayout.Foldout(foldout, "Behaviour Settings");
        if (foldout)
        {
            GUILayout.BeginHorizontal();
            toggle = EditorGUILayout.Toggle(toggle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("turnMulitplyer"));
            GUILayout.EndHorizontal();
        }
        
    }
}
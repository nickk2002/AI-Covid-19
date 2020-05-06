using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(BotAction))]
[CanEditMultipleObjects]
public class BotActionEditor : PropertyDrawer
{
    bool foldout = false;
    Rect rectButton;
    float lungime;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (foldout)
        {

            return rectButton.y + 20;
        }
        return base.GetPropertyHeight(property, label);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),new GUIContent(label));
        foldout = EditorGUI.Foldout(new Rect(position.x,position.y,position.width,15), foldout, label.text);
        if(foldout == true)
        {
            var aduna = 20;
            var probability = property.FindPropertyRelative("probability");
            var stopDistance = property.FindPropertyRelative("stopDistance");
            var rectprob = new Rect(position.x, position.y + 20, position.width, 16);
            var rectstopDistance = new Rect(rectprob.x, rectprob.y + aduna, position.width, 16);
            var targetT = property.FindPropertyRelative("targetTransform");
            
            var rectTransfrom = new Rect(position.x, rectstopDistance.y + aduna, position.width, 16f);
            var botPosition = property.FindPropertyRelative("position");
            var rectPos = new Rect(rectTransfrom.x, rectTransfrom.y + aduna, position.width, 16);
            rectButton = new Rect(rectPos.x, rectPos.y + 40 , position.width, 16);
            var botRotation = property.FindPropertyRelative("rotation");
            lungime = rectButton.y + aduna;

            //new school
            //EditorGUI.indentLevel -= 2;
            //EditorGUILayout.BeginVertical();
            //GUILayout.Space(30);

            //EditorGUILayout.PropertyField(probability);
            //EditorGUILayout.PropertyField(stopDistance);
            //EditorGUILayout.PropertyField(targetT, true);
            //EditorGUILayout.EndVertical();
            EditorGUI.PropertyField(rectprob, probability);
            EditorGUI.PropertyField(rectstopDistance, stopDistance);
            EditorGUI.PropertyField(rectTransfrom, targetT);
            EditorGUI.PropertyField(rectPos, botPosition);


            Bot ceva = GetParent(property) as Bot;
            GameObject go = ceva.gameObject;           
            
            if (GUI.Button(rectButton,"Set Bot Relative Position"))
            {
                botPosition.vector3Value = go.transform.position;
                botRotation.quaternionValue = go.transform.rotation;

            }
        }
        EditorGUI.EndProperty();
    }
    public object GetParent(SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements.Take(elements.Length - 1))
        {
            if (element.Contains("["))
            {
                Debug.Log("Containes that thing");
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(obj, elementName, index);
            }
            else
            {
                obj = GetValue(obj, element);
            }
        }
        
        return obj;
    }
    public object GetValue(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();
        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (f == null)
        {
            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null)
                return null;
            return p.GetValue(source, null);
        }
        return f.GetValue(source);
    }

    public object GetValue(object source, string name, int index)
    {
        var enumerable = GetValue(source, name) as IEnumerable;
        var enm = enumerable.GetEnumerator();
        while (index-- >= 0)
            enm.MoveNext();
        return enm.Current;
    }

}

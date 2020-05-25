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
    Rect rectSetPosition;
    Rect rectSnap;
    Rect rectPos;
    float lungime;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (foldout)
        {
            return 180;
        }
        return base.GetPropertyHeight(property, label);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Debug.Log("Entering on gui");
        EditorGUI.BeginProperty(position, label, property);
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),new GUIContent(label));
        foldout = EditorGUI.Foldout(new Rect(position.x,position.y,position.width,15), foldout, label.text);

        if(foldout == true)
        {
            property.serializedObject.ApplyModifiedProperties();
            var aduna = 20;
            var probability = property.FindPropertyRelative("probability");
            
            var stopDistance = property.FindPropertyRelative("stopDistance");
            var rectprob = new Rect(position.x, position.y + 20, position.width, 16);
            var rectstopDistance = new Rect(rectprob.x, rectprob.y + aduna, position.width, 16);

            var targetT = property.FindPropertyRelative("targetTransform");
            var rectTransfrom = new Rect(position.x, rectstopDistance.y + aduna, position.width, 16f);

            var botPosition = property.FindPropertyRelative("position");
            rectPos = new Rect(rectTransfrom.x, rectTransfrom.y + aduna, position.width, 16);

            var place = property.FindPropertyRelative("place");
            var rectPlace = new Rect(rectPos.x, rectPos.y + aduna * 2, position.width, 16);
            
            rectSetPosition = new Rect(rectPlace.x, rectPlace.y + aduna, position.width, 16);
            var botRotation = property.FindPropertyRelative("rotation");
            lungime = rectSetPosition.y + aduna;

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
            EditorGUI.PropertyField(rectPlace, place);
            lungime = 140;


            Bot ceva = GetParent(property) as Bot;
            GameObject go = ceva.gameObject;
            BotAction action = fieldInfo.GetValue(property.serializedObject.targetObject) as BotAction;

            lungime += aduna;
            if (action.targetTransform != null)
            {
                Debug.Log(action.targetTransform);
                GameObject targetGameobject = action.targetTransform.gameObject;
               
                if (targetGameobject.GetComponent<ActionPlace>() == null)
                {
                    targetGameobject.AddComponent<ActionPlace>();
                    Debug.Log("addint action by editor script");
                }
                ActionPlace targetPlace = targetGameobject.GetComponent<ActionPlace>();
                targetPlace.type = action.place;
            }
            if (GUI.Button(rectSetPosition, "Set Bot Relative Position"))
            {

                if (action.targetTransform == null)
                {
                    botPosition.vector3Value = go.transform.position;
                }
                else
                {
                    botPosition.vector3Value = go.transform.position - action.targetTransform.position;/// diferenta intre vectori
                }
                botRotation.quaternionValue = go.transform.rotation;
            }
            rectSnap = new Rect(rectPos.x, rectSetPosition.y + aduna, position.width, 16);
            if (GUI.Button(rectSnap, "Snap Object To Position"))
            {
                //Debug.Log("the position is now : " )
                Vector3 snap = botPosition.vector3Value + action.targetTransform.position;
                Debug.Log("Going to put him at position : " + snap);
                go.transform.position = action.position + action.targetTransform.position;
                go.transform.rotation = action.rotation;
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

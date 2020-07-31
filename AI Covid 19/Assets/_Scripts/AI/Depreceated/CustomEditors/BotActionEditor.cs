using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Covid19.AI.Depreceated.Actions;
using UnityEditor;
using UnityEngine;

namespace Covid19.AI.Depreceated.CustomEditors
{
    [CustomPropertyDrawer(typeof(BotAction))]
    [CanEditMultipleObjects]
    public class BotActionEditor : PropertyDrawer
    {
        private bool _foldout = false;
        private Rect _rectSetPosition;
        private Rect _rectSnap;
        private Rect _rectPos;
        private float _lungime;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_foldout) return _lungime;
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Debug.Log("Entering on gui");
            EditorGUI.BeginProperty(position, label, property);
            _foldout = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, 15), _foldout, label.text);
            if (_foldout)
            {
                var pos = new Rect(position);
                pos.width = position.width;
                pos.height = 16;
                property.serializedObject.ApplyModifiedProperties();
                var aduna = 20;
                var probability = property.FindPropertyRelative("probability");
                var rectprob = new Rect(position.x, position.y + aduna, position.width, 16);
                var stopDistance = property.FindPropertyRelative("stopDistance");

                var rectstopDistance = new Rect(rectprob.x, rectprob.y + aduna, position.width, 16);

                var target = property.FindPropertyRelative("targetTransform");
                var rectTransfrom = new Rect(position.x, rectstopDistance.y + aduna, position.width, 16f);

                var botPosition = property.FindPropertyRelative("position");

                var rectPos = new Rect(rectTransfrom.x, rectTransfrom.y + aduna, position.width, 16);

                var place = property.FindPropertyRelative("place");
                var rectPlace = new Rect(rectPos.x, rectPos.y + aduna, position.width, 16);

                _rectSetPosition = new Rect(rectPlace.x, rectPlace.y + aduna, position.width, 16);
                var botRotation = property.FindPropertyRelative("rotation");
                _lungime = _rectSetPosition.y + aduna;

                EditorGUI.PropertyField(rectprob, probability);
                EditorGUI.PropertyField(rectstopDistance, stopDistance);
                EditorGUI.PropertyField(rectTransfrom, target);
                EditorGUI.PropertyField(rectPos, botPosition);
                EditorGUI.PropertyField(rectPlace, place);


                var ceva = GetParent(property) as Bot;
                var go = ceva.gameObject;
                var action = fieldInfo.GetValue(property.serializedObject.targetObject) as BotAction;

                _lungime += aduna;
                if (action.targetTransform != null)
                {
                    Debug.Log(action.targetTransform);
                    var targetGameobject = action.targetTransform.gameObject;

                    if (targetGameobject.GetComponent<ActionPlace>() == null)
                    {
                        targetGameobject.AddComponent<ActionPlace>();
                        Debug.Log("addint action by editor script");
                    }

                    var targetPlace = targetGameobject.GetComponent<ActionPlace>();
                    targetPlace.type = action.place;
                    targetPlace.stopDistance = stopDistance.floatValue;
                }

                if (GUI.Button(_rectSetPosition, "Set Bot Relative Position"))
                {
                    if (action.targetTransform == null)
                        botPosition.vector3Value = go.transform.position;
                    else
                        botPosition.vector3Value =
                            go.transform.position - action.targetTransform.position; /// diferenta intre vectori
                    botRotation.quaternionValue = go.transform.rotation;
                }

                _rectSnap = new Rect(rectPos.x, _rectSetPosition.y + aduna, position.width, 16);
                if (GUI.Button(_rectSnap, "Snap Object To Position"))
                {
                    //Debug.Log("the position is now : " )
                    var snap = botPosition.vector3Value + action.targetTransform.position;
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
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
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
                var p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
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
}
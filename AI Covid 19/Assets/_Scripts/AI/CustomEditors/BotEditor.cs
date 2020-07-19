using System;
using System.Linq;
using Covid19.AI.SaveData;
using UnityEditor;
using UnityEngine;

namespace Covid19.AI.CustomEditors
{
    [CustomEditor(typeof(Bot))]
    [CanEditMultipleObjects]
    public class BotEditor : Editor
    {
        private bool _foldout;
        private bool _generalSettings;
        private Bot _bot;
        private bool _upToDate;

        private void OnEnable()
        {
            _bot = (Bot) target; // set the bot target
            Debug.Log("call on enable on the custom bot");
            LoadEveryThing();
        }

        private void LoadEveryThing()
        {
            // keep foldouts the way they were before
            _foldout = EditorPrefs.GetBool("foldout");
            _generalSettings = EditorPrefs.GetBool("general");

            _bot.SetUpReflection();
        }

        private void ListLoading()
        {
            var index = 0;
            var toggleListProperty = serializedObject.FindProperty("toggleList"); // get the toggle list
            _bot.ClearList();

            Debug.Log("loading the list");
            foreach (var action in _bot.reflectionActions)
                if (index < _bot.toggleList.Count)
                {
                    // begin a horizontal allignment
                    GUILayout.BeginHorizontal();

                    // se the value of the toggle
                    if (index >= toggleListProperty.arraySize)
                        return;
                    var toggleIndex = toggleListProperty.GetArrayElementAtIndex(index);
                    toggleIndex.boolValue = EditorGUILayout.Toggle(toggleIndex.boolValue, GUILayout.Width(16));
                    var item = _bot.toggleList[index];

                    index++; /// increase the index

                    GUILayout.Space(10); // ad some space
                    var name = action.name; // get the name of the action

                    if (item)
                    {
                        // draw the expandable action
                        _bot.AddAction(action);
#if UNITY_EDITOR
                        if (!EditorApplication.isPlaying)
                            Debug.Log("serializing : " + action.name);
#endif
                        Debug.Assert(serializedObject.FindProperty(name) != null);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), GUILayout.MinWidth(100));
                    }
                    else
                    {
                        // draw a label
                        _bot.RemoveAction(action);
#if UNITY_EDITOR
                        if (!EditorApplication.isPlaying)
                            Debug.Log(name);
#endif
                        var actualName = char.ToUpper(name[0]) + name.Substring(1);
                        EditorGUILayout.LabelField(actualName);
                    }

                    GUILayout.EndHorizontal();
                }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawImage()
        {
            // draw Texturee
            Texture texture;
            string text;

            if (_bot.reflectionActions.SequenceEqual(SaveSystem.LoadBotActions()))
            {
                texture = Resources.Load("tick") as Texture;
                _upToDate = true;
                text = "Everything up to date";
            }
            else
            {
                texture = Resources.Load("cross") as Texture;
                _upToDate = false;
                text = "Not up to date";
            }
            // update the gameobject icon


            GUILayout.BeginHorizontal();
            GUILayout.Box(texture);
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label(text);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _bot.UpdateGizmosUI(_upToDate);

            // draw script gui
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Bot) target), typeof(Bot), false);
            GUI.enabled = true;

            //draw foldout
            _foldout = EditorGUILayout.Foldout(_foldout, "Behaviour Settings");
            EditorPrefs.SetBool("foldout", _foldout);
            serializedObject.Update();
            if (_foldout)
            {
                DrawImage();
                ListLoading();

                if (GUILayout.Button("Randomise Probabilities"))
                    foreach (var action in _bot.reflectionActions)
                    {
                        var actionProperty = serializedObject.FindProperty(action.name);
                        var probabilityProperty = actionProperty.FindPropertyRelative("probability");
                        probabilityProperty.floatValue = UnityEngine.Random.value;
                    }

                GUILayout.BeginHorizontal();
                if (!_upToDate && GUILayout.Button("Save Preset")) _bot.SavePreset();
                if (!_upToDate && GUILayout.Button("Revert changes")) _bot.UpdatePreset();
                if (_upToDate && GUILayout.Button("Update all other")) _bot.UpdateAllOthers();
                GUILayout.EndHorizontal();
            }

            _generalSettings = EditorGUILayout.Foldout(_generalSettings, "General Settings");
            EditorPrefs.SetBool("general", _generalSettings);
            if (_generalSettings)
                foreach (var name in _bot.reflectionFields)
                    try
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), true);
                    }
                    catch (NullReferenceException)
                    {
                        continue;
                    }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
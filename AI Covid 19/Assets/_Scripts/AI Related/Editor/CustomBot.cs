using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

[CustomEditor(typeof(Bot))]
[CanEditMultipleObjects]
public class CustomBot : Editor
{
    bool foldout;
    bool generalSettings;
    Bot bot;
    bool upToDate;

    private void OnEnable()
    {
        bot = (Bot)target; // set the bot target
        Debug.Log("call on enable on the custom bot");
        LoadEveryThing();
    }

    void LoadEveryThing()
    {
        // keep foldouts the way they were before
        foldout = EditorPrefs.GetBool("foldout");
        generalSettings = EditorPrefs.GetBool("general");

        bot.SetUpReflection();
    }
    void ListLoading()
    {
        int index = 0;
        SerializedProperty toggleListProperty = serializedObject.FindProperty("toggleList"); // get the toggle list
        bot.ClearList();

        Debug.Log("loading the list");
        foreach (BotAction action in bot.reflectionActions)
        {
            if (index < bot.toggleList.Count)
            {
                // begin a horizontal allignment
                GUILayout.BeginHorizontal();

                // se the value of the toggle
                if (index >= toggleListProperty.arraySize)
                    return;
                SerializedProperty toggleIndex = toggleListProperty.GetArrayElementAtIndex(index);
                toggleIndex.boolValue = EditorGUILayout.Toggle(toggleIndex.boolValue, GUILayout.Width(16));
                var item = bot.toggleList[index];

                index++;/// increase the index

                GUILayout.Space(10); // ad some space
                string name = action.name; // get the name of the action

                if (item == true)
                {
                    // draw the expandable action
                    bot.AddAction(action);
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying)
                        Debug.Log("serializing : " + action.name); 
#endif
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(name), GUILayout.MinWidth(100));
                }
                else
                {
                    // draw a label
                    bot.RemoveAction(action);
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying)
                        Debug.Log(name);         
#endif
                    string actualName = char.ToUpper(name[0]) + name.Substring(1);
                    EditorGUILayout.LabelField(actualName);
                }
                GUILayout.EndHorizontal();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    void DrawImage()
    {
        // draw Texturee
        Texture texture;
        string text;

        if (bot.reflectionActions.SequenceEqual(SaveSystem.LoadBotActions()))
        {
            texture = Resources.Load("tick") as Texture;
            upToDate = true;
            text = "Everything up to date";
        }
        else
        {
            texture = Resources.Load("cross") as Texture;
            upToDate = false;
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
        bot.UpdateGizmosUI(upToDate);

        // draw script gui
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Bot)target), typeof(Bot), false);
        GUI.enabled = true;

        //draw foldout
        foldout = EditorGUILayout.Foldout(foldout, "Behaviour Settings");
        EditorPrefs.SetBool("foldout", foldout);
        serializedObject.Update();
        if (foldout)
        {
            DrawImage();
            ListLoading();
            if (!upToDate && GUILayout.Button("Save Preset"))
            {
                bot.SavePreset();
            }
            if (!upToDate && GUILayout.Button("Revert changes"))
            {
                bot.UpdatePreset();
            }
            if (upToDate && GUILayout.Button("Update all other"))
            {
                bot.UpdateAllOthers();
            }
        }
        generalSettings = EditorGUILayout.Foldout(generalSettings, "General Settings");
        EditorPrefs.SetBool("general", generalSettings);
        if (generalSettings)
        {
            foreach (string name in bot.reflectionFields)
            {
                try
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(name), true);
                }
                catch (NullReferenceException)
                {
                    continue;
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}

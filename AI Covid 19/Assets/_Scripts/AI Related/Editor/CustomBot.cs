using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Bot))]
[CanEditMultipleObjects]
public class CustomBot : Editor
{



    bool foldout;
    bool generalSettings;
    List<string> fieldsArray = new List<string>();
    List<FieldInfo> actionArray = new List<FieldInfo>();
    Bot bot;
    bool start;

    private void OnEnable()
    {
        bot = (Bot)target; // set the bot target
        Debug.Log("added listener");
        LoadEveryThing();
    }
    private void OnDisable()
    {
        bot.loadEvent.RemoveAllListeners();
    }
    void LoadEveryThing()
    {
        // keep foldouts the way they were before
        foldout = EditorPrefs.GetBool("foldout");
        generalSettings = EditorPrefs.GetBool("general");

        // Debug.Log(bot.name + "in enabled");
        // reset fields and action arrays
        fieldsArray.Clear();
        actionArray.Clear();
        foreach (FieldInfo field in typeof(Bot).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default))
        {
            string nume = field.Name;
            if (nume.EndsWith("Action") && field.FieldType == typeof(BotAction) && field.IsPrivate == false)
            {
                actionArray.Add(field);
            }
            else
            {
                fieldsArray.Add(nume);
            }
        }
    }
    public override void OnInspectorGUI()
    {
      
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Bot)target), typeof(Bot), false);
        GUI.enabled = true;
        foldout = EditorGUILayout.Foldout(foldout, "Behaviour Settings");
        EditorPrefs.SetBool("foldout", foldout);

        if (foldout)
        {
            if (bot.toggleList.Count < actionArray.Count)
            {
                int dif = actionArray.Count - bot.toggleList.Count;
                for (int i = 1; i <= dif; i++)
                {
                    bot.toggleList.Add(false);
                }
            }
            int index = 0;
            SerializedProperty toggleListProperty = serializedObject.FindProperty("toggleList");
            bot.ClearList();
            foreach (FieldInfo fieldInfo in actionArray)
            {
                if (index < bot.toggleList.Count)
                {
                    GUILayout.BeginHorizontal();
                    //Debug.Log(index + " " + bot.toggleList.Count);
                    SerializedProperty toggleIndex = toggleListProperty.GetArrayElementAtIndex(index);
                    toggleIndex.boolValue = EditorGUILayout.Toggle(toggleIndex.boolValue, GUILayout.Width(16));
                    var item = bot.toggleList[index];
                    index++;

                    GUILayout.Space(10);
                    string name = fieldInfo.Name;

                    BotAction actiune = (BotAction)fieldInfo.GetValue(bot);
                    Debug.Log(actiune.ToString());
                    if (item == true)
                    {
                        bot.AddAction(actiune);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), GUILayout.MinWidth(100));
                    }
                    else
                    {
                        Debug.Log("remove");
                        bot.RemoveAction(actiune);
                        string actualName = char.ToUpper(name[0]) + name.Substring(1);
                        EditorGUILayout.LabelField(actualName);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            Debug.Log(bot.actionList.Count + "bot list");
            serializedObject.ApplyModifiedProperties();
        }
        generalSettings = EditorGUILayout.Foldout(generalSettings, "General Settings");
        EditorPrefs.SetBool("general", generalSettings);
        if (generalSettings)
        {
            foreach (string name in fieldsArray)
            {
                //Debug.Log("field name : " + name);
                try
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(name), true);
                }
                catch
                {
                    continue;
                }
            }
        }
        serializedObject.ApplyModifiedProperties();

    }
}

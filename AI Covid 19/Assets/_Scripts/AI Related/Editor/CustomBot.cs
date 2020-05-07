using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Bot))]
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
    void LoadEveryThing()
    {
        // keep foldouts the way they were before
        foldout = EditorPrefs.GetBool("foldout");
        generalSettings = EditorPrefs.GetBool("general");

        // Debug.Log(bot.name + "in enabled");
        // reset fields and action arrays
        fieldsArray.Clear();
        actionArray.Clear();
        /// use refelction to fill action array and fields array

        
        foreach (FieldInfo field in typeof(Bot).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default))
        {
            string nume = field.Name;
            if (nume.EndsWith("Action") && field.FieldType == typeof(BotAction) && field.IsPrivate == false)
            {
                BotAction action = field.GetValue(bot) as BotAction;
                FieldInfo nameField = typeof(BotAction).GetField("name");
                nameField.SetValue(action, field.Name);
                Debug.Log("now the name is : " + field.Name);
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

        // draw script gui
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Bot)target), typeof(Bot), false);
        GUI.enabled = true;

        //draw foldout
        foldout = EditorGUILayout.Foldout(foldout, "Behaviour Settings");
        EditorPrefs.SetBool("foldout", foldout);

        if (foldout)
        {
            // make sure the bot toggle size matches the actionArray size
            if (bot.toggleList.Count < actionArray.Count)
            {
                int dif = actionArray.Count - bot.toggleList.Count;
                for (int i = 1; i <= dif; i++)
                {
                    bot.toggleList.Add(false);
                }
            }
            int index = 0;
            SerializedProperty toggleListProperty = serializedObject.FindProperty("toggleList"); // get the toggle list
            bot.ClearList();
            foreach (FieldInfo fieldInfo in actionArray)
            {
                if (index < bot.toggleList.Count)
                {
                    // begin a horizontal allignment
                    GUILayout.BeginHorizontal();

                    // se the value of the toggle
                    SerializedProperty toggleIndex = toggleListProperty.GetArrayElementAtIndex(index);
                    toggleIndex.boolValue = EditorGUILayout.Toggle(toggleIndex.boolValue, GUILayout.Width(16));
                    var item = bot.toggleList[index];

                    index++;/// increase the index

                    GUILayout.Space(10); // ad some space
                    string name = fieldInfo.Name; // get the name of the action
                    
                    BotAction actiune = (BotAction)fieldInfo.GetValue(bot);
                    if (item == true)
                    {
                        // draw the expandable action
                        bot.AddAction(actiune);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), GUILayout.MinWidth(100));
                    }
                    else
                    {
                        // draw a label
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

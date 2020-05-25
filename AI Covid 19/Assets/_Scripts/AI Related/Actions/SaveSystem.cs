using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
//using System.Runtime.Serialization.Formatters.Binary.Formatters.Binary;

public class SaveSystem
{

    [System.Serializable]
    class ActionsKeeper
    {
        public List<BotActionData> actionDataList;
        public ActionsKeeper()
        {

        }
        public ActionsKeeper(List<BotActionData> actionData)
        {
            actionDataList = actionData;
        }
    }
    static public string GetJsonStringForBotActions()
    {
        string jsonPath = Application.persistentDataPath + "/savedActions.txt";
        string content = File.ReadAllText(jsonPath);
        return content;
    }
    static public void WriteIntroJsonForBotActions(string content)
    {
        string jsonPath = Application.persistentDataPath + "/savedActions.txt";
        File.WriteAllText(jsonPath, content);
    }
    static private List<BotActionData> ConvertFromBotActionToData(List<BotAction> actionList)
    {
        List<BotActionData> dataList = new List<BotActionData>();
        foreach (BotAction action in actionList)
            dataList.Add(new BotActionData(action));
        return dataList;
    }
    static private List<BotAction> ConvertFromDataToBotAction(List<BotActionData> actionDataList)
    {
        List<BotAction> actionList = new List<BotAction>();
        foreach (BotActionData actionData in actionDataList)
            actionList.Add(actionData.ConvertToBotAction());
        return actionList;
    }
    static public void SaveBotAsJson(List<BotAction>actionList)
    {
        List<BotActionData> actionDataList = ConvertFromBotActionToData(actionList);
        ActionsKeeper actionKeeper = new ActionsKeeper(actionDataList);
        string jsonMessage = JsonUtility.ToJson(actionKeeper, true);
        Debug.Log(jsonMessage);
        string jsonPath = Application.persistentDataPath + "/savedActions.txt";

        File.WriteAllText(jsonPath, jsonMessage);
        Debug.Log("saved json at " + jsonPath);
    }
    static public List<BotAction> LoadBotActions()
    {
        string jsonPath = Application.persistentDataPath + "/savedActions.txt";
        string jsonMessage = File.ReadAllText(jsonPath);
        if (File.Exists(jsonPath)){
            ActionsKeeper actionKeeper = JsonUtility.FromJson<ActionsKeeper>(jsonMessage);
            return ConvertFromDataToBotAction(actionKeeper.actionDataList);
        }
        else
        {
            Debug.LogError("Nu exista jsonPath " + jsonPath);
            return null;
        }
    }
}

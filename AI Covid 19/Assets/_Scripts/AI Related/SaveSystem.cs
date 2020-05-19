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
        public List<BotAction> actionList;
        public ActionsKeeper()
        {

        }
        public ActionsKeeper(List<BotAction> actions)
        {
            actionList = actions;
        }
    }
    static public void SaveBotAsJson(List<BotAction>actionArray)
    {
        ActionsKeeper actionKeeper = new ActionsKeeper(actionArray);
        string jsonMessage = JsonUtility.ToJson(actionKeeper, true);
        string jsonPath = Application.persistentDataPath + "/savedActions.txt";

        //Regex rx = new Regex(".probability.+,\n");
        //jsonMessage = Regex.Replace(jsonMessage, ".probability.+,", "");
        //Debug.Log("json is " + jsonMessage);
       
        File.WriteAllText(jsonPath, jsonMessage);
        Debug.Log("saved json at " + jsonPath);
    }
    static public List<BotAction> LoadBotActions()
    {
        string jsonPath = Application.persistentDataPath + "/savedActions.txt";
        string jsonMessage = File.ReadAllText(jsonPath);
        if (File.Exists(jsonPath)){
            ActionsKeeper actionKeeper = JsonUtility.FromJson<ActionsKeeper>(jsonMessage);
            return actionKeeper.actionList;
        }
        else
        {
            Debug.LogError("Nu exista jsonPath " + jsonPath);
            return null;
        }
    }
}

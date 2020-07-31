using System;
using System.Collections.Generic;
using System.IO;
using Covid19.AI.Depreceated.Actions;
using UnityEngine;

namespace Covid19.AI.Depreceated.SaveData
{
    public class SaveSystem
    {
        [Serializable]
        private class ActionsKeeper
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

        public static string GetJsonStringForBotActions()
        {
            var jsonPath = Application.persistentDataPath + "/savedActions.txt";
            var content = File.ReadAllText(jsonPath);
            return content;
        }

        public static void WriteIntroJsonForBotActions(string content)
        {
            var jsonPath = Application.persistentDataPath + "/savedActions.txt";
            File.WriteAllText(jsonPath, content);
        }

        private static List<BotActionData> ConvertFromBotActionToData(List<BotAction> actionList)
        {
            var dataList = new List<BotActionData>();
            foreach (var action in actionList)
                dataList.Add(new BotActionData(action));
            return dataList;
        }

        private static List<BotAction> ConvertFromDataToBotAction(List<BotActionData> actionDataList)
        {
            var actionList = new List<BotAction>();
            foreach (var actionData in actionDataList)
                actionList.Add(actionData.ConvertToBotAction());
            return actionList;
        }

        public static void SaveBotAsJson(List<BotAction> actionList)
        {
            var actionDataList = ConvertFromBotActionToData(actionList);
            var actionKeeper = new ActionsKeeper(actionDataList);
            var jsonMessage = JsonUtility.ToJson(actionKeeper, true);
            Debug.Log(jsonMessage);
            var jsonPath = Application.persistentDataPath + "/savedActions.txt";

            File.WriteAllText(jsonPath, jsonMessage);
            Debug.Log("saved json at " + jsonPath);
        }

        public static List<BotAction> LoadBotActions()
        {
            var jsonPath = Application.persistentDataPath + "/savedActions.txt";
            var jsonMessage = File.ReadAllText(jsonPath);
            if (File.Exists(jsonPath))
            {
                var actionKeeper = JsonUtility.FromJson<ActionsKeeper>(jsonMessage);
                return ConvertFromDataToBotAction(actionKeeper.actionDataList);
            }
            else
            {
                Debug.LogError("Nu exista jsonPath " + jsonPath);
                return null;
            }    
        }
    }
}
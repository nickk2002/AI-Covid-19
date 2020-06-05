using Covid19.SaveData;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using Covid19.AIBehaviour;

namespace Tests
{
    public class BotEditorTest
    {
        [Test]
        public void BotTestUpdatePreset()
        {
            GameObject test = new GameObject();
            Bot bot = test.AddComponent<Bot>();

            bot.UpdatePreset();
            Debug.Assert(SaveSystem.LoadBotActions().SequenceEqual(bot.reflectionActions));
        }
        [Test]
        public void BotTestUpdateModifications()
        {
            // initialize
            GameObject test = new GameObject();
            Debug.Log("entered test");
            Bot bot = test.AddComponent<Bot>();
            bot.UpdatePreset(); // initialize the actions with the JSON file

            // store the data
            string jsonCurrentContent = SaveSystem.GetJsonStringForBotActions();
            //act modifing a random value
            bot.reflectionActions[0].probability = 1000; // modify a value
            bot.SavePreset(); // save the preset
            bot.UpdateAllOthers(); // update all others 

            Debug.Log($"There are : {UnityEngine.Object.FindObjectsOfType<Bot>().Count()}");
            // assert that they are all the same
            foreach (Bot otherBot in UnityEngine.Object.FindObjectsOfType<Bot>())
            {
                Debug.Assert(otherBot.reflectionActions.SequenceEqual(bot.reflectionActions));
            }
            // revert
            SaveSystem.WriteIntroJsonForBotActions(jsonCurrentContent);
        }
        [Test]
        public void ResetBotScript()
        {
            GameObject gameObject = new GameObject();
            Bot bot = gameObject.AddComponent<Bot>();
            UnityEngine.Object.DestroyImmediate(bot);
            bot = gameObject.AddComponent<Bot>();
            bot.UpdatePreset();
        }
    }
}

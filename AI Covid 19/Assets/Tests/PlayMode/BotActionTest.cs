using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class BotActionTest
    {
        [UnityTest]
        public IEnumerator CheckIfVarsNotResetAfterPlayMode()
        {
            yield return new WaitForSeconds(0.25f);
            SceneManager.LoadScene("BasicScene");
            yield return new WaitForSeconds(0.25f);
            Bot randomBot = UnityEngine.Object.FindObjectsOfType<Bot>()[0];
            //store the data
            string jsonContent = SaveSystem.GetJsonStringForBotActions();
            randomBot.washHandsAction.position.x += 0.5f;
            randomBot.SavePreset();
            randomBot.UpdatePreset();
            
            foreach (Bot otherBot in UnityEngine.Object.FindObjectsOfType<Bot>())
            {
                Debug.Assert(otherBot.reflectionActions.SequenceEqual(randomBot.reflectionActions));
            }
            SaveSystem.WriteIntroJsonForBotActions(jsonContent);
        }
    }
}

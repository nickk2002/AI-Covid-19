using System.Collections;
using System.Linq;
using Covid19.AIBehaviour;
using Covid19.AIBehaviour.SaveData;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
            Bot randomBot = Object.FindObjectsOfType<Bot>()[0];
            //store the data
            string jsonContent = SaveSystem.GetJsonStringForBotActions();
            randomBot.washHandsAction.position.x += 0.5f;
            randomBot.SavePreset();
            randomBot.UpdatePreset();

            foreach (Bot otherBot in Object.FindObjectsOfType<Bot>())
            {
                Debug.Assert(otherBot.reflectionActions.SequenceEqual(randomBot.reflectionActions));
            }
            SaveSystem.WriteIntroJsonForBotActions(jsonContent);
        }
    }
}

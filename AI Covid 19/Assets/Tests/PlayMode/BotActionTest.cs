using Covid19.SaveData;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Covid19.AIBehaviour;

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
            randomBot._washHandsAction.position.x += 0.5f;
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

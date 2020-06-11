using System.Collections;
using System.Linq;
using Covid19.AIBehaviour;
using Covid19.AIBehaviour.SaveData;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class BotActionTest
    {
        private Vector3 _washHandsPosition;
        private Bot _randomBot;
        [SetUp]
        public void Setup()
        {
            //arrange

            // modify something
        }

        [UnityTest]
        public IEnumerator CheckIfVarsSaveWhenChanging()
        {
            SceneManager.LoadScene("_Scenes/BasicScene");
            yield return null;
            _randomBot = Object.FindObjectsOfType<Bot>()[0];
            _washHandsPosition = _randomBot.washHandsAction.position;
            _randomBot.washHandsAction.position.x += 0.5f;
            
            Debug.Log("entered the test bro!");
            yield return new WaitForSeconds(0.25f);
            Assert.AreNotEqual(_washHandsPosition,_randomBot.washHandsAction.position);
        }
    }
}
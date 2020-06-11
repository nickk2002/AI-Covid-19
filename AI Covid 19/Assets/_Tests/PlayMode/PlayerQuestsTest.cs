using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Covid19.GameManagers.UI_Manager;
using Covid19.Player;
using Covid19.Player.Quests;
using Covid19.UI.Quests;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace Tests
{
    [TestFixture]
    public class PlayerQuestsTest
    {
        private Room _room;
        private List<Quest> _questList;
        private List<ActionTextUI> _textList;
        private ActionsManagerUI _actionsManagerUI;
        private static bool initialized = false;

        [UnitySetUp] 
        public IEnumerator SetUpScene()
        {
            
                Debug.Log("Loading the Scene");
                SceneManager.LoadScene("_Scenes/BasicScene");
                yield return new WaitForSeconds(1.5f);

                // arrange stuff that are needed in all the tests
                _room = Object.FindObjectOfType<Room>();
                _questList = _room.GetComponentsInChildren<Quest>().ToList();
                _actionsManagerUI = UIManager.Instance.actionsManagerUI;
                _actionsManagerUI.Show(_questList);
                _textList = _actionsManagerUI.Quests;
                _actionsManagerUI.UnShow();
            
        }
 

        [UnityTest]
        public IEnumerator RemoveAllQuestExceptionCheck()
        {
            //act
            foreach (Quest quest in _questList)
            {
                _room.RemoveQuest(quest);
                yield return new WaitForEndOfFrame();
            }
            // assert that nothing blows 
        }

        [UnityTest]
        public IEnumerator CheckWhetherUIRemovedAfterCollectingQuest()
        {
            // arrange
            _actionsManagerUI.Show(_questList);
            
            // act
            foreach (Quest quest in _questList)
            {
                _room.RemoveQuest(quest);
                yield return new WaitForEndOfFrame();
                
                // assert
                var textUI = _textList.Find(ui => ui.QuestOwner == quest);
                Assert.NotNull(textUI); // check if it actually exists such text
                Assert.AreEqual(textUI.gameObject.activeSelf, false); // assert that the text is disabled in the canvas
                
                yield return new WaitForEndOfFrame();
            }
        }
        [UnityTest]
        public IEnumerator UIAppearingWhenPlayerEnteredTheRoom()
        {
            _actionsManagerUI.Show(_questList);
            yield return null;
            Assert.Greater(_textList.Count,0);
        }
        [UnityTest]
        public IEnumerator UIDissappearingAfterExitingTheRoom()
        {

            
            //UIManager.

            // Player.Instance.JumpAt(initialPosition); // jump all the way up
            // Debug.Log("the player jumped at position " + position);
            // yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(5f);

            //Assert.AreEqual(_actionsManagerUI.gameObject.activeSelf,false);
            
            
        }
        
    }
    
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Covid19.Core.Quests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Covid19.UI.Quests
{
    public class ActionsManagerUI : MonoBehaviour
    {
        public List<ActionTextUI> Quests => _questDictionary.Values.ToList();
        public GameObject actionPrefabUI;
        private readonly Dictionary<Quest, ActionTextUI> _questDictionary = new Dictionary<Quest, ActionTextUI>();
        private List<GameObject> _prefabs;
        
        
        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void InstantiateNewText(Quest quest)
        {
            GameObject textPrefab = Instantiate(actionPrefabUI, transform);
            var actionTextUI = textPrefab.GetComponent<ActionTextUI>();
            if (actionTextUI == null)
            {
                Debug.LogError("The prefab has no ActionTextUI Script attached to it!");
            }
            else
            {
                if (_questDictionary.ContainsKey(quest))
                    Debug.LogError("A quest has been added twice to the dictionary");
                _questDictionary.Add(quest, actionTextUI);
                StartCoroutine(AnimateText(textPrefab));
                actionTextUI.QuestOwner = quest;
                actionTextUI.Text = quest.QuestName + "   " + _questDictionary.Count;
                actionTextUI.gameObject.name = actionTextUI.Text;
            }
        }

        public void Show(List<Quest> questList)
        {
            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);
            if (questList.Count > 0)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
            foreach (Quest quest in questList)
            {
                if (_questDictionary.ContainsKey(quest))
                {
                    continue;
                }

                InstantiateNewText(quest);
            }

            foreach (Quest quest in _questDictionary.Keys)
            {
                // if the quest was removed , then we have to destroy the gameobject
                if (!questList.Contains(quest))
                {
                    Debug.Log($"The quest name is {quest.name} and the UI is now deleted");
                    var actionTextUI = _questDictionary[quest];
                    actionTextUI.gameObject.SetActive(false);
                }
            }
        }

        public void UnShow()
        {
            gameObject.SetActive(false);
        }

        public void Select(Quest quest)
        {
            if (_questDictionary.ContainsKey(quest))
            {
                var actionTextUI = _questDictionary[quest];
                actionTextUI.Select();
            }
            else
            {
                Debug.LogError("Hilight error.The quest name does not exist, Code problem most probably");
            }
        }

        public void Deselect(Quest quest)
        {
            if (_questDictionary.ContainsKey(quest))
            {
                var actionTextUI = _questDictionary[quest];
                actionTextUI.Deselect();
            }
            else
            {
                Debug.LogError("Hilight error.The quest name does not exist, Code problem most probably");
            }
        }

        private IEnumerator AnimateText(GameObject prefab)
        {
            prefab.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            prefab.SetActive(true);
        }
    }
}
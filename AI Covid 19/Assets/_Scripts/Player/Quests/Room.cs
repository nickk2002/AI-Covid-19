using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Covid19.GameManagers.UI_Manager;
using Covid19.UI.Quests;
using UnityEngine;

namespace Covid19.Player.Quests
{
    [RequireComponent(typeof(BoxCollider))]
    public class Room : MonoBehaviour
    {
         
        [SerializeField] private List<Quest> playerQuests;
        [SerializeField] private bool detectFromChildren = true;
        private ActionsManagerUI _actionManagerUI;


        private Quest _selectedQuest;
        private Quest _lastSelectedQuest;
        private bool _playerEntered = false;
        
        private void Start()
        {
            // make sure the trigger is set on
            GetComponent<BoxCollider>().isTrigger = true;
            _actionManagerUI = UIManager.Instance.actionsManagerUI;

            if (detectFromChildren)
                playerQuests = GetComponentsInChildren<Quest>().ToList();

            foreach (Quest quest in playerQuests)
            {
                quest.Owner = this;
            }
        }
        
        public void RemoveQuest(Quest quest)
        {
            Debug.Log($"removing quest {quest.name}");
            
            playerQuests.Remove(quest);

            _actionManagerUI.Show(playerQuests);
            quest.gameObject.SetActive(false);
        }
        private Quest GetSelectedAction()
        {
            int firstNumber = (int)KeyCode.Alpha1;
            for (int i = 0; i < playerQuests.Count; i++)
            {
                if (Input.GetKeyDown((KeyCode)(firstNumber + i))) {
                    return playerQuests[i];
                }
            }
            return null;

        }
        private void ManageSelection()
        {
            Quest currentQuest = GetSelectedAction();
            if (currentQuest != null)
            {
                Debug.Log("A selection was made");
                _lastSelectedQuest = _selectedQuest;
                _selectedQuest = currentQuest;
                if (_lastSelectedQuest == null)
                {
                    _actionManagerUI.Select(_selectedQuest);
                }
                else
                {
                    _actionManagerUI.Select(_lastSelectedQuest);
                    _actionManagerUI.Deselect(_selectedQuest);
                }
                   
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            GameObject obj = other.gameObject;
            if (obj.GetComponent<Player>() != null)
            {
                Debug.Log("Player entered the room");
                _playerEntered = true;
                _actionManagerUI.Show(playerQuests);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            GameObject obj = other.gameObject;
            if (obj.GetComponent<Player>() != null)
            {
                Debug.Log("player is staying in the room");
            }
            
        }
        private void OnTriggerExit(Collider other)
        {
            GameObject obj = other.gameObject;
            if (obj.GetComponent<Player>() != null)
            {
                _actionManagerUI.UnShow();
                Debug.Log("player exited the room");
                _playerEntered = false;
            }

        }
        private void Update()
        {
            if(_playerEntered)
                ManageSelection();
        
        }
    }
}

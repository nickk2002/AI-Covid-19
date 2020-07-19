using System.Collections.Generic;
using UnityEngine;

namespace Covid19.Core.Quests
{
    [RequireComponent(typeof(BoxCollider))]
    public class Quest : MonoBehaviour
    {
        [SerializeField] private string questName;
        [SerializeField] private List<QuestRequirement> questRequirements;
        public Room Owner { get; set; }
        public string QuestName => questName;
        


        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            if (questName.Length == 0)
                questName = name;
            gameObject.name = questName;
        }


        bool CheckRequirements(Player player)
        {
            foreach(QuestRequirement requirement in questRequirements)
            {
                if (player.HasRequirement(requirement) == false)
                    return false;
            }
            return true;
        }
        private void OnTriggerEnter(Collider other)
        {
            Player player = other.gameObject.GetComponent<Player>();
            if(player != null)
            {
                if (Owner == null)
                {
                    Debug.LogError("You attached a Quest script and did not put it into the room list");
                    return;
                }
                if (CheckRequirements(player) == false)
                {
                    print("Not all requirements are satisfied!");
                }
                else
                {
                    print("Quest Completed");
                    Owner.RemoveQuest(this);
                    
                }
            }
        }
    }
}

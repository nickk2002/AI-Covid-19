using UnityEngine;

namespace Covid19.Core.Quests {
    public class QuestRequirement : MonoBehaviour
    {
        
        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;            
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player>() != null)
            {
                Player player = other.GetComponent<Player>();
                player.TakeQuestRequirement(this);
                Debug.Log("player has taken the object");
                gameObject.SetActive(false);
            }
        }
    }
}

using UnityEngine;
using Covid19.AI.Behaviour.Configuration;

namespace Covid19.AI.Behaviour
{
    public class AddToList : MonoBehaviour
    {
        [SerializeField] private AgentNPCList gameObjectList;
        private AgentNPC _npc;

        private void OnEnable()
        {
            _npc = GetComponent<AgentNPC>();
            gameObjectList.Add(_npc);
        }
        private void OnDisable()
        {
            gameObjectList.Remove(_npc);
        }

        private void OnDestroy()
        {
            gameObjectList.Remove(_npc);
        }
        
    }
}
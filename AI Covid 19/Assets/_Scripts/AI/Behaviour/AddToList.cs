using UnityEngine;
using Covid19.AI.Behaviour.Configuration;

namespace Covid19.AI.Behaviour
{
    public class AddToList : MonoBehaviour
    {
        [SerializeField] private MonoBehaviourList gameObjectList;
        [SerializeField] public MonoBehaviour componentToAdd;

        private void OnEnable()
        {
            //_npc = GetComponent<AgentNPC>();
            gameObjectList.Add(componentToAdd);
        }
        private void OnDisable()
        {
            gameObjectList.Remove(componentToAdd);
        }

        private void OnDestroy()
        {
            gameObjectList.Remove(componentToAdd);
        }
        
    }
}
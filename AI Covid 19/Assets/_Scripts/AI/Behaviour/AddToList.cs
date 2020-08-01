using UnityEngine;
using Covid19.AI.Behaviour.Configuration;

namespace Covid19.AI.Behaviour
{
    public class AddToList : MonoBehaviour
    {
        [SerializeField] private MonoBehaviourList list;
        [SerializeField] private MonoBehaviour componentToAdd;

        private void OnEnable()
        {
            list.Add(componentToAdd);
        }
        private void OnDisable()
        {
            list.Remove(componentToAdd);
        }

        private void OnDestroy()
        {
            list.Remove(componentToAdd);
        }
        
    }
}
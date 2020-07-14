using System.Collections;
using Covid19.AIBehaviour.Behaviour;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class EatBeahaviour : MonoBehaviour,IBehaviour
    {
        public void Enable()
        {
            Debug.Log("Acum mananci");
        }

        public void Disable()
        {
            
        }

        public IEnumerator OnUpdate()
        {
            yield return null;
        }
    }
}
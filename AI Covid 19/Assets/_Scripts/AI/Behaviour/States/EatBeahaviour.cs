using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class EatBeahaviour : MonoBehaviour,IBehaviour
    {
        public void WakeUp()
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
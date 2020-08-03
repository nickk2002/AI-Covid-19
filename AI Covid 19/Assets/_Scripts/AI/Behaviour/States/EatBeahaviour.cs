using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class EatBeahaviour : MonoBehaviour, IBehaviour
    {
        public void Entry()
        {
            Debug.Log("Acum mananci");
        }

        public void Exit()
        {
        }

        public IEnumerator OnUpdate()
        {
            yield return null;
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class TestCoroutines : MonoBehaviour
    {
        private IEnumerator MyCoroutine()
        {
            while (true)
            {
                Debug.Log("The coroutine shows before yield");
                yield return null;
                Debug.Log("The coroutine shows after yield");
            }
        }

        void StopCoroutines()
        {
            StopAllCoroutines();
        }

        void Start()
        {
        }
        
        
    }
}
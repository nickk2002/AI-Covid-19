using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class TestCoroutines : MonoBehaviour
    {
        private IEnumerator MyCoroutine()
        {
            yield return StartCoroutine(WaitCoroutine());
            Debug.Log("Stopped myCoroutine");
        }

        private IEnumerator WaitCoroutine()
        {
            yield return new WaitForSeconds(2);
            Debug.Log("waited 2 seconds");
            yield return new WaitForSeconds(1f); 
            Debug.Log("waited 1 more seconds");
            while (true)
            {
                if (Time.time > 4)
                {
                    break;
                }

                yield return null;
            }
            Debug.Log("waited 1 more seconds");
            yield return StartCoroutine(AnotherCoroutine());
            yield return new WaitForSeconds(1f);
            Debug.Log("stopped wait coroutine");
        }

        private IEnumerator AnotherCoroutine()
        {
            Debug.Log("enter another coroutine");
            yield return new WaitForSeconds(3);
            Debug.Log("The another coroutine finished");
        }
        
        void Start()
        {
            Time.timeScale = 0;
            Debug.Log("Started Wait Coroutine");
            StartMyCoroutine(WaitCoroutine());
        }

        void StartMyCoroutine(IEnumerator waitCoroutine)
        {
            IEnumerator current = waitCoroutine;
            int iterations = 0;
            while (current.MoveNext())
            {
                Debug.Log(current);
                iterations++;
                if (iterations > 10)
                    return;
            }
        }
        
        
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Covid19.AI.Behaviour.States
{
    public class GoToLocationBehaviour : MonoBehaviour,IBehaviour
    {
        public float stopDistance;
        public Vector3 destination;
        private AgentNPC _npc;
        public void WakeUp()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
            _npc.Agent.SetDestination(destination);
            if (stopDistance == 0)
                stopDistance = 0.1f;
        }

        public override string ToString()
        {
            return "Going Somewhere";
        }

        public void Disable()
        {
            
        }

        public IEnumerator OnUpdate(){
            
            
            while (true)
            {
                if (Vector3.SqrMagnitude(destination - transform.position) < stopDistance * stopDistance)
                {
                    yield return null;
                    _npc.BehaviourSystem.RemoveBehaviour(this);
                }
                yield return null;
            }    
        }
    }
}
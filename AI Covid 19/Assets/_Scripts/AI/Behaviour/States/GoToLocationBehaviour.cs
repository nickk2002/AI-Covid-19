using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class GoToLocationBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;
        public Vector3 destination;
        public float stopDistance;
        public float remainingDistance;

        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
            _npc.Agent.SetDestination(destination);
            if (stopDistance == 0)
                stopDistance = 0.1f;
        }

        public void Exit()
        {
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                // TODO Check if the position is reachable
                remainingDistance = Vector3.Distance(destination, transform.position);
                if (Vector3.SqrMagnitude(destination - transform.position) < stopDistance * stopDistance)
                {
                    yield return null;
                    _npc.BehaviourSystem.RemoveBehaviour(this);
                }

                yield return null;
            }
        }

        public override string ToString()
        {
            return "Going Somewhere";
        }
    }
}
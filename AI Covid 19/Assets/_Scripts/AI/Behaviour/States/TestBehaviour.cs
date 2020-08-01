using System.Collections;
using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class TestBehaviour : MonoBehaviour,IBehaviour
    {
        private AgentNPC _npc;
        public void WakeUp()
        {
            _npc = GetComponent<AgentNPC>();
            Vector3 position = transform.position + transform.forward * 3;
            var goToLocationBehaviour = gameObject.AddComponent<GoToLocationBehaviour>();
            goToLocationBehaviour.destination = position;
            _npc.BehaviourSystem.SetBehaviour(goToLocationBehaviour,TransitionType.StackTransition);
            // var idleBeahaviour = gameObject.AddComponent<IdleBehaviour>();
            // _npc.BehaviourSystem.SetBehaviour(idleBeahaviour,TransitionType.StackTransition);
        }

        public void Disable()
        {
            
        }

        override public string ToString()
        {
            return "Test";
        }

        public IEnumerator OnUpdate()
        {
            yield return new WaitForSeconds(3f);
            _npc.BehaviourSystem.RemoveBehaviour(this);
        }
    }
}
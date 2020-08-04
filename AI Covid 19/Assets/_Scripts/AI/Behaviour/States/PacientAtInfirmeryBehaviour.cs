using System.Collections;
using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class PacientAtInfirmeryBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;
        public Transform destination;

        // this behaviuor is entered only if the Infirmery has available space
        // TODO : Make a good safety check on this
        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
            Debug.Assert(destination != null, "destination != null");
            
            var goToLocation = gameObject.AddComponent<GoToLocationBehaviour>();
            goToLocation.destination = destination.position;
            goToLocation.stopDistance = 2f;
            _npc.BehaviourSystem.SetBehaviour(goToLocation,TransitionType.EntryTransition);
        }

        public void Exit()
        {
            _npc.Agent.isStopped = false;
        }

        public IEnumerator OnUpdate()
        {
            yield return null;
            _npc.Agent.isStopped = true;
            _npc.InfectionSystem.CallDoctor();

            while (true)
            {
                if (_npc.InfectionSystem.Cured)
                {
                    Debug.Log($"{_npc} Now I am healed! The bed is free");
                    _npc.InfectionSystem.FreeBed();
                    _npc.BehaviourSystem.RemoveBehaviour(this);
                }
                yield return null;
            }
        }

        public override string ToString()
        {
            return "Being Healed behaviour";
        }
    }
}
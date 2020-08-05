using System.Collections;
using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    [DisallowMultipleComponent]
    public class InfectionCheckBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;

        public float investigationDuration;
        public Vector3 meetingPosition;
        public AgentNPC partnerNPC;

        public void Entry()
        {
            if (gameObject.GetComponents<InfectionCheckBehaviour>().Length > 1)
                Debug.LogError("There are more than one InfectionCheckBehaviours");
            _npc = GetComponent<AgentNPC>();

            var goToLocation = gameObject.AddComponent<GoToLocationBehaviour>();
            goToLocation.destination = meetingPosition;
            goToLocation.LocationName = "Investigate Point";
            _npc.BehaviourSystem.SetBehaviour(goToLocation,TransitionType.EntryTransition);
        }

        public void Exit()
        {
            _npc.Agent.isStopped = false;
        }

        public IEnumerator OnUpdate()
        {
            _npc.Agent.isStopped = true;
            yield return new WaitForSeconds(investigationDuration);

            if (_npc.agentConfig.agentType != AgentType.Doctor)
            {
                PacientAtInfirmeryBehaviour behaviour = _npc.gameObject.AddComponent<PacientAtInfirmeryBehaviour>();
                
                behaviour.destination = _npc.InfectionSystem.Infirmery.GetBedPosition(_npc); 
                _npc.BehaviourSystem.SetBehaviour(behaviour, TransitionType.OverrideTransition);
            }
            else
            {
                _npc.BehaviourSystem.RemoveBehaviour(this); // daca este doctor atunci opresc direct si gata
            }
        }

        public override string ToString()
        {
            return "CheckIllness";
        }
    }
}
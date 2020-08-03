using System.Collections;
using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    [DisallowMultipleComponent]
    public class InfectionCheckBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;

        private bool _reached = false;
        public float investigationDuration;
        public Vector3 meetingPosition;
        public AgentNPC partnerNPC;

        public void Entry()
        {
            if (gameObject.GetComponents<InfectionCheckBehaviour>().Length > 1)
                Debug.LogError("There are more than one InfectionCheckBehaviours");
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
            _npc.Agent.SetDestination(meetingPosition);
        }

        public void Exit()
        {
            Debug.Log("agent is ok");
            _npc.Agent.isStopped = false;
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (_npc.Agent.pathPending)
                    yield return null;
                if (_npc.Agent.remainingDistance < 0.1f && _reached == false)
                {
                    _reached = true;
                    _npc.Agent.isStopped = true;
                    yield return new WaitForSeconds(investigationDuration);

                    if (_npc.agentConfig.agentType != AgentType.Doctor && _npc.InfectionSystem.InfectionLevel > 0)
                    {
                        GoToInfirmeryBehaviour behaviour = _npc.gameObject.AddComponent<GoToInfirmeryBehaviour>();
                        behaviour.destination = FindObjectOfType<Infirmery>().GetBedPosition(_npc);
                        _npc.BehaviourSystem.SetBehaviour(behaviour, TransitionType.OverrideTransition);
                    }
                    else
                    {
                        _npc.BehaviourSystem.RemoveBehaviour(this); // daca este doctor atunci opresc direct si gata
                    }
                }

                yield return null;
            }
        }

        public override string ToString()
        {
            return "CheckIllness";
        }
    }
}
using System.Collections;
using Covid19.AIBehaviour.Behaviour.States;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class InfectionSystem
    {
        private static readonly int Cough = Animator.StringToHash("cough");
        private readonly Animator _animator;
        private readonly AgentNPC _npc;
        private bool _goingToInfirmery = false;
        private bool _infected = false;
        private float _infectionLevel = 0;

        public bool cured = false;

        public InfectionSystem(AgentNPC owner)
        {
            _npc = owner;
            _animator = owner.GetComponent<Animator>();
        }

        public void StartInfection()
        {
            if (_infected || cured) return;
            Debug.Log("Heii I am infected" + _npc.name);
            _infected = true;
            _npc.StartCoroutine(InfectionHandler());
            _npc.StartCoroutine(InfectingOthersHandler());
        }

        private void InfectNearbyAgents()
        {
            foreach (AgentNPC agentNPC in AgentManager.Instance.agentNPCList)
                if (Vector3.Distance(_npc.transform.position, agentNPC.transform.position) <
                    AgentManager.Instance.generalConfiguration.infectionDistance)
                    agentNPC.StartInfection();
        }

        private IEnumerator InfectingOthersHandler()
        {
            while (true)
            {
                if (cured)
                    break;
                var coughVal =
                    AIManager.Instance.coughCurve
                        .Evaluate(_infectionLevel); // evaluate from gamemanager cough interval over infection function
                var coughInterval = Random.Range(coughVal - 1, coughVal); // add a bit of random 
                yield return new WaitForSeconds(coughInterval);
                if (_animator.GetBool(Cough) == false)
                {
                    _animator.SetTrigger(Cough);
                    InfectNearbyAgents();
                }
            }
        }

        private IEnumerator InfectionHandler()
        {
            while (true)
            {
                yield return new WaitForSeconds(AgentManager.Instance.generalConfiguration.growthInterval);
                _infectionLevel += AgentManager.Instance.generalConfiguration.growthInterval;
                if (cured)
                    break;
                if (_infectionLevel > 1 && _goingToInfirmery == false)
                {
                    _npc.SetBehaviour(_npc.gameObject.AddComponent<InfirmeryBehaviour>());
                    _goingToInfirmery = true;
                }
            }
        }
    }
}
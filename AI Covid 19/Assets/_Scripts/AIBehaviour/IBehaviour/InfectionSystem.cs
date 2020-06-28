using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class InfectionSystem
    {
        private static readonly int Cough = Animator.StringToHash("cough");
        private readonly Animator _animator;
        private bool _infected = false;
        private float _infectionLevel = 0;
        private readonly AgentNPC _npc;

        public InfectionSystem(AgentNPC owner)
        {
            _npc = owner;
            _animator = owner.GetComponent<Animator>();
        }

        public void StartInfection()
        {
            if (_infected) return;
            Debug.Log("Heii I am infected" + _npc.name);
            _infected = true;
            _npc.StartCoroutine(InfectionHandler());
            _npc.StartCoroutine(InfectingOthersHandler());
        }

        private void InfectNearbyAgents()
        {
            foreach (AgentNPC agentNPC in NPCManager.Instance.agentNpcs)
                if (Vector3.Distance(_npc.transform.position, agentNPC.transform.position) < 10)
                    agentNPC.StartInfection();
        }

        private IEnumerator InfectingOthersHandler()
        {
            while (true)
            {
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
                yield return new WaitForSeconds(NPCManager.Instance.generalConfiguration.growthInterval);
                _infectionLevel += NPCManager.Instance.generalConfiguration.growthInterval;
            }
        }
    }
}
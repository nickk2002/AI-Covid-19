using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class GoToInfirmeryBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;
        private bool _reached = false;
        public Transform destination;

        // this behaviuor is entered only if the Infirmery has available space
        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
            Debug.Log("going to infirmery");
            Debug.Assert(destination != null, "destination != null");
            _npc.Agent.SetDestination(destination.position);
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                Debug.Log(Vector3.Distance(transform.position, destination.position));
                if (Vector3.Distance(transform.position, destination.position) < 2f)
                    if (_reached == false)
                    {
                        Debug.Log("Heii am ajuns la destinatie gata, oprirea!!!");
                        _npc.Agent.isStopped = true;
                        _npc.InfectionSystem.CallDoctor();
                        _reached = true;
                    }

                if (_npc.InfectionSystem.Cured)
                {
                    Debug.Log("Now I am healed! The bed is free");
                    _npc.InfectionSystem.FreeBed();
                    _npc.Agent.isStopped = false;
                    _npc.BehaviourSystem.RemoveBehaviour(this);
                }

                yield return null;
            }
        }

        public override string ToString()
        {
            return "Going To Infirmery";
        }
    }
}
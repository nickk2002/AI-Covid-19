using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class GoToInfirmeryBehaviour : MonoBehaviour, IBehaviour
    {
        public Transform destination;
        private AgentNPC _npc;
        private bool _reached = false;

        // this behaviuor is entered only if the Infirmery has available space
        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            
            Debug.Assert(destination != null,"destination != null");
            _npc.Agent.SetDestination(destination.position);
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (_npc.Agent.remainingDistance < 0.3f)
                    if (_reached == false)
                    {
                        Debug.Log("Heii am ajuns la destinatie gata, oprirea!!!");
                        _npc.Agent.isStopped = true;
                        Infirmery.Instance.CallDoctor(_npc);
                        _reached = true;
                    }

                if (_npc.InfectionSystem.Cured)
                {
                    Debug.Log("Now I am healed! The bed is free");
                    Infirmery.Instance.FreeBed(_npc);
                    _npc.Agent.isStopped = false;
                    _npc.RemoveBehaviour(this);
                    
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
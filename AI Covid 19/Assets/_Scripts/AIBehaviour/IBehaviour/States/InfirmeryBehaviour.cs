using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class InfirmeryBehaviour : MonoBehaviour, IBehaviour
    {
        private Vector3 _destination;
        private AgentNPC _npc;
        private bool _reached = false;

        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            _destination = Infirmery.Instance.target.transform.position;
            _npc.Agent.SetDestination(_destination);
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

                if (_npc.InfectionSystem.cured)
                {
                    _npc.RemoveBehaviour(this);
                    _npc.Agent.isStopped = false;
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
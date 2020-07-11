using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class InfirmeryBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;
        private Vector3 _destination;
        
        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            _destination = AgentManager.Instance.infirmery.target.transform.position;
            _npc.Agent.SetDestination(_destination);
        }

        public override string ToString()
        {
            return "Going To Infirmery";
        }

        public void Disable()
        {
            
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (Vector3.Distance(_npc.transform.position, _destination) < 0.1f)
                {
                    _npc.Agent.isStopped = true;
                }
                yield return null;
            }

        }
    }
}
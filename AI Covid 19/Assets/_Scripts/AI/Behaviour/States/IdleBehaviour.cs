#region

using System.Collections;
using UnityEngine;

#endregion

namespace Covid19.AI.Behaviour.States
{
    public class IdleBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;
        public void WakeUp()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = true;
        }

        public void Disable()
        {
            _npc.Agent.isStopped = false;
        }

        override public string ToString()
        {
            return "Idle";
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                _npc.BehaviourSystem.RemoveBehaviour(this);
                yield return null;
                Debug.Assert(this != null,"Idle is null and the coroutine still continues");
            }
        }
    }
}
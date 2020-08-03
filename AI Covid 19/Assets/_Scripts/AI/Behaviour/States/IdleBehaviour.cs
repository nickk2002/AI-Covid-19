#region

using System.Collections;
using UnityEngine;

#endregion

namespace Covid19.AI.Behaviour.States
{
    public class IdleBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;

        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = true;
        }

        public void Exit()
        {
            _npc.Agent.isStopped = false;
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                _npc.BehaviourSystem.RemoveBehaviour(this);
                yield return null;
                Debug.Assert(this != null, "Idle is null and the coroutine still continues");
            }
        }

        override public string ToString()
        {
            return "Idle";
        }
    }
}
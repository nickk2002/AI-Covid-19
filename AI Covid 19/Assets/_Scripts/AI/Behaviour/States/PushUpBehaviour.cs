using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class PushUpBehaviour : MonoBehaviour,IBehaviour
    {
        private AgentNPC _npc;
        [SerializeField] private int pushUpCount;
        [SerializeField] private int maxPushUps;
        private static readonly int ExitPushUp = Animator.StringToHash("exitPushUp");
        private static readonly int PushUp = Animator.StringToHash("pushUp");

        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
        }

        // event called at the end of the push up animation
        // when the body is up.
        public void BodyUp()
        {
            pushUpCount++;
            if (pushUpCount == maxPushUps)
            {
                _npc.BehaviourSystem.RemoveBehaviour(this);
            }
        }
        // event called at the end of the push up animation
        // when the body is down.
        public void EndPushUp()
        {
            if (pushUpCount == maxPushUps - 1) // o sa mai faca inca un push up ca sa se ridice
            {
                _npc.Animator.SetTrigger(ExitPushUp);
            }
        }
        public void Exit()
        {
        }

        public IEnumerator OnUpdate()
        {
            _npc.Animator.SetTrigger(PushUp);
            maxPushUps = Random.Range(2, 10);
            yield return null;
        }
    }
}
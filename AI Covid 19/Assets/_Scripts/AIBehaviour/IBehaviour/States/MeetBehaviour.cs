using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class MeetBehaviour : MonoBehaviour, IBehaviour
    {
        public Vector3 MeetPosition { set; private get; }
        private Animator _animator;
        private AgentNPC _npc;

        private static readonly int TalkingBool = Animator.StringToHash("talking");
        private static readonly int TalkID = Animator.StringToHash("talkID");

        public void Enable()
        {
            Debug.Log($"Entered meeting {name}");
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.SetDestination(MeetPosition);
            _animator = GetComponent<Animator>();
        }


        public void Disable()
        {

        }

        public IEnumerator OnUpdate()
        {    
            while (true)
            {
                Debug.Log($"In meeting {name}");
                if (_npc.Agent.remainingDistance < 0.1f)
                {
                    _npc.Agent.isStopped = true;
                    _animator.SetBool(TalkingBool, true);
                    _animator.SetInteger(TalkID,_npc.MeetSystem.TalkAnimationID);
                    yield return new WaitForSeconds(_npc.MeetSystem.TalkDuration);
                    Debug.Log($"Exiting meeting {name}");
                    
                    _npc.MeetSystem.LastMeetingTime = Time.time;
                    _npc.Agent.isStopped = false;
                    _animator.SetBool(TalkingBool, false);
                    _npc.RemoveBehaviour(this);
                }

                yield return null;
            }
        }
    }
}
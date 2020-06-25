using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class MeetBehaviour : MonoBehaviour, IBehaviour
    {
        public Vector3 MeetPosition { set; private get; }
        private Animator _animator;
        private AgentNPC _npc;
        public AgentNPC partnerNPC;

        private static readonly int MeetingBool = Animator.StringToHash("meeting");
        private static readonly int TalkingBool = Animator.StringToHash("talking");
        private static readonly int ListeningBool = Animator.StringToHash("listening");
        private static readonly int TalkID = Animator.StringToHash("talkID");
        private bool _reached = false;
        private float _startListeningTime = 0;

        public void Enable()
        {
            Debug.Log($"Entered meeting {name}");
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.SetDestination(MeetPosition);
            _animator = GetComponent<Animator>();

        }


        public void Disable()
        {
            _npc.MeetSystem.LastMeetingTime = Time.time;
            _npc.Agent.isStopped = false;
            _animator.SetBool(MeetingBool, false);
        }

        public IEnumerator OnUpdate()
        {    
            while (true)
            {
                if (_npc.Agent.remainingDistance < 0.1f)
                {
                    if (_reached == false)
                    {
                        _reached = true;
                        _npc.Agent.isStopped = true;
                        _animator.SetBool(MeetingBool, true);
                        if (partnerNPC.gameObject.GetComponent<Animator>().GetBool(TalkingBool) == false)
                            _animator.SetBool(TalkingBool, true);
                        else
                        {
                            _animator.SetBool(ListeningBool, true);
                            _startListeningTime = Time.time;
                        }
                    }
                }
                if(_reached == true){
                    if (_animator.GetBool(ListeningBool))
                    {
                        if (Time.time - _startListeningTime > 4f)
                        {
                            Debug.Log($"<color=red> Swith conversation {_npc.name} </color>");
                            _animator.SetBool(TalkingBool, true);
                            _animator.SetBool(ListeningBool, false);

                            partnerNPC.gameObject.GetComponent<Animator>().SetBool(TalkingBool, false);
                            partnerNPC.gameObject.GetComponent<Animator>().SetBool(ListeningBool, true);
                            partnerNPC.gameObject.GetComponent<MeetBehaviour>()._startListeningTime = Time.time;
                        }
                    }
                }
                
                yield return null;
            }
        }
    }
}
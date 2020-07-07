using System.Collections;
using Covid19.Utils;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class MeetBehaviour : MonoBehaviour, IBehaviour
    {
        private static readonly int MeetingBool = Animator.StringToHash("meeting");
        private static readonly int TalkingBool = Animator.StringToHash("talking");
        private static readonly int ListeningBool = Animator.StringToHash("listening");
        private static readonly int TalkID = Animator.StringToHash("talkID");
        private Animator _animator;

        private bool _drawGizmos = true;
        private GameObject _meetGizmos;
        private AgentNPC _npc;
        private bool _reached = false;
        private float _startListeningTime = 0;
        public AgentNPC partnerNPC;
        public Vector3 MeetPosition { set; private get; }

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
            if (_meetGizmos)
                Destroy(_meetGizmos);
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (_npc.Agent.remainingDistance < 0.1f)
                    if (_reached == false)
                    {
                        _reached = true;
                        _npc.Agent.isStopped = true;
                        _animator.SetBool(MeetingBool, true);
                        if (partnerNPC.gameObject.GetComponent<Animator>().GetBool(TalkingBool) == false)
                        {
                            _animator.SetBool(TalkingBool, true);
                        }
                        else
                        {
                            _animator.SetBool(ListeningBool, true);
                            _startListeningTime = Time.time;
                        }
                    }

                if (_reached)
                    if (_animator.GetBool(ListeningBool))
                        if (Time.time - _startListeningTime > 4f)
                        {
                            Debug.Log($"<color=red> Swith conversation {_npc.name} </color>");
                            _animator.SetBool(TalkingBool, true);
                            _animator.SetBool(ListeningBool, false);

                            partnerNPC.gameObject.GetComponent<Animator>().SetBool(TalkingBool, false);
                            partnerNPC.gameObject.GetComponent<Animator>().SetBool(ListeningBool, true);
                            partnerNPC.gameObject.GetComponent<MeetBehaviour>()._startListeningTime = Time.time;
                        }

                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            if (partnerNPC == null)
                return;
            if (_npc.IsCurrentBehaviour(this) && _drawGizmos && partnerNPC.GetComponent<MeetBehaviour>()._drawGizmos)
            {
                _meetGizmos = new GameObject {name = "MeetGizmos"};
                Vector3 otherMeetPosition = partnerNPC.GetComponent<MeetBehaviour>().MeetPosition;
                Vector3 meetGizmoPosition = MeetPosition + (otherMeetPosition - MeetPosition) / 2;
                meetGizmoPosition += Vector3.up * 0.5f;
                _meetGizmos.transform.position = meetGizmoPosition;
                CustomHierarchy.SetIcon(_meetGizmos, CustomHierarchy.MeetingPointIcon);
                _drawGizmos = false;
                partnerNPC.GetComponent<MeetBehaviour>()._drawGizmos = false;
            }
        }
    }
}
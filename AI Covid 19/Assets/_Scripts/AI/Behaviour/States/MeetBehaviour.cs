using System.Collections;
using Covid19.Utils;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
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

        public Vector3 meetPosition;

        public AgentNPC partnerNPC;
        public float talkDuration;

        public void Enable()
        {
            Debug.Log($"Entered meeting {name}");
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.SetDestination(meetPosition);
            _animator = GetComponent<Animator>();
        }


        public void Disable()
        {
            _npc.MeetSystem.LastMeetingTime = Time.time;
            _npc.Agent.isStopped = false;
            _animator.SetBool(MeetingBool, false);
            _animator.SetBool(TalkingBool, false);
            _animator.SetBool(ListeningBool, false);
            if (_meetGizmos)
                Destroy(_meetGizmos);
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (_npc.Agent.pathPending)
                    yield return null;
                if (_npc.Agent.remainingDistance < 0.1f && _reached == false)
                {
                    StartCoroutine(WaitUntilMeetingEnds());
                    _reached = true;
                    _npc.Agent.isStopped = true;
                    _animator.SetBool(MeetingBool, true);
                    if (partnerNPC.Animator.GetBool(TalkingBool) == false)
                    {
                        var randomTalkID = Random.Range(0, 3);
                        _animator.SetBool(TalkingBool, true);
                        _animator.SetInteger(TalkID, randomTalkID);
                    }
                    else
                    {
                        _animator.SetBool(ListeningBool, true);
                    }
                }

                if (_animator.GetBool(ListeningBool))
                {
                    float listeningDuration = Random.Range(4f, 10f); // random listening duration time;
                    yield return new WaitForSeconds(listeningDuration);
                    SwitchConversation();
                }

                yield return null;
            }
        }

        private void SwitchConversation()
        {
            Debug.Log($"<color=red> Swith conversation {_npc.name} </color>");
            var randomTalkID = Random.Range(0, 3);
            _animator.SetInteger(TalkID, randomTalkID);
            _animator.SetBool(TalkingBool, true);
            _animator.SetBool(ListeningBool, false);

            partnerNPC.Animator.SetBool(TalkingBool, false);
            partnerNPC.Animator.SetBool(ListeningBool, true);
        }

        public override string ToString()
        {
            return "Meet";
        }

        private IEnumerator WaitUntilMeetingEnds()
        {
            yield return new WaitForSeconds(talkDuration);
            _npc.BehaviourSystem.RemoveBehaviour(this);
        }

        private void OnDrawGizmos()
        {
            if (partnerNPC == null)
                return;
            if (!_npc.BehaviourSystem.IsCurrentBehaviour(typeof(MeetBehaviour)) || !_drawGizmos ||
                !partnerNPC.GetComponent<MeetBehaviour>()._drawGizmos)
                return;
            _meetGizmos = new GameObject {name = "MeetGizmos"};
            Vector3 otherMeetPosition = partnerNPC.GetComponent<MeetBehaviour>().meetPosition;
            Vector3 meetGizmosPosition = meetPosition + (otherMeetPosition - meetPosition) / 2;
            meetGizmosPosition += Vector3.up * 0.5f;
            _meetGizmos.transform.position = meetGizmosPosition;
            CustomHierarchy.SetIcon(_meetGizmos, CustomHierarchy.MeetingPointIcon);
            _drawGizmos = false;
            partnerNPC.GetComponent<MeetBehaviour>()._drawGizmos = false;
        }
    }
}
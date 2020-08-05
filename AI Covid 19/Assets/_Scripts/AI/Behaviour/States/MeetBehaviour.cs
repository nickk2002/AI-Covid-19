using System.Collections;
using Covid19.AI.Behaviour.Systems;
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

        private MeetBehaviour _partnerBehaviour;

        public Vector3 meetPosition;
        public AgentNPC partnerNPC;
        public float talkDuration;

        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
            _animator = GetComponent<Animator>();

            _partnerBehaviour = partnerNPC.GetComponent<MeetBehaviour>();

            var goToLocation = gameObject.AddComponent<GoToLocationBehaviour>();
            goToLocation.destination = meetPosition;
            goToLocation.LocationName = "Meeting Point";

            _npc.BehaviourSystem.SetBehaviour(goToLocation, TransitionType.EntryTransition);
        }


        public void Exit()
        {
        }

        public IEnumerator OnUpdate()
        {
            
            yield return null;

            StartCoroutine(WaitUntilMeetingEnds());
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

            while (true)
            {
                if (_animator.GetBool(ListeningBool))
                {
                    float listeningDuration = Random.Range(4f, 10f); // random listening duration time;
                    yield return new WaitForSeconds(listeningDuration);
                    SwitchConversation();
                }

                yield return null;
            }
        }

        private void EndMeeting()
        {
            Debug.Log("Meeting ended");
            _npc.MeetSystem.LastMeetingTime = Time.time;
            _npc.Agent.isStopped = false;
            _animator.SetBool(MeetingBool, false);
            _animator.SetBool(TalkingBool, false);
            _animator.SetBool(ListeningBool, false);
            if (_meetGizmos)
                Destroy(_meetGizmos);
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
            EndMeeting();
            _npc.BehaviourSystem.RemoveBehaviour(this);
        }

        private void OnDrawGizmos()
        {
            if (partnerNPC == null)
                return;

            if (_partnerBehaviour == null || !_npc.BehaviourSystem.IsCurrentBehaviour(typeof(MeetBehaviour)) || !_drawGizmos ||
                !_partnerBehaviour._drawGizmos)
                return;
            _meetGizmos = new GameObject {name = "MeetGizmos"};
            Vector3 otherMeetPosition = _partnerBehaviour.meetPosition;
            Vector3 meetGizmosPosition = meetPosition + (otherMeetPosition - meetPosition) / 2;
            meetGizmosPosition += Vector3.up * 0.5f;
            _meetGizmos.transform.position = meetGizmosPosition;
            CustomHierarchy.SetIcon(_meetGizmos, CustomHierarchy.MeetingPointIcon);
            _drawGizmos = false;
            _partnerBehaviour._drawGizmos = false;
        }
    }
}
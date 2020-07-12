using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class HealAgentBehaviour : MonoBehaviour, IBehaviour
    {
        private Vector3 _meetingPosition;
        private AgentNPC _npc;
        private bool _reachedFaceToFace = false;
        private bool _reachedMeeting = false;
        public Infirmery infirmery;
        public AgentNPC pacient;

        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            Transform pacientTransform = pacient.transform;
            _meetingPosition = pacientTransform.position + pacientTransform.forward * 2;
            Debug.Log($"enabling heal behaviour at position {_meetingPosition}");
            var go = new GameObject("Meeting Position For Doctor");
            go.transform.position = _meetingPosition;
            _npc.Agent.SetDestination(_meetingPosition);
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (Vector3.Distance(transform.position, _meetingPosition) < 0.1f)
                    if (_reachedMeeting == false)
                    {
                        Debug.Log("doctor reached");
                        _npc.Agent.isStopped = true;
                        _reachedMeeting = true;
                        StartCoroutine(RotateCoroutine(1f));
                    }

                if (_reachedFaceToFace)
                {
                    yield return new WaitForSeconds(2f);
                    pacient.InfectionSystem.cured = true;
                    _npc.RemoveBehaviour(this);
                }

                yield return null;
            }
        }

        public override string ToString()
        {
            return "Heal";
        }

        private IEnumerator RotateCoroutine(float duration)
        {
            Quaternion initialRotation = transform.rotation;
            Quaternion desiredRotation = pacient.transform.rotation * Quaternion.Euler(0, 180, 0);
            var initialTime = Time.time;

            while (Time.time - initialTime < duration)
            {
                transform.rotation =
                    Quaternion.Lerp(initialRotation, desiredRotation, (Time.time - initialTime) / duration);
                yield return null;
            }

            transform.rotation = desiredRotation;
            _reachedFaceToFace = true;
        }
    }
}
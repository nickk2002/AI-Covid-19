using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class HealAgentBehaviour : MonoBehaviour, IBehaviour
    {
        private Vector3 _meetingPosition;
        private AgentNPC _npc;

        public Infirmery infirmery;
        
        private readonly Stack<AgentNPC> _pacientsStack = new Stack<AgentNPC>();
    
        public void Enable()
        {    
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
            Debug.Log($"enabling heal behaviour at position {_meetingPosition}");
        }

        public void Disable()
        {
        }

        public void AddPacient(AgentNPC npc)
        {
            _pacientsStack.Push(npc);
        }
        
        private void PrepareDestination(AgentNPC pacient)
        {
            Transform pacientTransform = pacient.transform;
            _meetingPosition = pacientTransform.position + pacientTransform.forward * 2;
            _npc.Agent.SetDestination(_meetingPosition);
        }
        private IEnumerator HealPacient(AgentNPC pacient)
        {
            PrepareDestination(pacient);
            while (true)
            {
                if (_npc.Agent.pathPending)
                    yield return null;
                Debug.Log(Vector3.Distance(transform.position, _meetingPosition) + " " + _npc.Agent.remainingDistance);
                if (_npc.Agent.remainingDistance < 0.5f)
                {
                    Debug.Log($"doctor reached at {pacient.name}");
                    _npc.Agent.isStopped = true;
                    yield return StartCoroutine(RotateCoroutine(1f, pacient));
                    
                    // TODO: add heal animation
                    yield return new WaitForSeconds(2f);
                    
                    Debug.Log($"{pacient.name} is healed");
                    pacient.InfectionSystem.Cured = true;
                    _npc.Agent.isStopped = false;
                    break;
                }
                yield return null;
            }    
        }
        public IEnumerator OnUpdate()
        {
            while (_pacientsStack.Count > 0)
            {
                AgentNPC pacient = _pacientsStack.Peek();
                _pacientsStack.Pop();
                yield return StartCoroutine(HealPacient(pacient));
            }

            _npc.BehaviourSystem.RemoveBehaviour(this);
        }
        public override string ToString()
        {
            return "Heal";
        }

        private IEnumerator RotateCoroutine(float duration,AgentNPC pacient)
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
        }
    }
}
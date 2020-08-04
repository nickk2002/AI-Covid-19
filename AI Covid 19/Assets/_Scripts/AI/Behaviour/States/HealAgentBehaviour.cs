using System.Collections;
using System.Collections.Generic;
using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class HealAgentBehaviour : MonoBehaviour, IBehaviour
    {
        private readonly Stack<AgentNPC> _pacientsStack = new Stack<AgentNPC>();
        private AgentNPC _npc;

        public Infirmery infirmery;

        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
        }
        
        public void Exit()
        {
        }

        public IEnumerator OnUpdate()
        {
            Debug.Log("calling again the Heal Behaviour");
            yield return null;
            while (_pacientsStack.Count > 0)
            {
                AgentNPC pacient = _pacientsStack.Peek();
                _pacientsStack.Pop();
                yield return StartCoroutine(HealPacient(pacient));
            }

            Debug.Log("The heal is finished");
            _npc.BehaviourSystem.RemoveBehaviour(this);
        }

        public void AddPacient(AgentNPC npc)
        {
            _pacientsStack.Push(npc);
        }

        private void PrepareDestination(AgentNPC pacient)
        {
            Transform pacientTransform = pacient.transform;
            Vector3 meetingPosition = pacientTransform.position + pacientTransform.forward * 2; //TODO Meeting based on offset meeting

            var goToLocation = gameObject.AddComponent<GoToLocationBehaviour>();
            goToLocation.destination = meetingPosition;
            goToLocation.stopDistance = 1.5f;
            _npc.BehaviourSystem.SetBehaviour(goToLocation, TransitionType.StackTransition);
        }

        private IEnumerator HealPacient(AgentNPC pacient)
        {
            PrepareDestination(pacient);

            while (true)
            {
                if (_npc.BehaviourSystem.IsCurrentBehaviour(typeof(HealAgentBehaviour)))
                {
                    break;
                }

                yield return null;
            }

            Debug.Log($"doctor reached at {pacient.name}");
            _npc.Agent.isStopped = true;
            Debug.Log(Time.timeScale);
            yield return new WaitForSeconds(2f);
            Debug.Log("wait 2 seconds please");

            yield return StartCoroutine(RotateCoroutine(5f, pacient));
            // TODO: add heal animation
            yield return new WaitForSeconds(2f); // heal the pacient
            Debug.Log($"{pacient.name} is healed");
            pacient.InfectionSystem.Cured = true;
            _npc.Agent.isStopped = false;
            yield return null;
        }

        public override string ToString()
        {
            return "Heal";
        }

        private IEnumerator RotateCoroutine(float duration, AgentNPC pacient)
        {
            Quaternion initialRotation = transform.rotation;
            Quaternion desiredRotation = pacient.transform.rotation * Quaternion.Euler(0, 180, 0);
            var initialTime = Time.time;

            while (Time.time - initialTime < duration)
            {
                transform.rotation = Quaternion.Lerp(initialRotation, desiredRotation, (Time.time - initialTime) / duration);
                yield return null;
            }

            Debug.Log("Finished the rotation");

            transform.rotation = desiredRotation;
        }
    }
}
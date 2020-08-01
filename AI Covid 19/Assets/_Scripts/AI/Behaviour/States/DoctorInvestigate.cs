using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class DoctorInvestigate : MonoBehaviour, IBehaviour
    {
        private Vector3 _currentDestination = Vector3.negativeInfinity;
        private int _indexPatrol = 0;
        private AgentNPC _npc;
        private bool _startInvestigation = false;

        public void WakeUp()
        {
            _npc = GetComponent<AgentNPC>();
            SetUpPosHolder();
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (_npc.Agent.pathPending)
                    yield return null;
                if (_startInvestigation == false ||
                    _npc.Agent.remainingDistance < _npc.agentConfig.stoppingDistance)
                {
                    if (_startInvestigation)
                        _currentDestination = _npc.patrolPositions[_indexPatrol].transform.position;
                    _startInvestigation = true;
                    _npc.Agent.SetDestination(_npc.patrolPositions[_indexPatrol].transform.position);
                    _indexPatrol++;
                    if (_indexPatrol == _npc.patrolPositions.Length)
                        _indexPatrol = 0;
                }

                // VERY IMPORTANT TO PAUSE THE EXECUTION HERE, it will make sure that this coroutine can be stopped
                yield return null;

                TransitionToMeeting();
                yield return null;
            }
        }

        public override string ToString()
        {
            return "Doctor Investigate";
        }

        private void TransitionToMeeting()
        {
            AgentNPC partnerNPC = _npc.MeetSystem.FindNPCToMeet(typeof(PatrolBehaviour));
            if (partnerNPC != null)
            {
                Debug.Log($"Botul {_npc.name} si {partnerNPC.name} se intalnesc pt investigare");
                var talkDuration = Random.Range(8f, 11f);
                _npc.MeetSystem.Meet(partnerNPC, talkDuration);
                partnerNPC.MeetSystem.Meet(_npc, talkDuration);
            }
        }

        private void SetUpPosHolder()
        {
            if (_npc.posHolder != null)
            {
                // daca se intampla ca cineva sa puna un obiect aleator ca si PosHolder care nu are copii
                if (_npc.posHolder.transform.childCount == 0)
                    Debug.LogError("Pos holder of bot : " + name + "has no other children");
                // initializez vectorul de pozitii cu cati copii are posHolder
                _npc.patrolPositions = new GameObject[_npc.posHolder.transform.childCount];
                var i = 0;
                foreach (Transform child in _npc.posHolder.transform)
                {
                    GameObject childGameObject = child.gameObject;
                    _npc.patrolPositions[i] = childGameObject;
                    i++;
                    childGameObject.name = $"Pos{i}";
                }
            }
        }

        private void OnDrawGizmos()
        {
            WakeUp();
            if (_npc.posHolder == null) return;

            if (Application.isPlaying == false || _npc.BehaviourSystem.IsCurrentBehaviour(typeof(DoctorInvestigate)))
            {
                var positions = new List<Vector3>();
                foreach (GameObject patrolGameObject in _npc.patrolPositions)
                    positions.Add(patrolGameObject.transform.position);

                positions.Add(positions[0]);
                Color redColor = new Color(1f, 0.15f, 0.32f);
                Color blueColor = new Color(0.44f, 0.68f, 1f);
                for (var i = 0; i < positions.Count - 1; i++)
                {
                    if (Application.isPlaying && _currentDestination == positions[i + 1])
                        Gizmos.color = redColor;
                    else
                        Gizmos.color = blueColor;
                    Gizmos.DrawLine(positions[i], positions[i + 1]);
                    Vector3 direction = (positions[i] - positions[i + 1]).normalized;
                    Vector3 pos1 = positions[i + 1] + Quaternion.Euler(0, 45, 0) * direction;
                    Vector3 pos2 = positions[i + 1] + Quaternion.Euler(0, -45, 0) * direction;
                    if (Application.isPlaying && _currentDestination == positions[i + 1])
                        Gizmos.color = redColor;
                    else
                        Gizmos.color = Color.white;
                    Gizmos.DrawLine(positions[i + 1], pos1);
                    Gizmos.DrawLine(positions[i + 1], pos2);
                }
            }
        }
    }
}
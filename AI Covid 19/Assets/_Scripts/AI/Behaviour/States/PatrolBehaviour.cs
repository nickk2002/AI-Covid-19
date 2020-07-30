using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class PatrolBehaviour : MonoBehaviour, IBehaviour
    {
        private Vector3 _currentDestination = Vector3.negativeInfinity;
        private int _indexPatrol = 0;
        private AgentNPC _npc;
        private bool _startPatroling = false;

        public void Enable()
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
                if (_startPatroling == false ||
                    _npc.Agent.remainingDistance < _npc.agentConfig.stoppingDistance)
                {
                    if (_startPatroling)
                        _currentDestination = _npc.patrolPositions[_indexPatrol].transform.position;
                    _startPatroling = true;
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
            return "Patrol";
        }

        private void TransitionToMeeting()
        {
            AgentNPC partnerNPC = _npc.MeetSystem.FindNPCToMeet(typeof(PatrolBehaviour));
            if (partnerNPC != null)
            {
                // check if the probabilites and the sociable level are satisfied for both
                var randomValue = Random.Range(1, 10);
                if (randomValue <= _npc.agentConfig.sociableLevel
                    && randomValue <= partnerNPC.agentConfig.sociableLevel)
                {
                    Debug.Log($"Botul {_npc.name} si {partnerNPC.name} se intalnesc"); // then they actually meet.
                    var talkDuration = Random.Range(8f, 11f);
                    _npc.MeetSystem.Meet(partnerNPC, talkDuration);
                    partnerNPC.MeetSystem.Meet(_npc, talkDuration);
                }
                else
                {
                    Debug.Log(
                        $"<color=red>meeting failed due to probability, expected <= {randomValue} {_npc.name} {partnerNPC.name} </color>");
                    _npc.MeetSystem.IgnoreAgent(partnerNPC, 10); // ignores agent for a number of 10 seconds
                    partnerNPC.MeetSystem.IgnoreAgent(_npc, 10);
                }
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
            Enable();
            if (_npc.posHolder == null) return;

            if (Application.isPlaying == false || _npc.BehaviourSystem.IsCurrentBehaviour(typeof(PatrolBehaviour)))
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
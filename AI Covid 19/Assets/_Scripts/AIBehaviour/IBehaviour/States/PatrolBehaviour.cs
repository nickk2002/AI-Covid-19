using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class PatrolBehaviour : MonoBehaviour, IBehaviour
    {
        private NavMeshAgent _agent;
        private Coroutine _coroutine;
        private Vector3 _currentDestination = Vector3.negativeInfinity;
        private int _indexPatrol = 0;
        private AgentNPC _npc;
        private bool _startPatroling = false;

        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            _agent = GetComponent<NavMeshAgent>();
            SetUpPosHolder();
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                if (_startPatroling == false ||
                    _npc.Agent.remainingDistance < _npc.agentConfiguration.stoppingDistance)
                {
                    if (_startPatroling)
                        _currentDestination = _npc.patrolPositions[_indexPatrol].transform.position;
                    _startPatroling = true;
                    _npc.Agent.SetDestination(_npc.patrolPositions[_indexPatrol].transform.position);

                    _indexPatrol++;
                    if (_indexPatrol == _npc.patrolPositions.Length)
                        _indexPatrol = 0;
                }

                yield return
                    null; // VERY IMPORTANT TO PAUSE THE EXECUTION HERE, it will make sure that this coroutine can be stopped

                AgentNPC partnerNPC = _npc.MeetSystem.FindNPCToMeet();
                if (partnerNPC != null)
                {
                    // check if the probabilites and the sociable level are satisfied for both
                    var randomValue = Random.Range(1, 10);
                    if (randomValue <= _npc.agentConfiguration.sociableLevel
                        && randomValue <= partnerNPC.agentConfiguration.sociableLevel)
                    {
                        Debug.Log($"Botul {_npc.name} si {partnerNPC.name}"); // then they actually meet.
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

                yield return null;
            }
        }

        public override string ToString()
        {
            return "Patrol";
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

            if (Application.isPlaying == false || _npc.IsCurrentBehaviour(this))
            {
                var positions = new List<Vector3>();
                foreach (GameObject patrolGameObject in _npc.patrolPositions)
                    positions.Add(patrolGameObject.transform.position);

                positions.Add(positions[0]);
                for (var i = 0; i < positions.Count - 1; i++)
                {
                    if (Application.isPlaying && _currentDestination == positions[i + 1])

                        Gizmos.color = new Color(1f, 0.15f, 0.32f);
                    else
                        Gizmos.color = new Color(0.44f, 0.68f, 1f);
                    Gizmos.DrawLine(positions[i], positions[i + 1]);
                    Vector3 direction = (positions[i] - positions[i + 1]).normalized;
                    Vector3 pos1 = positions[i + 1] + Quaternion.Euler(0, 45, 0) * direction;
                    Vector3 pos2 = positions[i + 1] + Quaternion.Euler(0, -45, 0) * direction;
                    if (Application.isPlaying && _currentDestination == positions[i + 1])
                        Gizmos.color = new Color(1f, 0.15f, 0.32f);
                    else
                        Gizmos.color = Color.white;
                    Gizmos.DrawLine(positions[i + 1], pos1);
                    Gizmos.DrawLine(positions[i + 1], pos2);
                }
            }
        }
    }
}
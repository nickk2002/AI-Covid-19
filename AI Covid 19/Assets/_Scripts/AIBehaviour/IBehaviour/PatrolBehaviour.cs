using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AIBehaviour.Behaviour
{
    public class PatrolBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;
        private NavMeshAgent _agent;
        private int _indexPatrol = 0;
        private bool _startPatroling = false;
        private Coroutine _coroutine;

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
                    // vectorul de pozitii retine gameobject-uri ca .gameobject
                    _npc.patrolPositions[i] = child.gameObject; 
                    i++; // cresc indexul
                }
            }
        }

        public void Enable()
        {
            Debug.Log($"Entered patrol {name}");
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
                Debug.Log($"In patrol {name}");
                if (_startPatroling == false || _npc.Agent.remainingDistance < 0.2f)
                {
                    _startPatroling = true;
                    _indexPatrol++;
                    if (_indexPatrol == _npc.patrolPositions.Length)
                        _indexPatrol = 0;
                    _npc.Agent.SetDestination(_npc.patrolPositions[_indexPatrol].transform.position);
                }

                yield return null;
                
                AgentNPC partnerNPC = _npc.MeetingSystem.FindNPCToMeet();
                if (partnerNPC)
                {
                    // TODO : make the two agents wait the same random amount of time
                    Vector3 meetingPosition = _npc.MeetingSystem.GetMeetingPosition(partnerNPC);
                    MeetBehaviour meetBehaviour = _npc.gameObject.AddComponent<MeetBehaviour>();
                    meetBehaviour.MeetPosition = meetingPosition;
                    _npc.SetBehaviour(meetBehaviour);
                }

                yield return null;
            }
        }
    }
}
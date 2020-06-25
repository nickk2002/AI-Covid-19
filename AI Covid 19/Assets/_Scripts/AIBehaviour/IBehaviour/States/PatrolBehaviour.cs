using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AIBehaviour.Behaviour.States
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
                    _npc.patrolPositions[i] = child.gameObject; 
                    i++;
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
                if (_startPatroling == false || _npc.Agent.remainingDistance < _npc.patrolConfiguration.stoppingDistance)
                {
                    _startPatroling = true;
                    _npc.Agent.SetDestination(_npc.patrolPositions[_indexPatrol].transform.position);
                    
                    _indexPatrol++;
                    if (_indexPatrol == _npc.patrolPositions.Length)
                        _indexPatrol = 0;
                }

                yield return null; // VERY IMPORTANT TO PAUSE THE EXECUTION HERE, it will make sure that this coroutine can be stopped
                
                AgentNPC partnerNPC = _npc.MeetSystem.FindNPCToMeet();
                if (partnerNPC)
                {
                    _npc.MeetSystem.SetTalkDuration(partnerNPC,UnityEngine.Random.Range(8,10));

                    Vector3 meetingPosition = _npc.MeetSystem.GetMeetingPosition(partnerNPC);
                    MeetBehaviour meetBehaviour = _npc.gameObject.AddComponent<MeetBehaviour>();
                    meetBehaviour.MeetPosition = meetingPosition;
                    meetBehaviour.partnerNPC = partnerNPC;
                    _npc.SetBehaviour(meetBehaviour);
                }

                yield return null;
            }
        }
    }
}
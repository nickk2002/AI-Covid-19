using System;
using System.Collections;
using System.Collections.Generic;
using Covid19.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AIBehaviour.Behaviour
{
    public class AgentNPC : MonoBehaviour
    {
        public List<AgentNPC> ListAgents => listAgents;
        public MeetingSystem MeetingSystem => _meetingSystem;
        [SerializeField] private List<AgentNPC> listAgents;
        
        public GameObject[] patrolPositions; // array holding patrol positions
        public GameObject posHolder; // This is used for an easier way to set patrol points
        public NavMeshAgent Agent { get; private set; }
        [SerializeField] private bool randomLocations = false; // if he uses randomLocations
        [SerializeField] private float randomRange = 10f; // the range of the random
        [SerializeField] private float stoppingDistance = 3f; // the distance in which the agent stops and moves to the next position

        [SerializeField] public int offsetMeeting;
        [SerializeField] public float viewDist;
        [SerializeField] public float viewAngle;


        private Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
        private Dictionary<IBehaviour,Coroutine> _dictionary = new Dictionary<IBehaviour, Coroutine>();
        private IBehaviour _currentBehaviour;
        private MeetingSystem _meetingSystem;
        

        void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            SetBehaviour(GetComponent<IBehaviour>());
            _meetingSystem = new MeetingSystem(this);
        }
        
        public bool AcceptsMeeting(AgentNPC agentNPC)
        {
            // TODO : Modify to make the list of ignored bots etc.
            if (AIUtils.CanSeeObject(transform, agentNPC.transform, viewDist, viewAngle))
            {
                return true;
            }
            return false;
        }
        public void SetBehaviour(IBehaviour behaviour)
        {
            if (_currentBehaviour != null)
            {
                //Debug.Log($"Ending coroutine {_currentBehaviour} because we are setting up {behaviour} now",this);
                StopCoroutine(_dictionary[_currentBehaviour]);
                _currentBehaviour.Disable();
            }
            _currentBehaviour = behaviour; // set the current behaviour
            _currentBehaviour.Enable(); // run initialization logic ( get the dependencies)
            
            _behaviours.Push(_currentBehaviour); // push this behaviour to the stack
            Coroutine coroutine = StartCoroutine(_currentBehaviour.OnUpdate()); // cache the corutine to stop later
            //Debug.Log($"Putting the coroutine {_currentBehaviour} into the dictionary",this);
            _dictionary[_currentBehaviour] = coroutine;
        }
        public void RemoveBehaviour(IBehaviour behaviour)
        {
            StopCoroutine(_dictionary[behaviour]);
            behaviour.Disable();
            Destroy(behaviour as UnityEngine.Object);
            _behaviours.Pop();
            if (_behaviours.Count > 0)
            {
                _currentBehaviour = _behaviours.Peek();
                _currentBehaviour.Enable();
                Coroutine coroutine = StartCoroutine(_currentBehaviour.OnUpdate());
                _dictionary[_currentBehaviour] = coroutine;
            }
        }

        [ContextMenu("Disable patrol")]
        public void DisablePatrol()
        {
            _currentBehaviour.Disable();
        }
    }
}
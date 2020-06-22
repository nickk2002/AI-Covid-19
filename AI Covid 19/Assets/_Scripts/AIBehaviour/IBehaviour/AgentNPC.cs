using System.Collections.Generic;
using Covid19.AIBehaviour.Behaviour.States;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AIBehaviour.Behaviour
{
    public class AgentNPC : MonoBehaviour
    {
        public NavMeshAgent Agent { get; private set; }
        public List<AgentNPC> ListAgents => listAgents;
        public MeetSystem MeetSystem => _meetSystem;
        [SerializeField] private List<AgentNPC> listAgents;

        [Header("Patrol")]
        public GameObject[] patrolPositions; // array holding patrol positions
        public GameObject posHolder; // This is used for an easier way to set patrol points
        
        public PatrolConfiguration patrolConfiguration;
        
        public MeetConfiguration meetConfiguration;
        
        private MeetSystem _meetSystem;
        
        private readonly Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
        private readonly Dictionary<IBehaviour, Coroutine> _dictionary = new Dictionary<IBehaviour, Coroutine>();
        private IBehaviour _currentBehaviour;
        
        void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            SetBehaviour(GetComponent<IBehaviour>());
            _meetSystem = new MeetSystem(this);
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

    }
}
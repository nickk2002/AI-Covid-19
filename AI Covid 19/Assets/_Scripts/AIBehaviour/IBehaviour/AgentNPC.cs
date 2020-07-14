using System;
using System.Collections.Generic;
using Covid19.AIBehaviour.Behaviour.Configuration;
using Covid19.AIBehaviour.Behaviour.States;
using Covid19.Player;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Covid19.AIBehaviour.Behaviour
{
    public class AgentNPC : MonoBehaviour
    {
        public enum AgentType
        {
            Cook,
            BusinessMan,
            Doctor
        }
        
        public AgentType agentType;
        public AgentConfiguration agentConfiguration;
        public CoughConfiguration coughConfiguration;
        
        [HideInInspector] public GameObject[] patrolPositions; // array holding patrol positions
        // This is used for an easier way to set patrol points, should be added to a SO in the feature
        public GameObject posHolder;
        public GameObject rightHand;

        public NavMeshAgent Agent { get; private set; }
        public Animator Animator { get; private set; }
        public MeetSystem MeetSystem { get; private set; }
        public InfectionSystem InfectionSystem { get; private set; }
        
        
        private readonly Dictionary<IBehaviour, Coroutine>
            _behaviourCoroutine = new Dictionary<IBehaviour, Coroutine>();
        private readonly Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
        private AgentUI _agentUI;
        private Dictionary<AgentType, List<IBehaviour>> _altceva = new Dictionary<AgentType, List<IBehaviour>>();

        private List<Type> _basicActions = new List<Type>
        {
            typeof(MeetBehaviour),
            typeof(EatBeahaviour)
        };

        private IBehaviour _currentBehaviour;
        
        private void Start()
        {
            Type type = typeof(MeetBehaviour);
            // altceva.Add(GroupType.Cook, new List<IBehaviour>(TypingBehaviour), typeof(MeetBehaviour)));
            // altceva.Add(GroupType.Business, new List<IBehaviour>(TypingBehaviour, MeetBehaviour,));
            AgentManager.Instance.AddAgent(this);

            _agentUI = GetComponentInChildren<AgentUI>();

            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            SetBehaviour(GetComponent<IBehaviour>());

            MeetSystem = new MeetSystem(this);
            if(agentType != AgentType.Doctor)
                InfectionSystem = new InfectionSystem(this);
            
            StartInfection();
        }

        public bool IsCurrentBehaviour(IBehaviour behaviour)
        {
            if (_behaviours.Count == 0)
                return false;
            return _behaviours.Peek() == behaviour;
        }

        public void SetBehaviour(IBehaviour behaviour)
        {
            if (_currentBehaviour != null)
            {
                //Debug.Log($"Ending coroutine {_currentBehaviour} because we are setting up {behaviour} now",this);
                StopCoroutine(_behaviourCoroutine[_currentBehaviour]);
                _currentBehaviour.Disable();
            }

            _currentBehaviour = behaviour; // set the current behaviour
            _behaviours.Push(_currentBehaviour); // push this behaviour to the stack
            _currentBehaviour.Enable(); // run initialization logic ( get the dependencies)

            Coroutine coroutine = StartCoroutine(_currentBehaviour.OnUpdate()); // cache the corutine to stop later
            //Debug.Log($"Putting the coroutine {_currentBehaviour} into the dictionary",this);
            _behaviourCoroutine[_currentBehaviour] = coroutine;
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            StopCoroutine(_behaviourCoroutine[behaviour]);
            behaviour.Disable();
            Destroy(behaviour as Object);
            _behaviours.Pop();
            if (_behaviours.Count > 0)
            {
                _currentBehaviour = _behaviours.Peek();
                _currentBehaviour.Enable();
                Coroutine coroutine = StartCoroutine(_currentBehaviour.OnUpdate());
                _behaviourCoroutine[_currentBehaviour] = coroutine;
            }
        }

        public void StartInfection()
        {
            InfectionSystem?.StartInfection();
        }

        private void OnDrawGizmos()
        {
            if (_behaviours.Count > 0 && _agentUI) _agentUI.gizmoActionNameHolder.name = _behaviours.Peek().ToString();
        }
    }
}
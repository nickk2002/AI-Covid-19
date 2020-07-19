using System;
using System.Collections.Generic;
using Covid19.AI.Behaviour.Configuration;
using Covid19.AI.Behaviour.States;
using Covid19.AI.Behaviour.Systems;
using Covid19.Core;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Covid19.AI.Behaviour
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
        public GeneralAIConfiguration generalConfig;
        public CoughConfiguration coughConfiguration;
        
        [HideInInspector] public GameObject[] patrolPositions; // array holding patrol positions

        // This is used for an easier way to set patrol points, should be added to a SO in the feature
        public GameObject posHolder;
        public GameObject rightHand;

        public NavMeshAgent Agent { get; private set; }
        public Animator Animator { get; private set; }
        public MeetSystem MeetSystem { get; private set; }
        public InfectionSystem InfectionSystem { get; private set; }
        public BehaviourSystem BehaviourSystem { get; private set; }

        
        private AgentUI _agentUI;

        private void Start()
        {
            _agentUI = GetComponentInChildren<AgentUI>();

            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();


            MeetSystem = new MeetSystem(this);
            BehaviourSystem = new BehaviourSystem(this);
            BehaviourSystem.SetBehaviour(GetComponent<IBehaviour>());
            if (agentType != AgentType.Doctor)
                InfectionSystem = new InfectionSystem(this);
            Debug.Log($"there are {generalConfig.agentList.items.Count} bots");
        }
        
        public void StartInfection()
        {
            InfectionSystem?.StartInfection();
        }

        private void OnDrawGizmos()
        {
            //if (_behaviours.Count > 0 && _agentUI)
                //_agentUI.gizmoActionNameHolder.name = _behaviours.Peek().ToString();
        }
    }

}
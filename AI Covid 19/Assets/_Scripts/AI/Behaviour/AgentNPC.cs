using System;
using System.Collections.Generic;
using System.Globalization;
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
            if(GetComponent<IBehaviour>() != null)
                BehaviourSystem.SetBehaviour(GetComponent<IBehaviour>());
            
            if (agentConfiguration.agentType != BehaviourSystem.AgentType.Doctor)
                InfectionSystem = new InfectionSystem(this);
            StartInfection();
            Debug.Log($"there are {generalConfig.agentList.items.Count} bots");
        }
        
        public void StartInfection()
        {
            InfectionSystem?.StartInfection();
        }

        private void OnDrawGizmos()
        {
            Time.timeScale = generalConfig.timeScale;
            if (_agentUI)
            {
                _agentUI.actionName.name = BehaviourSystem.CurrentBehaviour.ToString();
                _agentUI.infectionLevel.name = InfectionSystem?.InfectionLevel.ToString("0.0000") ?? String.Empty;
                Debug.Log($"{BehaviourSystem.CurrentBehaviour.ToString()} {_agentUI.actionName.name}");
            }
        }
    }

}
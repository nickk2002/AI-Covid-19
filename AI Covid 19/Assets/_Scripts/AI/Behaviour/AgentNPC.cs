using System;
using Covid19.AI.Behaviour.Configuration;
using Covid19.AI.Behaviour.States;
using Covid19.AI.Behaviour.Systems;
using Covid19.AI.Behaviour.UI;
using Covid19.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI.Behaviour
{
    public class AgentNPC : MonoBehaviour
    {
        private AgentUI _agentUI;

        public AgentConfiguration agentConfig;
        public CoughConfiguration coughConfiguration;
        public GeneralAIConfiguration generalConfig;

        [HideInInspector] public GameObject[] patrolPositions; // array holding patrol positions

        // This is used for an easier way to set patrol points, should be added to a SO in the feature
        public GameObject posHolder;
        public GameObject rightHand;

        public NavMeshAgent Agent { get; private set; }
        public Animator Animator { get; private set; }
        public MeetSystem MeetSystem { get; private set; }
        public InfectionSystem InfectionSystem { get; private set; }
        public BehaviourSystem BehaviourSystem { get; private set; }
        public DebuggerSystem DebuggerSystem { get; private set; }

        private void Start()
        {
            _agentUI = GetComponentInChildren<AgentUI>();

            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();

            DebuggerSystem = new DebuggerSystem(this);
            MeetSystem = new MeetSystem(this);
            BehaviourSystem = new BehaviourSystem(this);
            if (GetComponent<IBehaviour>() != null)
                BehaviourSystem.SetBehaviour(GetComponent<IBehaviour>(),TransitionType.StackTransition);
            if (agentConfig == null)
            {
                Debug.LogError($"Agent Configuration SO not set in inspector {name}", this);
                return;
            }

            if (agentConfig.agentType != AgentType.Doctor)
                InfectionSystem = new InfectionSystem(this);
            StartInfection();
        }

        public void StartInfection()
        {
            InfectionSystem?.StartInfection();
        }

        // TODO : move this function somewhere else
        private void DrawLineOfSight()
        {
            if (generalConfig == null || agentConfig == null)
                return;
            Gizmos.color = Color.green;
            var circle = new Vector3[generalConfig.viewAngle + 2];
            var index = 0;
            var position = transform.position;
            for (var angle = -generalConfig.viewAngle / 2; angle <= generalConfig.viewAngle / 2; angle++)
            {
                var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
                if (agentConfig.drawRealSight)
                {
                    if (Physics.Raycast(position, direction, out RaycastHit hit, generalConfig.viewDistance))
                        circle[++index] = hit.point;
                    else
                        circle[++index] = position + direction.normalized * generalConfig.viewDistance;
                }
                else
                {
                    circle[++index] = position + direction.normalized * generalConfig.viewDistance;
                }
            }
            Gizmos.DrawLine(position, circle[1]);
            Gizmos.DrawLine(position, circle[index]);
            for (var i = 1; i <= index - 1; i++)
                Gizmos.DrawLine(circle[i], circle[i + 1]);
        }

        //TODO : have a proper UI Handler
        private void Update()
        {
            Time.timeScale = generalConfig.timeScale;

            _agentUI.actionName.name = BehaviourSystem.CurrentBehaviour.ToString();
            _agentUI.infectionLevel.name = InfectionSystem?.InfectionLevel.ToString("0.0000") ?? String.Empty;
        }

        override public string ToString()
        {
            return gameObject.name;
        }

        private void OnDrawGizmos()
        {
            DrawLineOfSight();
        }
    }
}
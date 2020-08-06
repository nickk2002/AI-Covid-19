using Covid19.AI.Behaviour.Configuration;
using Covid19.AI.Behaviour.States;
using Covid19.AI.Behaviour.Systems;
using Covid19.AI.Behaviour.UI;
using Covid19.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI.Behaviour
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AudioSource))]
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
        public AudioSystem AudioSystem { get; private set; }
        
        private void Start()
        {
            _agentUI = GetComponentInChildren<AgentUI>();

            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();

            MeetSystem = new MeetSystem(this);
            BehaviourSystem = new BehaviourSystem(this);

            DebuggerSystem = new DebuggerSystem(this); // depends on other behaviours
            AudioSystem = new AudioSystem(coughConfiguration.soundArray,GetComponent<AudioSource>());
            
            if (GetComponent<IBehaviour>() != null)
                BehaviourSystem.SetBehaviour(GetComponent<IBehaviour>(), TransitionType.StackTransition);
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

        //TODO : have a proper UI Handler
        private void Update()
        {
            Time.timeScale = generalConfig.timeScale;
            _agentUI.actionName.name = BehaviourSystem.CurrentBehaviour.ToString();
            if (InfectionSystem == null)
                _agentUI.infectionLevel.name = "Doctor";
            else if (InfectionSystem.Cured == false)
                _agentUI.infectionLevel.name = InfectionSystem.InfectionLevel.ToString("0.0000");
            else
                _agentUI.infectionLevel.name = "cured";

        }

        override public string ToString()
        {
            return gameObject.name;
        }

        private void OnDrawGizmos()
        {
            Debug.Assert(generalConfig != null, $"{this} the generalConfig is not set in the inspector");
            Debug.Assert(agentConfig != null, $"{this} the agentConfig is not set in the inspector");
            Debug.Assert(coughConfiguration != null, $"{this} the coughConfiguration is not set in the inspector");
            if (Application.isPlaying)
            {
                DebuggerSystem.DrawLineOfSight();;
            }
        }
    }
}
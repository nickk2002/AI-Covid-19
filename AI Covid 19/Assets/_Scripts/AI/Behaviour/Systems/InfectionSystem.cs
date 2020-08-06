#region

using System.Collections;
using Covid19.AI.Behaviour.States;
using UnityEngine;

#endregion

namespace Covid19.AI.Behaviour.Systems
{
    public class InfectionSystem
    {
        private static readonly int Cough = Animator.StringToHash("cough");
        private readonly Animator _animator;
        private readonly AudioSource _audioSource;

        public Infirmery Infirmery { get; }
        private readonly AgentNPC _npc;
        private Coroutine _coughCoroutine;
        private bool _cured = false;

        private bool _infected = false;

        private Coroutine _infectionGrowthCoroutine;
        private AudioClip _lastAudioClip;
        public float InfectionLevel { get; private set; }
        public bool Cured
        {
            get => _cured;
            set
            {
                if (_coughCoroutine != null && _infectionGrowthCoroutine != null)
                {
                    _npc.StopCoroutine(_coughCoroutine);
                    _npc.StopCoroutine(_infectionGrowthCoroutine);
                }

                _cured = value;
            }
        }

        public InfectionSystem(AgentNPC owner)
        {
            _npc = owner;
            _animator = owner.GetComponent<Animator>();
            _audioSource = owner.GetComponent<AudioSource>();
            if (owner.generalConfig.infirmeryList.items.Count > 0)
                Infirmery = (Infirmery) owner.generalConfig.infirmeryList.items[0];
            if (Infirmery == null)
            {
                Debug.LogError(
                    "Infirmery not set in the General AI SO. And no Add To List Component was added to infirmery gameobject. Trying to find in scene");
                Infirmery = Object.FindObjectOfType<Infirmery>();
                Debug.Log("<color=green>Infirmery was actually found!</color>");
            }
        }


        public void StartInfection()
        {
            if (_infected || _cured || Infirmery == null) return;

            _infected = true;
            _infectionGrowthCoroutine = _npc.StartCoroutine(InfectionHandler());
            _coughCoroutine = _npc.StartCoroutine(CoughHandler());
        }

        public void CallDoctor()
        {
            Infirmery.CallDoctor(_npc);
        }

        public void FreeBed()
        {
            Infirmery.FreeBed(_npc);
        }



        private void InfectNearbyAgents()
        {
            foreach (AgentNPC otherNPC in _npc.generalConfig.agentList.items)
                if (Vector3.Distance(_npc.transform.position, otherNPC.transform.position) <
                    _npc.generalConfig.infectionDistance)
                    otherNPC.StartInfection();
        }

        private IEnumerator CoughHandler()
        {
            while (true)
            {
                // evaluate from the cough curve (on OX-> infection level, OY = cough Interval)
                var coughVal = _npc.generalConfig.coughCurve.Evaluate(InfectionLevel);
                var coughInterval = Random.Range(coughVal - 1, coughVal); // add a bit of random 
                yield return new WaitForSeconds(coughInterval);
                if (_animator.GetBool(Cough) == false)
                {
                    _animator.SetTrigger(Cough);
                    //Debug.Log($"{_npc.name} has coughed!");
                    float coughLength = _npc.AudioSystem.PlayRandomCough();
                    InfectNearbyAgents();
                    yield return new WaitForSeconds(coughLength);
                }
            }
        }

        private IEnumerator InfectionHandler()
        {
            while (true)
            {
                yield return new WaitForSeconds(_npc.generalConfig.growthInterval);
                // add to the agent infection using the immunity
                InfectionLevel += Mathf.Max(_npc.generalConfig.maxInfectionValue,
                    _npc.generalConfig.infectionSpeed / _npc.agentConfig.immunityLevel);

                // if the agent is infected and the infirmery has available space,then go to infirmery
                // Cautious Level is 1-> 10, 1-> not cautious, 10 -> very cautious => [1,10] -> [10,1]
                int threshold = 10 - _npc.agentConfig.cautiousLevel + 1;
                if (InfectionLevel >= threshold && _npc.BehaviourSystem.IsCurrentBehaviour(typeof(PatrolBehaviour)) &&
                    Infirmery.HasAvailableSpace())
                {    
                    var behaviour = _npc.gameObject.AddComponent<PacientAtInfirmeryBehaviour>();
                    behaviour.destination = Infirmery.GetBedPosition(_npc);
                    _npc.BehaviourSystem.SetBehaviour(behaviour, TransitionType.StackTransition);
                }

                if (InfectionLevel == _npc.generalConfig.maxInfectionValue)
                    break; // if it reached the maximum level then we can stop the coroutine
            }
        }
    }
}
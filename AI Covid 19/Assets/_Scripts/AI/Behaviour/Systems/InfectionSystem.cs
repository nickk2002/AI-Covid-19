using System.Collections;
using Covid19.AI.Behaviour.States;
using UnityEngine;

namespace Covid19.AI.Behaviour.Systems
{
    public class InfectionSystem
    {
        private static readonly int Cough = Animator.StringToHash("cough");
        private readonly Animator _animator;
        private readonly AudioSource _audioSource;

        private readonly Infirmery _infirmery;
        private readonly AgentNPC _npc;
        private Coroutine _coughCoroutine;
        private bool _cured = false;

        private bool _infected = false;

        private Coroutine _infectionGrowthCoroutine;
        private AudioClip _lastAudioClip;

        public InfectionSystem(AgentNPC owner)
        {
            _npc = owner;
            _animator = owner.GetComponent<Animator>();
            _audioSource = owner.GetComponent<AudioSource>();
            if (owner.generalConfig.infirmeryList.items.Count > 0)
                _infirmery = (Infirmery) owner.generalConfig.infirmeryList.items[0];
            if (_infirmery == null)
            {
                Debug.LogError(
                    "Infirmery not set in the General AI SO. And no Add To List Component was added to infirmery gameobject. Trying to find in scene");
                _infirmery = Object.FindObjectOfType<Infirmery>();
                Debug.Log($"<color=green>Infirmery was actually found!</color>");
            }
        }

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

        public void StartInfection()
        {
            if (_infected || _cured || _infirmery == null) return;
            Debug.Log("Heii I am infected" + _npc.name);

            _infected = true;
            _infectionGrowthCoroutine = _npc.StartCoroutine(InfectionHandler());
            _coughCoroutine = _npc.StartCoroutine(CoughHandler());
        }

        public void CallDoctor()
        {
            _infirmery.CallDoctor(_npc);
        }

        public void FreeBed()
        {
            _infirmery.FreeBed(_npc);
        }

        private AudioClip RandomAudio()
        {
            AudioClip clip =
                _npc.coughConfiguration.soundArray[Random.Range(0, _npc.coughConfiguration.soundArray.Length - 1)];
            var tries = 0;
            while (clip == _lastAudioClip && tries <= 3)
            {
                clip = _npc.coughConfiguration.soundArray[
                    Random.Range(0, _npc.coughConfiguration.soundArray.Length - 1)];
                tries++;
            }

            _lastAudioClip = clip;
            return clip;
        }

        private void InfectNearbyAgents()
        {
            foreach (AgentNPC otherNPC in _npc.generalConfig.agentList.items)
            {
                if (Vector3.Distance(_npc.transform.position, otherNPC.transform.position) <
                    _npc.generalConfig.infectionDistance)
                    otherNPC.StartInfection();
            }
        }

        private IEnumerator CoughHandler()
        {
            while (true)
            {
                // evaluate from the cough curve (on OX-> infection level, OY = cough Interval)
                float coughVal = _npc.generalConfig.coughCurve.Evaluate(InfectionLevel);
                float coughInterval = Random.Range(coughVal - 1, coughVal); // add a bit of random 
                yield return new WaitForSeconds(coughInterval);
                if (_animator.GetBool(Cough) == false)
                {
                    _animator.SetTrigger(Cough);
                    AudioClip clip = RandomAudio();
                    _audioSource.clip = clip;
                    _audioSource.Play();
                    Debug.Log($"{_npc.name} has coughed!");
                    InfectNearbyAgents();
                    yield return new WaitForSeconds(clip.length);
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
                if (InfectionLevel == _npc.generalConfig.maxInfectionValue)
                    break; // if it reached the maximum level then we can stop the coroutine

                // if the agent is infected and had not already goen to the infirmery, and the infirmery has available space,then go to infirmery
                if (InfectionLevel > 10 && _npc.BehaviourSystem.IsCurrentBehaviour(typeof(PatrolBehaviour)) &&
                    _infirmery.HasAvailableSpace())
                {
                    GoToInfirmeryBehaviour behaviour = _npc.gameObject.AddComponent<GoToInfirmeryBehaviour>();
                    behaviour.destination = _infirmery.GetBedPosition(_npc);
                    _npc.BehaviourSystem.SetBehaviour(behaviour);
                }
            }
        }
    }
}
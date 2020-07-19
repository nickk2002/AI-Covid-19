﻿using System.Collections;
using Covid19.AI.Behaviour.States;
using UnityEngine;

namespace Covid19.AI.Behaviour.Systems
{
    public class InfectionSystem
    {
        private static readonly int Cough = Animator.StringToHash("cough");
        private readonly Animator _animator;
        private readonly AgentNPC _npc;

        private float _infectionLevel = 0;
        
        private Coroutine _infectionGrowthCoroutine;
        private Coroutine _coughCoroutine;
        
        private bool _goingToInfirmery = false;
        private bool _infected = false;
        private bool _cured = false;

        private AudioClip _lastAudioClip;
        private AudioSource _audioSource;
        
        
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

                Debug.Log("Stopped coroutines!");
                _cured = value;
            }
        }  

        public InfectionSystem(AgentNPC owner)
        {
            _npc = owner;
            _animator = owner.GetComponent<Animator>();
            _audioSource = owner.GetComponent<AudioSource>();
        }

        public void StartInfection()
        {
            if (_infected || _cured) return;
            Debug.Log("Heii I am infected" + _npc.name);
            _infected = true;
            _infectionGrowthCoroutine = _npc.StartCoroutine(InfectionHandler());
            _coughCoroutine = _npc.StartCoroutine(InfectingOthersHandler());
        }
        private AudioClip RandomAudio()
        {
            AudioClip clip = _npc.coughConfiguration.soundArray[Random.Range(0, _npc.coughConfiguration.soundArray.Length - 1)];
            var tries = 0;
            while (clip == _lastAudioClip && tries <= 3)
            {
                clip = _npc.coughConfiguration.soundArray[Random.Range(0, _npc.coughConfiguration.soundArray.Length - 1)];
                tries++;
            }

            _lastAudioClip = clip;
            return clip;
        }
        private void InfectNearbyAgents()
        {
            foreach (AgentNPC otherNPC in _npc.generalConfig.agentList.items)
            {
                if (Vector3.Distance(_npc.transform.position, otherNPC.transform.position) < _npc.generalConfig.infectionDistance)
                    otherNPC.StartInfection();
            }
        }

        private IEnumerator InfectingOthersHandler()
        {
            while (true)
            {
                // evaluate from the cough curve (on OX-> infection level, OY = cough Interval)
                var coughVal = _npc.generalConfig.coughCurve.Evaluate(_infectionLevel); 
                var coughInterval = Random.Range(coughVal - 1, coughVal); // add a bit of random 
                yield return new WaitForSeconds(coughInterval);
                if (_animator.GetBool(Cough) == false)
                {
                    _animator.SetTrigger(Cough);
                    AudioClip clip = RandomAudio();
                    _audioSource.clip = clip;
                    _audioSource.Play();
                    Debug.Log($"{_npc.name} has coughed!");
                    InfectNearbyAgents();
                }
            }
        }

        private IEnumerator InfectionHandler()
        {
            while (true)
            {
                yield return new WaitForSeconds(_npc.generalConfig.growthInterval);
                _infectionLevel += _npc.generalConfig.growthInterval;
                if (_infectionLevel > 1 && _goingToInfirmery == false && Infirmery.Instance.HasAvailableSpace() && _npc.BehaviourSystem.IsCurrentBehaviour(_npc.GetComponent<PatrolBehaviour>()))
                {
                    GoToInfirmeryBehaviour behaviour = _npc.gameObject.AddComponent<GoToInfirmeryBehaviour>();
                    behaviour.destination = Infirmery.Instance.GetBedPosition(_npc);
                    _goingToInfirmery = true;
                    _npc.BehaviourSystem.SetBehaviour(behaviour);
                    
                }
            }
        }
    }
}
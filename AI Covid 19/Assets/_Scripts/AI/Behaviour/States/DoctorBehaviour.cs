﻿using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class DoctorBehaviour : MonoBehaviour, States.IBehaviour
    {
        private AgentNPC _npc;
        private AudioSource _audioSource;

        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = true;
            _audioSource = GetComponent<AudioSource>();
            
            Player.Player.Instance.OnFirstCough += SayHeyToPlayer;
            Infirmery.Instance.AddDoctor(_npc);
            
        }

        public void SayHeyToPlayer()
        {
            _audioSource.Play();
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            yield return null;
        }

        public override string ToString()
        {
            return "Stays in place";
        }
    }
}
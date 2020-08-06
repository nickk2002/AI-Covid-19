using System.Collections;
using Covid19.AI.Behaviour.Systems;
using Covid19.Core;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class DoctorBehaviour : MonoBehaviour, IBehaviour
    {
        private AudioSource _audioSource;
        private AgentNPC _npc;

        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = true;
            _audioSource = GetComponent<AudioSource>();

            Player.OnFirstCough += SayHeyToPlayer;
            Infirmery infirmery = _npc.generalConfig.infirmeryList[0] as Infirmery;
            infirmery.AddDoctor(_npc);

            var doctorInvestigate = _npc.gameObject.AddComponent<DoctorInvestigate>();
            _npc.BehaviourSystem.SetBehaviour(doctorInvestigate, TransitionType.EntryTransition);
        }

        public void Exit()
        {
            _npc.Agent.isStopped = false;
        }

        public IEnumerator OnUpdate()
        {
            yield return null;
        }

        public void SayHeyToPlayer()
        {
            _audioSource.Play();
        }

        public override string ToString()
        {
            return "Stays in place";
        }
    }
}
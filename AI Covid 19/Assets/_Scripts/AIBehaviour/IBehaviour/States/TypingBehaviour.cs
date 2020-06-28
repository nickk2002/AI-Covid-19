using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    public class TypingBehaviour : MonoBehaviour, IBehaviour
    {
        public GameObject chairTarget;
        private Animator _animator;
        private AgentNPC _npc;

        private static readonly int Typing = Animator.StringToHash("typing");
        private static readonly int Sitting = Animator.StringToHash("sitting");
        private bool _reached = false;
        
        public void Enable()
        {
            _animator = GetComponent<Animator>();
            _npc = GetComponent<AgentNPC>();
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            var position = chairTarget.transform.position;
            Debug.Log($"in On Update {chairTarget} with position {position}", chairTarget);
            _npc.Agent.SetDestination(position);
            while (true)
            {
                if (Vector3.Distance(transform.position,chairTarget.transform.position) < 0.15f)
                {
                    if (_reached == false)
                    {
                        _npc.Agent.isStopped = true;
                        _reached = true;
                    }
                }

                if (_reached)
                {
                    transform.rotation = Quaternion.identity;
                    _animator.SetBool(Sitting,true);   
                    _animator.SetBool(Typing,true);
                }
                yield return null;    
            }
        }
    }
}
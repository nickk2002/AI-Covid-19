using System;
using System.Collections.Generic;
using Covid19.AI.Behaviour.States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Covid19.AI.Behaviour.Systems
{
    public class BehaviourSystem
    {    
        public enum AgentType
        {
            Cook,
            BusinessMan,
            Doctor
        }
        
        private readonly AgentNPC _npc;
        private readonly Dictionary<IBehaviour, Coroutine> _behaviourCoroutine = new Dictionary<IBehaviour, Coroutine>();
        private readonly Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
        private Dictionary<AgentType, List<IBehaviour>> _altceva = new Dictionary<AgentType, List<IBehaviour>>();

        private List<Type> _basicActions = new List<Type>
        {
            typeof(MeetBehaviour),
            typeof(EatBeahaviour)
        };
        
        public IBehaviour CurrentBehaviour { get; private set; }
        
        public BehaviourSystem(AgentNPC owner)
        {
            _npc = owner;
        }

        public bool IsCurrentBehaviour(IBehaviour behaviour)
        {
            if (_behaviours.Count == 0)
                return false;
            return _behaviours.Peek() == behaviour;
        }
        public void SetBehaviour(IBehaviour behaviour)
        {
            if (CurrentBehaviour != null)
            {
                _npc.StopCoroutine(_behaviourCoroutine[CurrentBehaviour]);
                CurrentBehaviour.Disable();
            }

            CurrentBehaviour = behaviour; // set the current behaviour
            _behaviours.Push(CurrentBehaviour); // push this behaviour to the stack
            CurrentBehaviour.Enable(); // run initialization logic ( get the dependencies)
            
            Coroutine coroutine = _npc.StartCoroutine(CurrentBehaviour.OnUpdate()); // cache the corutine to stop later
            _behaviourCoroutine[CurrentBehaviour] = coroutine;
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            _npc.StopCoroutine(_behaviourCoroutine[behaviour]);
            behaviour.Disable();
            Object.Destroy(behaviour as Object);
            _behaviours.Pop();
            if (_behaviours.Count > 0)
            {
                CurrentBehaviour = _behaviours.Peek();
                CurrentBehaviour.Enable();
                Coroutine coroutine = _npc.StartCoroutine(CurrentBehaviour.OnUpdate());
                _behaviourCoroutine[CurrentBehaviour] = coroutine;
            }
        }
    }
}
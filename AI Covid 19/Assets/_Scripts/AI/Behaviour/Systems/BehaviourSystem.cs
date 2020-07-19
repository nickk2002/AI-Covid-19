using System;
using System.Collections.Generic;
using Covid19.AI.Behaviour.States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Covid19.AI.Behaviour.Systems
{
    public class BehaviourSystem
    {
        private AgentNPC _npc;
        
        private readonly Dictionary<IBehaviour, Coroutine> _behaviourCoroutine = new Dictionary<IBehaviour, Coroutine>();
        private readonly Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
        private Dictionary<AgentNPC.AgentType, List<IBehaviour>> _altceva = new Dictionary<AgentNPC.AgentType, List<IBehaviour>>();

        private List<Type> _basicActions = new List<Type>
        {
            typeof(MeetBehaviour),
            typeof(EatBeahaviour)
        };
        
        private IBehaviour _currentBehaviour;
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
            if (_currentBehaviour != null)
            {
                _npc.StopCoroutine(_behaviourCoroutine[_currentBehaviour]);
                _currentBehaviour.Disable();
            }

            _currentBehaviour = behaviour; // set the current behaviour
            _behaviours.Push(_currentBehaviour); // push this behaviour to the stack
            _currentBehaviour.Enable(); // run initialization logic ( get the dependencies)
            
            Coroutine coroutine = _npc.StartCoroutine(_currentBehaviour.OnUpdate()); // cache the corutine to stop later
            _behaviourCoroutine[_currentBehaviour] = coroutine;
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            _npc.StopCoroutine(_behaviourCoroutine[behaviour]);
            behaviour.Disable();
            Object.Destroy(behaviour as Object);
            _behaviours.Pop();
            if (_behaviours.Count > 0)
            {
                _currentBehaviour = _behaviours.Peek();
                _currentBehaviour.Enable();
                Coroutine coroutine = _npc.StartCoroutine(_currentBehaviour.OnUpdate());
                _behaviourCoroutine[_currentBehaviour] = coroutine;
            }
        }

    }
}
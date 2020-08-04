using System;
using System.Collections.Generic;
using Covid19.AI.Behaviour.States;
using UnityEngine;
using static UnityEngine.Object;
using Object = UnityEngine.Object;


namespace Covid19.AI.Behaviour.Systems
{
    public class BehaviourSystem
    {
        private readonly Stack<IBehaviour> _behaviourStack = new Stack<IBehaviour>();

        // keep the Behaviour -> Coroutine map
        private readonly Dictionary<IBehaviour, Coroutine> _dictionary = new Dictionary<IBehaviour, Coroutine>();
        private readonly HashSet<IBehaviour> _entryBehaviours = new HashSet<IBehaviour>();
        private readonly AgentNPC _npc;

        // TODO : Add a group of actions based on the agentType
        // TODO : Pause and Resume the courotines
        private Dictionary<AgentType, List<IBehaviour>> _altceva = new Dictionary<AgentType, List<IBehaviour>>();

        private List<Type> _basicActions = new List<Type>
        {
            typeof(MeetBehaviour),
            typeof(EatBeahaviour),
        };

        public BehaviourSystem(AgentNPC owner)
        {
            _npc = owner;
        }

        public Dictionary<IBehaviour, Coroutine> Dictionary => _dictionary;
        public HashSet<IBehaviour> EntryBehaviours => _entryBehaviours;

        public IBehaviour CurrentBehaviour { get; private set; }

        public bool IsCurrentBehaviour(Type behaviour)
        {
            if (_behaviourStack.Count == 0)
                return false;
            return _behaviourStack.Peek().GetType() == behaviour;
        }

        public void SetBehaviour(IBehaviour behaviour, TransitionType type)
        {
            if (CurrentBehaviour != null)
            {
                StopRoutine(CurrentBehaviour);
                if (type == TransitionType.OverrideTransition)
                {
                    Destroy(CurrentBehaviour as Object);
                }
            }

            CurrentBehaviour = behaviour; // set the current behaviour
            _behaviourStack.Push(CurrentBehaviour); // push this behaviour to the stack

            if (!_entryBehaviours.Contains(CurrentBehaviour))
            {
                _npc.DebuggerSystem.Log($"<color=green> {_npc} wake up  {CurrentBehaviour} </color>", _npc);
                CurrentBehaviour.Entry(); // call wake up only when adding a new behaviour, not when resuming.
                _entryBehaviours.Add(CurrentBehaviour);
            }

            if (type != TransitionType.EntryTransition)
            {
                StartRoutine(CurrentBehaviour);
            }
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            StopRoutine(behaviour);
            _entryBehaviours.Remove(behaviour); // remove the behaviour => the next time is added the enable is called.
            Destroy(behaviour as Object);
            _npc.DebuggerSystem.Log($"{_npc} <color=red> Destroyed {behaviour} </color>");

            if (_behaviourStack.Count > 1)
            {
                _behaviourStack.Pop();
                CurrentBehaviour = _behaviourStack.Peek();
                StartRoutine(CurrentBehaviour);
            }
        }

        private void StartRoutine(IBehaviour behaviour)
        {
            _npc.DebuggerSystem.DebugStart(behaviour);
            Coroutine coroutine = _npc.StartCoroutine(behaviour.OnUpdate());
            _dictionary[behaviour] = coroutine;
            _npc.DebuggerSystem.PrintDictionary();
        }

        private void StopRoutine(IBehaviour behaviour)
        {
            _npc.DebuggerSystem.DebugExit(behaviour);
            if (_dictionary.ContainsKey(behaviour))
            {
                _npc.StopCoroutine(_dictionary[behaviour]);
                _dictionary.Remove(behaviour);
            }

            _npc.DebuggerSystem.PrintDictionary();
            behaviour.Exit();
        }
    }
}
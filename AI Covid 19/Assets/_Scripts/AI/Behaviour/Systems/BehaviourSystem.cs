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
        // keep the Behaviour -> Coroutine map
        private readonly Dictionary<IBehaviour, Coroutine> _behaviourCoroutine = new Dictionary<IBehaviour, Coroutine>();

        private readonly Stack<IBehaviour> _behaviourStack = new Stack<IBehaviour>();
        private readonly HashSet<IBehaviour> _wakedUpBehaviours = new HashSet<IBehaviour>();
        private readonly AgentNPC _npc;

        private int _numberOfTransitions = 0;

        // TODO : Add a group of actions based on the agentType
        private Dictionary<AgentType, List<IBehaviour>> _altceva = new Dictionary<AgentType, List<IBehaviour>>();

        private List<Type> _basicActions = new List<Type>
        {
            typeof(MeetBehaviour),
            typeof(EatBeahaviour)
        };

        public BehaviourSystem(AgentNPC owner)
        {
            _npc = owner;
        }

        public IBehaviour CurrentBehaviour { get; private set; }

        public bool IsCurrentBehaviour(Type behaviour)
        {
            if (_behaviourStack.Count == 0)
                return false;
            return _behaviourStack.Peek().GetType() == behaviour;
        }

        public void SetBehaviour(IBehaviour behaviour, TransitionType type)
        {
            _numberOfTransitions++;
            if (CurrentBehaviour != null)
            {
                StopCoroutineOnBehavaviour(CurrentBehaviour);
                
                if (type == TransitionType.OverrideTransition)
                {
                    Destroy(CurrentBehaviour as Object);
                }
            }

            CurrentBehaviour = behaviour; // set the current behaviour
            _behaviourStack.Push(CurrentBehaviour); // push this behaviour to the stack

            int lastNumberOfTransitions = _numberOfTransitions;
            if (!_wakedUpBehaviours.Contains(CurrentBehaviour))
            {
                _npc.DebuggerSystem.AddDebugLog($"<color=green> {_npc.name} wake up  {CurrentBehaviour} </color>", _npc);
                CurrentBehaviour.WakeUp(); // call wake up only when adding a new behaviour, not when resuming.
                _wakedUpBehaviours.Add(CurrentBehaviour);
            }

            // in CurrentBehaviour.WakeUp() the CurrentBehaviour can change by calling SetBehaviour() or RemoveBehaviour() and we must be aware of that!
            if (lastNumberOfTransitions == _numberOfTransitions)// if the number of Transitions changed in WakeUp()
            {
                StartCoroutineOnBehaviour(CurrentBehaviour);
            }
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            _numberOfTransitions++;
            StopCoroutineOnBehavaviour(behaviour);

            _wakedUpBehaviours.Remove(behaviour); // remove the behaviour => the next time is added the enable is called.
            Destroy(behaviour as Object);
            _npc.DebuggerSystem.AddDebugLog($"{_npc.name} <color=red> Destroyed {behaviour} </color>");
            
            if (_behaviourStack.Count > 1)
            {
                _behaviourStack.Pop();
                CurrentBehaviour = _behaviourStack.Peek();
                StartCoroutineOnBehaviour(CurrentBehaviour);
            }
        }

        private void StartCoroutineOnBehaviour(IBehaviour behaviour)
        {
            _npc.DebuggerSystem.AddDebugLog($"<color=green> {_npc.name} started coroutine  {behaviour.OnUpdate()} </color>", _npc);
            Coroutine coroutine = _npc.StartCoroutine(behaviour.OnUpdate());
            
            // the dictionary must not contain that key.
            Debug.Assert(_behaviourCoroutine.ContainsKey(CurrentBehaviour) == false, $"{_npc.name} There are two coroutines running at the same time {CurrentBehaviour}");
            _behaviourCoroutine[behaviour] = coroutine;
        }

        private void StopCoroutineOnBehavaviour(IBehaviour behaviour)
        {
            if (_behaviourCoroutine.ContainsKey(behaviour))
            {
                _npc.StopCoroutine(_behaviourCoroutine[behaviour]);
                _npc.DebuggerSystem.AddDebugLog($"<color=red> {_npc.name} exited {CurrentBehaviour}</color>", _npc);
                _behaviourCoroutine.Remove(behaviour);
            }
            else
            {
                _npc.DebuggerSystem.AddDebugLog($"<color=red> exited  {_npc.name} {behaviour}  but the coroutine was not in dict </color>", _npc);
            }
            behaviour.Disable();
        }       
    }
}
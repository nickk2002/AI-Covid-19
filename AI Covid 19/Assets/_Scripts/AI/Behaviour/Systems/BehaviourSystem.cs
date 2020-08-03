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
        public readonly Dictionary<IBehaviour, Coroutine> _behaviourCoroutine = new Dictionary<IBehaviour, Coroutine>();

        public readonly Stack<IBehaviour> _behaviourStack = new Stack<IBehaviour>();
        public readonly HashSet<IBehaviour> _wakedUpBehaviours = new HashSet<IBehaviour>();
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
            if (behaviour == CurrentBehaviour)
                return;
            _numberOfTransitions++;
            if (CurrentBehaviour != null)
            {
                StopCoroutineOnBehavaviour(CurrentBehaviour); // vreau sa stiu daca type este enable!
                
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
                _npc.DebuggerSystem.AddDebugLog($"<color=green> {_npc} wake up  {CurrentBehaviour} </color>", _npc);
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
            _npc.DebuggerSystem.AddDebugLog($"{_npc} <color=red> Destroyed {behaviour} </color>");
            
            if (_behaviourStack.Count > 1)
            {
                _behaviourStack.Pop();
                CurrentBehaviour = _behaviourStack.Peek();
                StartCoroutineOnBehaviour(CurrentBehaviour);
            }
        }
        private void StartCoroutineOnBehaviour(IBehaviour behaviour)
        {
            // the dictionary must not contain that key.
            Debug.Assert(_behaviourCoroutine.ContainsKey(behaviour) == false, $"{_npc} There are two coroutines running at the same time {behaviour}");
            
            _npc.DebuggerSystem.AddDebugLog($"<color=green> {_npc} started coroutine  {behaviour} + add to dict </color>", _npc);
            Coroutine coroutine = _npc.StartCoroutine(behaviour.OnUpdate());
            _behaviourCoroutine[behaviour] = coroutine;
            DictionaryPrinter();
        }
        private void StopCoroutineOnBehavaviour(IBehaviour behaviour)
        {
            if (_behaviourCoroutine.ContainsKey(behaviour))
            {
                _npc.StopCoroutine(_behaviourCoroutine[behaviour]);
                _npc.DebuggerSystem.AddDebugLog($"<color=red> {_npc} exited {CurrentBehaviour} + remove from dict</color>", _npc);
                _behaviourCoroutine.Remove(behaviour);
            }
            else if(_wakedUpBehaviours.Contains(behaviour) == true)
            {
                Debug.LogError($"{_npc} <color=red> exited {behaviour}  but the coroutine was not in dictionary, put yield return null before transition </color>", _npc);
            }
            DictionaryPrinter();
            behaviour.Disable();
        }

        private void DictionaryPrinter()
        {
            _npc.DebuggerSystem.AddDebugLog($"{_npc} dict is : {string.Join("",_behaviourCoroutine.Keys)}");
        }
    }
}
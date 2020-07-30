using System;
using System.Collections.Generic;
using Covid19.AI.Behaviour.States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Covid19.AI.Behaviour.Systems
{
    public class BehaviourSystem
    {
        private readonly Dictionary<IBehaviour, Coroutine>
            _behaviourCoroutine = new Dictionary<IBehaviour, Coroutine>();

        private readonly Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
        private readonly AgentNPC _npc;
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
            if (_behaviours.Count == 0)
                return false;
            return _behaviours.Peek().GetType() == behaviour;
        }

        public void SetBehaviour(IBehaviour behaviour)
        {
            if (CurrentBehaviour != null)
            {
                if (_behaviourCoroutine.ContainsKey(CurrentBehaviour))
                {
                    //_npc.StopAllCoroutines();
                    _npc.StopCoroutine(_behaviourCoroutine[CurrentBehaviour]);
                    Debug.Log($"<color=red> {_npc.name} exited {CurrentBehaviour} </color>", _npc);
                }
                else
                {
                    Debug.Log(
                        $"<color=red> exited  {_npc.name} {CurrentBehaviour}  but the coroutine was not in dict </color>",
                        _npc);
                }

                CurrentBehaviour.Disable();
            }

            CurrentBehaviour = behaviour; // set the current behaviour
            _behaviours.Push(CurrentBehaviour); // push this behaviour to the stack
            CurrentBehaviour.Enable(); // run initialization logic ( get the dependencies)
            Debug.Log($"<color=green> {_npc.name} started  {CurrentBehaviour} </color>", _npc);

            Coroutine coroutine = _npc.StartCoroutine(CurrentBehaviour.OnUpdate()); // cache the corutine to stop later
            _behaviourCoroutine[CurrentBehaviour] = coroutine;
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            _npc.StopCoroutine(_behaviourCoroutine[behaviour]);
            Debug.Log($"<color=red> {_npc.name} exited  {CurrentBehaviour} </color>", _npc);
            behaviour.Disable();
            Object.Destroy(behaviour as Object);
            _behaviours.Pop();
            if (_behaviours.Count > 0)
            {
                CurrentBehaviour = _behaviours.Peek();
                CurrentBehaviour.Enable();
                Debug.Log($"<color=green> {_npc.name} started {CurrentBehaviour} </color>", _npc);
                Coroutine coroutine = _npc.StartCoroutine(CurrentBehaviour.OnUpdate());
                _behaviourCoroutine[CurrentBehaviour] = coroutine;
            }
        }
    }
}
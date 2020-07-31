using System;
using System.Collections.Generic;
using Covid19.AI.Depreceated.StateMachine.States;

namespace Covid19.AI.Depreceated.StateMachine
{
    public class FiniteStateMachine
    {
        private Dictionary<Type, FSMState> _dictionary;

        private FSMState _currentState;
        private FSMState _lastState;

        public Bot Bot { get; }

        public FiniteStateMachine(Bot ownerBot /*,Dictionary<Type,FSMState> statesDictionary*/)
        {
            Bot = ownerBot;
            UnityEngine.Debug.Log("hei bot");
            _dictionary = new Dictionary<Type, FSMState>
            {
            };
        }

        public void ChangeState(FSMState state)
        {
            _lastState = state;
            _currentState = state;
        }

        public void Update()
        {
            _currentState.Update();
        }
    }
}
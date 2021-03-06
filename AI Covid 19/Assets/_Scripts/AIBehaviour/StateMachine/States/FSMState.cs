﻿using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AIBehaviour.StateMachine.States
{
    [System.Serializable]
    public class FSMState
    {
        public float ceva;
        public FiniteStateMachine StateMachine { get; }
        public Bot CurrentBot => StateMachine.Bot;
        public GameObject GameObject => CurrentBot.gameObject;
        public Transform Transform => CurrentBot.transform;
        public NavMeshAgent Agent => CurrentBot.GetComponent<NavMeshAgent>();

        public Vector3 CurrentDestination
        {
            get => CurrentBot.CurrentDestination;
            set => CurrentBot.CurrentDestination = value;
        }

        public FSMState(FiniteStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNPC : MonoBehaviour
{
    public GameObject[] patrolPositions; // array holding patrol positions
    public GameObject posHolder; // This is used for an easier way to set patrol points
    [HideInInspector] public NavMeshAgent Agent { get; private set; }
    [SerializeField] private bool randomLocations = false; // if he uses randomLocations
    [SerializeField] private float randomRange = 10f; // the range of the random
    [SerializeField] private float stoppingDistance = 3f; // the distance in which the agent stops and moves to the next position


    private Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
    private IBehaviour _currentBehaviour;
    
    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        SetBehaviour(GetComponent<IBehaviour>());
    }
    public void SetBehaviour(IBehaviour behaviour)
    {
        Debug.Log("Am intrat aici");
        if (_currentBehaviour != null)
            _currentBehaviour.Disable();
        _currentBehaviour = behaviour;
        _currentBehaviour.Enable();
        _behaviours.Push(_currentBehaviour);
    }

    public void RemoveBehaviour(IBehaviour behaviour)
    {
        behaviour.Disable();
        Destroy(behaviour as UnityEngine.Object);
        _behaviours.Pop();
        if (_behaviours.Count > 0)
        {
            _currentBehaviour = _behaviours.Peek();
            _currentBehaviour.Enable();
        }
    }

    [ContextMenu("Disable patrol")]
    public void DisablePatrol()
    {
        _currentBehaviour.Disable();
    }
}

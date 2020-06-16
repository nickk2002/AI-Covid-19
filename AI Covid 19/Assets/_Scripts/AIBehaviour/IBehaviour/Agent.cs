using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public GameObject[] patrolPositions; // array holding patrol positions
    public GameObject posHolder; // This is used for an easier way to set patrol points
    [SerializeField] private bool randomLocations = false; // if he uses randomLocations
    [SerializeField] private float randomRange = 10f; // the range of the random
    [SerializeField] private float stoppingDistance = 3f; // the distance in which the agent stops and moves to the next position


    private Stack<IBehaviour> _behaviours = new Stack<IBehaviour>();
    private IBehaviour _currentBehaviour;

    void Start()
    {
        SetBehaviour(GetComponent<IBehaviour>());
    }
    
    public void SetBehaviour(IBehaviour behaviour)
    {
        if (_currentBehaviour != null)
            _currentBehaviour.Disable();
        _currentBehaviour = behaviour;
        _currentBehaviour.Enable();
        StartCoroutine(_currentBehaviour.OnUpdate());
        _behaviours.Push(_currentBehaviour);
    }

    public void EndBehaviour(IBehaviour behaviour)
    {
        Destroy(behaviour as Object);
        _behaviours.Pop();
        if (_behaviours.Count > 0)
        {
            _currentBehaviour = _behaviours.Peek();
            StartCoroutine(_currentBehaviour.OnUpdate());
        }
    }


}

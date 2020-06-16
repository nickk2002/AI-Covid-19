using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehaviour : MonoBehaviour , IBehaviour
{
    private Agent _owner;
    private NavMeshAgent _agent;
    private int indexPatrol = 0;
    private bool startPatroling = false;

    private void Awake()
    {
        _owner = GetComponent<Agent>();
        _agent = GetComponent<NavMeshAgent>();
        SetUpPosHolder();
    }

    private void SetUpPosHolder()
    {
        if (_owner.posHolder != null)
        {
            if (_owner.posHolder.transform.childCount == 0) // daca se intampla ca cineva sa puna un obiect aleator ca si PosHolder care nu are copii
                Debug.LogError("Pos holder of bot : " + name + "has no other children");
            
            // initializez vectorul de pozitii cu cati copii are posHolder
            _owner.patrolPositions = new GameObject[_owner.posHolder.transform.childCount]; 
            var i = 0;
            foreach (Transform child in _owner.posHolder.transform)
            {
                _owner.patrolPositions[i] = child.gameObject; // vectorul de pozitii retine gameobject-uri asa ca .gameobject
                i++; // cresc indexul
            }
        }
    }
    public void Enable()
    {
        
    }

    public void Disable()
    {
        enabled = false;
    }
    
    public IEnumerator OnUpdate()
    {
        while (true)
        {
            Debug.Log(Vector3.Distance(transform.position, _owner.patrolPositions[indexPatrol].transform.position));
            if (startPatroling == false || _agent.remainingDistance < 2f)
            {
                startPatroling = true;
                indexPatrol++;
                if (indexPatrol == _owner.patrolPositions.Length)
                    indexPatrol = 0;
                _agent.SetDestination(_owner.patrolPositions[indexPatrol].transform.position);
            }

            yield return null;
        }
    }
}

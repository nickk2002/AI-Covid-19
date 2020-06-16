using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehaviour : MonoBehaviour , IBehaviour
{
    private AgentNPC _npc;
    private NavMeshAgent _agent;
    private int _indexPatrol = 0;
    private bool _startPatroling = false;
    private Coroutine _coroutine;
    
    private void SetUpPosHolder()
    {
        if (_npc.posHolder != null)
        {
            if (_npc.posHolder.transform.childCount == 0) // daca se intampla ca cineva sa puna un obiect aleator ca si PosHolder care nu are copii
                Debug.LogError("Pos holder of bot : " + name + "has no other children");
            
            // initializez vectorul de pozitii cu cati copii are posHolder
            _npc.patrolPositions = new GameObject[_npc.posHolder.transform.childCount]; 
            var i = 0;
            foreach (Transform child in _npc.posHolder.transform)
            {
                _npc.patrolPositions[i] = child.gameObject; // vectorul de pozitii retine gameobject-uri asa ca .gameobject
                i++; // cresc indexul
            }
        }
    }
    
    public void Enable()
    {
        _npc = GetComponent<AgentNPC>();
        _agent = GetComponent<NavMeshAgent>();
        SetUpPosHolder();
        _coroutine = StartCoroutine(OnUpdate());
    }

    public void Disable()
    {
        StopCoroutine(_coroutine);
        Debug.Log("Disabled patrol for now");    
    }
    
    public IEnumerator OnUpdate()
    {
        while (true)
        {
            Debug.Log("In update de la patrol");
            if (_startPatroling == false || _npc.Agent.remainingDistance < 0.2f)
            {
                _startPatroling = true;
                _indexPatrol++;
                if (_indexPatrol == _npc.patrolPositions.Length)
                    _indexPatrol = 0;
                _npc.Agent.SetDestination(_npc.patrolPositions[_indexPatrol].transform.position);
            }

            yield return null;
        }
    }
}

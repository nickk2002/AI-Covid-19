using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingBehaviour : MonoBehaviour,IBehaviour
{
    public GameObject chairTarget;
    private Animator _animator;
    private AgentNPC _npc;
    private Coroutine _coroutine;
    
    private static readonly int Typing = Animator.StringToHash("typing");
    
    
    public void Enable()
    {
        _animator = GetComponent<Animator>();
        _npc = GetComponent<AgentNPC>();
        _coroutine = StartCoroutine(OnUpdate());
    }

    public void Disable()
    {
        StopCoroutine(_coroutine);
    }

    public IEnumerator OnUpdate()
    {
        var position = chairTarget.transform.position;
        Debug.Log($"in On Update {chairTarget} with position {position}",chairTarget);
        _npc.Agent.SetDestination(position);
        while (true)
        {
            if (_npc.Agent.remainingDistance < 1)
            {
                _npc.Agent.isStopped = true;
                _animator.SetBool(Typing, true);
                yield return new WaitForSeconds(3f);
                _animator.SetBool(Typing, false);
                _npc.RemoveBehaviour(this);
                _npc.Agent.isStopped = false;
            }
            yield return null;
        }
    }
}

using System;
using Covid19.AIBehaviour.Behaviour;
using Covid19.AIBehaviour.Behaviour.States;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class BehaviourPlace : MonoBehaviour
{
    [SerializeField] private GameObject chair;
    private bool _occupied = false;
    
    private void OnTriggerEnter(Collider other)
    {
        AgentNPC npc = other.gameObject.GetComponent<AgentNPC>();
        
        if (npc != null && _occupied == false)
        {
            _occupied = true;
            TypingBehaviour typingBehaviour = npc.gameObject.AddComponent<TypingBehaviour>();
            typingBehaviour.chairTarget = chair;
            Debug.Log(typingBehaviour.chairTarget);
            npc.SetBehaviour(typingBehaviour);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _occupied = false;
    }
}

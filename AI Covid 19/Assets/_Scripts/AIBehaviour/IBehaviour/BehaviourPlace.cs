using Covid19.AIBehaviour.Behaviour;
using Covid19.AIBehaviour.Behaviour.States;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class BehaviourPlace : MonoBehaviour
{
    [SerializeField] private GameObject chair;
    
    private void OnTriggerEnter(Collider other)
    {
        AgentNPC npc = other.gameObject.GetComponent<AgentNPC>();
        if (npc != null)
        {
            Debug.Log("On trigger enter");
            TypingBehaviour typingBehaviour = npc.gameObject.AddComponent<TypingBehaviour>();
            typingBehaviour.chairTarget = chair;
            Debug.Log(typingBehaviour.chairTarget);
            npc.SetBehaviour(typingBehaviour);
        }
    }
}

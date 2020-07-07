using Covid19.AIBehaviour.Behaviour;
using Covid19.AIBehaviour.Behaviour.States;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class BehaviourPlace : MonoBehaviour
{
    private Collider _collider;
    private bool _occupied = false;

    [SerializeField] private GameObject chair;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var npc = other.gameObject.GetComponent<AgentNPC>();

        if (npc != null && _occupied == false)
        {
            _occupied = true;
            var typingBehaviour = npc.gameObject.AddComponent<TypingBehaviour>();
            typingBehaviour.chairTarget = chair;
            Debug.Log(typingBehaviour.chairTarget);
            npc.SetBehaviour(typingBehaviour);
            _collider.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _occupied = false;
    }
}
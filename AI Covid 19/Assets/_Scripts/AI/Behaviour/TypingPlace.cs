using System.Collections.Generic;
using Covid19.AI.Behaviour.States;
using UnityEngine;

namespace Covid19.AI.Behaviour
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class TypingPlace : MonoBehaviour
    {
        private Collider _collider;
        private bool _occupied = false;

        [SerializeField] private GameObject chair;
        [SerializeField] private GameObject destination;
        [SerializeField] private GameObject chairFinalPosition;
        [SerializeField] private GameObject chairCorrectPosition;
        [SerializeField] private GameObject mouse;

        private AgentNPC _npc;
        private readonly Dictionary<AgentNPC, float> _dictionary = new Dictionary<AgentNPC, float>();
        
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _npc = other.gameObject.GetComponent<AgentNPC>();

            if (_npc != null && _occupied == false && _npc.GetComponent<TypingBehaviour>() == null)
            {
                _occupied = true;
                var typingBehaviour = _npc.gameObject.AddComponent<TypingBehaviour>();
                typingBehaviour.targetPosition = destination;
                typingBehaviour.chairFinalPosition = chairFinalPosition.transform.position;
                typingBehaviour.chair = chair;
                typingBehaviour.mouse = mouse;
                typingBehaviour.correctSittingPosition = chairCorrectPosition.transform.localPosition;
                Debug.Log(typingBehaviour.targetPosition);
                _npc.SetBehaviour(typingBehaviour);
                _collider.enabled = false;
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (_npc.GetComponent<TypingBehaviour>() == null)
            {
                _occupied = false;
                // if(_dictionary.
                //     )
                _dictionary.Add(_npc, Time.time);
            }
        }
    }
}
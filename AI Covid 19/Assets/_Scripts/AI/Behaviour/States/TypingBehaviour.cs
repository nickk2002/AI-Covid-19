using System.Collections;
using UnityEngine;

namespace Covid19.AI.Behaviour.States
{
    public class TypingBehaviour : MonoBehaviour, IBehaviour
    {
        private static readonly int Typing = Animator.StringToHash("typing");
        private static readonly int Sitting = Animator.StringToHash("sitting");
        private Animator _animator;

        private Vector3 _chairInitialPosition;
        private Vector3 _initialPos;
        private Transform _initialTransformParent;
        private AgentNPC _npc;
        private bool _reachedChair = false;

        private bool _reachedDesk = false;
        private bool _startMoveChair = false;
        private float _typingDuration;
        public GameObject chair;
        public Vector3 chairFinalPosition;
        public Vector3 correctSittingPosition;
        public GameObject mouse;
        public GameObject targetPosition;

        public void Entry()
        {
            _animator = GetComponent<Animator>();
            _npc = GetComponent<AgentNPC>();
            _initialTransformParent = mouse.transform.parent;
            _initialPos = mouse.transform.localPosition;
            _chairInitialPosition = chair.transform.position;
        }

        public void Exit()
        {
        }


        public IEnumerator OnUpdate()
        {
            var position = targetPosition.transform.position;
            _npc.Agent.SetDestination(position);
            while (true)
            {
                if (Vector3.Distance(transform.position, targetPosition.transform.position) < 0.15f)
                {
                    if (_reachedChair == false)
                    {
                        _npc.Agent.isStopped = true;
                        _reachedChair = true;
                        transform.rotation = Quaternion.identity;
                        transform.position = targetPosition.transform.position;
                        transform.position = new Vector3(transform.position.x, 0.5223575f, transform.position.z);
                        _animator.Update(0);
                        _animator.SetBool(Sitting, true);
                    }
                }

                if (_reachedChair && _startMoveChair)
                {
                    chair.transform.position =
                        Vector3.MoveTowards(chair.transform.position, chairFinalPosition, Time.deltaTime);
                }

                if (Vector3.Distance(chair.transform.position, chairFinalPosition) < 0.1 && !_reachedDesk)
                {
                    _animator.SetBool(Typing, true);
                    _reachedDesk = true;
                }

                yield return null;
            }
        }


        public override string ToString()
        {
            return "Typing";
        }

        public void MoveChair()
        {
            Debug.Log("Move chair");
            _startMoveChair = true;
            transform.SetParent(chair.transform);
        }

        public void ParentMouse()
        {
            mouse.transform.SetParent(_npc.rightHand.transform, true);
        }

        public void UnParentMouse()
        {
            mouse.transform.parent = _initialTransformParent;
            mouse.transform.localPosition = _initialPos;
            mouse.transform.localRotation = Quaternion.identity;
        }

        public void PositionCorrectly()
        {
            transform.localPosition = correctSittingPosition;
            _animator.Update(0);
            StartCoroutine(StopTyping());
        }

        private IEnumerator StopTyping()
        {
            _typingDuration = Random.Range(5, 10);
            yield return new WaitForSeconds(_typingDuration);
            _animator.SetBool(Sitting, false);
            _animator.SetBool(Typing, false);
        }

        public void SitUp()
        {
            // this is called when the bot stood up from the chair => move chair back to initial position
            //StartCoroutine(MoveChairBack());
        }

        private IEnumerator MoveChairBack()
        {
            while (Vector3.Distance(chair.transform.position, _chairInitialPosition) < 0.1f)
            {
                chair.transform.position =
                    Vector3.MoveTowards(chair.transform.position, _chairInitialPosition, Time.deltaTime);
                yield return null;
            }

            chair.transform.position = _chairInitialPosition;
            _npc.Agent.isStopped = false;
            transform.parent = null;
            _npc.BehaviourSystem.RemoveBehaviour(this);
        }
    }
}
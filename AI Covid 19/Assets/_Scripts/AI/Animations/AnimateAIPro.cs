using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI.Animations
{
    public class AnimateAIPro : MonoBehaviour
    {
        [SerializeField] private float angleAccuracy = 1f;
        private NavMeshAgent _agent;
        private Animator _animator;
        private Vector3 _lastDirection;

        private float _angle;
        private bool _rotating = false;
        private bool _alreadyRotated = false;

        // Start is called before the first frame update
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.updatePosition = false;
            _agent.updateRotation = false;
        }

        private void Move(Vector3 direction)
        {
            if (_agent.hasPath == false)
            {
                _animator.SetFloat("Rotation", 0);
                _animator.SetFloat("Velocity", 0);
                return;
            }

            if (direction.magnitude > 1)
                direction.Normalize();
            _angle = Vector3.Angle(transform.forward, direction) * Mathf.Sign(Vector3.Dot(transform.right, direction));

            Debug.Log(_angle);
            _rotating = _animator.GetBool("rotating");
            if (Mathf.Abs(_angle) > 0.1f && _alreadyRotated == false)
            {
                _alreadyRotated = true;
                _rotating = true;
                _animator.SetFloat("StartAngle", _angle / 90);
            }

            if (_rotating == false && _alreadyRotated) transform.Rotate(0, _angle * Time.deltaTime * 10, 0);
            _animator.SetFloat("Angle", _angle / 90, 0.1f, Time.deltaTime);
            _animator.SetFloat("Velocity", direction.magnitude);
        }


        // Update is called once per frame
        private void Update()
        {
            if (_rotating == false) _lastDirection = _agent.desiredVelocity;
            Move(_lastDirection);
        }

        private void OnAnimatorMove()
        {
            transform.rotation = _animator.rootRotation;
            if (_rotating)
            {
                _agent.isStopped = true;
                transform.position = _animator.rootPosition;
                _agent.nextPosition = transform.position;
                Debug.Log("now rotating");
            }
            else
            {
                Debug.Log("now moving");
                _agent.isStopped = false;
                transform.position = _agent.nextPosition;
            }
        }

        private void OnDrawGizmos()
        {
            if (_agent != null)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(_agent.nextPosition, 0.3f);
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
                Gizmos.color = Color.magenta;
                var velocity = _lastDirection;
                Gizmos.DrawLine(_agent.nextPosition, _agent.nextPosition + velocity);

                if (_agent.destination != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(_agent.destination, 0.25f);
                }
            }
        }
    }
}
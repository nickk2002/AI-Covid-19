using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI.Animations
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class AnimateAI : MonoBehaviour
    {
        [SerializeField] private float turnMulitplyer = 5;
        private NavMeshAgent _agent;
        private Animator _animator;
        private float _angle;

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
            if (!_agent.hasPath || _agent.isStopped || _agent.enabled == false)
            {
                UpdateAnimator(0, 0);
            }
            else
            {
                if (direction.magnitude > 1)
                    direction.Normalize();
                _angle = Vector3.Angle(transform.forward, direction);
                if (Vector3.Cross(transform.forward, direction).y > 0)
                    _angle = -_angle;
                transform.Rotate(0, -_angle * turnMulitplyer * Time.deltaTime, 0);
                _angle /= 180;
                UpdateAnimator(direction.magnitude, _angle);
            }
        }

        private void UpdateAnimator(float speed, float rotation)
        {
            _animator.SetFloat("Velocity", speed, 0.1f, Time.deltaTime);
            _animator.SetFloat("Rotation", rotation, 0.1f, Time.deltaTime);
        }

        private void DoAnimations()
        {
            Move(_agent.desiredVelocity);
        }

        // Update is called once per frame
        private void Update()
        {
            DoAnimations();
        }

        private void OnAnimatorMove()
        {
            if (_agent.enabled == false || _agent.isStopped)
            {
                _agent.nextPosition = transform.position;
                transform.position = _animator.rootPosition;
            }
            else
            {
                transform.position = _agent.nextPosition;
            }

            transform.rotation = _animator.rootRotation;
        }

        private void OnDrawGizmos()
        {
            if (_agent != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_agent.nextPosition, 0.3f);
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
                Gizmos.color = Color.magenta;
                var velocity = _agent.desiredVelocity;
                Gizmos.DrawLine(_agent.nextPosition, _agent.nextPosition + velocity);

                if (_agent.destination != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(_agent.destination, 0.25f);
                }
            }
        }
    }
}
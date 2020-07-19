using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI.Elevator
{
    public class ElevatorAgent : MonoBehaviour
    {
        public Transform target;
        public Elevator elevator;
        public Transform startLink;
        public Transform endLink;
        private Rigidbody _rb;
        private NavMeshAgent _agent;

        // Start is called before the first frame update
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.destination = target.transform.position;
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            //elevator.MoveElevator(5, false);
        }

        private Vector3 _lastPos;

        // Update is called once per frame
        private void Update()
        {
            if (_agent.isOnOffMeshLink)
            {
                if (Vector3.Distance(transform.position, startLink.position) < 1f)
                {
                    Debug.Log("reached down");
                    elevator.MoveUp();
                    _agent.isStopped = true;
                    _rb.isKinematic = false;
                }

                if (elevator.transform.position.y - _lastPos.y == 0 && endLink.position.y - transform.position.y < 1f &&
                    _agent.isStopped)
                {
                    Debug.Log("In here");
                    elevator.GetComponent<NavMeshSurface>().BuildNavMesh();
                    _agent.isStopped = false;
                    _agent.Warp(_agent.transform.position);
                    _agent.autoTraverseOffMeshLink = false;
                    _agent.destination = target.position;
                    _agent.SetDestination(target.position);

                    _rb.isKinematic = true;
                }

                _lastPos = elevator.transform.position;
            }
        }
    }
}
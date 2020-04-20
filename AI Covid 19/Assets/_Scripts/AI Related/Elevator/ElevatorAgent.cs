using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class ElevatorAgent : MonoBehaviour
{
    public Transform target;
    public Elevator elevator;
    public Transform startLink;
    public Transform endLink;
    Rigidbody rb;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        //elevator.MoveElevator(5, false);
    }

    Vector3 lastPos;
    // Update is called once per frame
    void Update()
    {


        if (agent.isOnOffMeshLink == true)
        {
            if (Vector3.Distance(transform.position, startLink.position) < 1f)
            {
                Debug.Log("reached down");
                elevator.MoveUp();
                agent.isStopped = true;
                rb.isKinematic = false;
            }
            if (elevator.transform.position.y - lastPos.y == 0 && endLink.position.y - transform.position.y < 1f && agent.isStopped == true)
            {
                Debug.Log("In here");
                elevator.GetComponent<NavMeshSurface>().BuildNavMesh();
                agent.isStopped = false;
                agent.Warp(agent.transform.position);
                agent.autoTraverseOffMeshLink = false;
                agent.destination = target.position;
                agent.SetDestination(target.position);

                rb.isKinematic = true;


            }

            lastPos = elevator.transform.position;
        }

    }
}

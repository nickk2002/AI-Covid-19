  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimateAI : MonoBehaviour
{
    [SerializeField] float turnMulitplyer = 5;
    NavMeshAgent agent;
    Animator animator;
    float angle;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        
    }
    void Move(Vector3 direction)
    {
        if (!agent.hasPath || agent.isStopped == true || agent.enabled == false)
        {
            UpdateAnimator(0, 0);
        }
        else
        {
            if(direction.magnitude > 1)
                direction.Normalize();
            angle = Vector3.Angle(transform.forward, direction);
            if (Vector3.Cross(transform.forward, direction).y > 0)
                angle = -angle;
            transform.Rotate(0, -angle * turnMulitplyer *  Time.deltaTime , 0);
            angle /= 180;
            UpdateAnimator(direction.magnitude, angle);
        }
    }
    void UpdateAnimator(float speed,float rotation)
    {
        animator.SetFloat("Velocity", speed,0.1f,Time.deltaTime);
        animator.SetFloat("Rotation", rotation,0.1f,Time.deltaTime);
    }
    void DoAnimations()
    {
        Move(agent.desiredVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        DoAnimations();
    }
    private void OnAnimatorMove()
    {
        if (agent.enabled == false || agent.isStopped == true)
        {
            agent.nextPosition = transform.position;
            transform.position = animator.rootPosition;
        }
        else
        {
            transform.position = agent.nextPosition;
        }
        transform.rotation = animator.rootRotation;
    }
    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(agent.nextPosition, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
            Gizmos.color = Color.magenta;
            Vector3 velocity = agent.desiredVelocity;
            Gizmos.DrawLine(agent.nextPosition, agent.nextPosition + velocity);

            if (agent.destination != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(agent.destination, 0.25f);
            }
        }
        
    }

}
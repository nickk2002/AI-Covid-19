using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimateAIPro : MonoBehaviour
{
    [SerializeField] private float angleAccuracy = 1f;
    NavMeshAgent agent;
    Animator animator;
    Vector3 lastDirection;
    
    float angle;
    private bool rotating = false;
    private bool alreadyRotated = false;
    private bool smallAngleRot = false;


    IEnumerator Rotate()
    {
        smallAngleRot = true;
        Quaternion initialRot = transform.rotation;
        Quaternion desiredRot = transform.rotation * Quaternion.Euler(0, angle, 0);
        float initialTime = Time.time;
        float duration = 0.25f;
        
        while (Time.time - initialTime > duration)
        {
            transform.rotation = Quaternion.Lerp(initialRot, desiredRot, (Time.time - initialTime) / duration);
            yield return null;
        }
        transform.rotation = desiredRot;
        smallAngleRot = false;
    }
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
        if (agent.hasPath == false)
        {
            animator.SetFloat("Rotation", 0);
            animator.SetFloat("Velocity", 0);
            return;
        }
        if (direction.magnitude > 1)
            direction.Normalize();
        angle = Vector3.Angle(transform.forward, direction) * Mathf.Sign(Vector3.Dot(transform.right, direction));
        
        Debug.Log(angle);
        rotating = animator.GetBool("rotating");
        if (Mathf.Abs(angle) > 0.1f && alreadyRotated == false)
        {
            alreadyRotated = true;
            rotating = true;
            animator.SetFloat("StartAngle",angle / 90);
        }
        if(rotating == false && alreadyRotated == true){
            transform.Rotate(0, angle * Time.deltaTime * 10, 0);
        }
        animator.SetFloat("Angle", angle / 90,0.1f,Time.deltaTime);
        animator.SetFloat("Velocity",direction.magnitude);
        

    }



    // Update is called once per frame
    void Update()
    {
        if (rotating == false)
        {
            lastDirection = agent.desiredVelocity;
        }
        Move(lastDirection);
    }
    private void OnAnimatorMove()
    {
        transform.rotation = animator.rootRotation;
        if (rotating)
        {
            agent.isStopped = true;
            transform.position = animator.rootPosition;
            agent.nextPosition = transform.position;
            Debug.Log("now rotating");
        }
        else
        {
            Debug.Log("now moving");
            agent.isStopped = false;
            transform.position = agent.nextPosition;
        }

    }
    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(agent.nextPosition, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
            Gizmos.color = Color.magenta;
            Vector3 velocity = lastDirection;
            Gizmos.DrawLine(agent.nextPosition, agent.nextPosition + velocity);
            
            if (agent.destination != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(agent.destination, 0.25f);
            }
        }
        
    }

}

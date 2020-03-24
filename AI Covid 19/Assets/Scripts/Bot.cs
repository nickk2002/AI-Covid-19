using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    
    public enum State{
        Random,
        Meet,
    }
    public State currentState;
    public Bot meetingBot;
    public NavMeshAgent agent;
    public bool owner = false;
    public bool talking = false;
    public bool isMeeting = false;
    private Vector3 meetingPoint;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddBot(this);
        agent = GetComponent<NavMeshAgent>();
        
    }
    Vector3 RandomLoc()
    {
        return new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100));
    }
    void RandomWalk()
    {
        agent.SetDestination(RandomLoc());
    }
    void RandomTalk() { 
    
        
        int numberBots = GameManager.Instance.listaBoti.Count;
        int randomIndex = Random.Range(0, numberBots - 1);
        foreach(Bot bot in GameManager.Instance.listaBoti)
        {
            if (bot.currentState != State.Meet && bot != this)
                meetingBot = bot;
        }
        if(meetingBot == null)
        {
            currentState = State.Random;
            return;
        }
        owner = true;
        currentState = State.Meet;
        meetingBot.currentState = State.Meet;
        Vector3 randomLoc = RandomLoc();
        meetingPoint = randomLoc;

        /// se duc amandoi in acelasi loc
        meetingBot.agent.SetDestination(randomLoc);
        agent.SetDestination(randomLoc);

    }
    

    // Update is called once per frame
    void Update()
    {

        if (currentState == State.Random)
        {
            if (agent.remainingDistance < 1f || agent.destination == null)
            {
                RandomWalk();
            }
        }
        Debug.Log(gameObject.name + "is " + currentState);
        
    }
    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            Gizmos.DrawLine(transform.position, agent.destination);
            Gizmos.DrawSphere(agent.destination, 1f);
        }
        if (meetingPoint != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meetingPoint, 5);
        }
    }
}

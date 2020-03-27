using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    public GameObject[] pozitii;/// trebuie sa putem sa punem mai mult de 3 pozitii
    private int current = 0; //pun privat pt ca sunt prea multe
    float radius = 2f;
    private bool moving = false;  /// la fel pun privat
    public GameObject posHolder;

    private NavMeshAgent agent;
    public float offsetMeeting = 5;
    private bool inMeeting = false;
    [SerializeField] bool randomLocations;
    [SerializeField] bool drawLines = false;
    [SerializeField] bool realSightView = false;
    [SerializeField] int viewAngle;
    [SerializeField] float viewDistance;
    Vector3 meetingPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddBot(this); // pun in Gamemanager bot-ul
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true; // initial ii opresc pe amandoi
        pozitii = new GameObject[posHolder.transform.childCount];
        int i = 0;
        foreach (Transform child in posHolder.transform)
        {
            pozitii[i] = child.gameObject;
            i++;
        }
    }
    Vector3 RandomLoc()
    {
        return new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100));
    }
    void RandomWalk()
    {
        agent.SetDestination(RandomLoc());
    }
    bool CanSeeObject(Transform initial,Transform target)
    {
        if(Vector3.Distance(initial.position,target.position) <= viewDistance)
        {
            Vector3 diferenta = target.position - initial.position;
            if(Vector3.Angle(initial.forward,diferenta) <= viewAngle / 2)
            {  
                if(Physics.Raycast(initial.position, diferenta,viewDistance))
                {
                    return true;
                }
            }
        }
        return false;
        
    }
    void PrepareForMeeting(Bot bot,Vector3 meetingPosition)
    {
        bot.agent.isStopped = false;
        bot.agent.destination = meetingPosition;
        bot.inMeeting = true;
    }
    private int CompareBot(Bot a,Bot b)
    {
        // pt a compara botii a si b in functie de distanta. returneaza bot-ul cel mai apropiat de cel curent
        float dista = Vector3.Distance(transform.position, a.gameObject.transform.position);
        float distb = Vector3.Distance(transform.position, b.gameObject.transform.position);
        if (dista == distb)
            return 0;
        if (dista < distb)
            return -1; // botul a
        else
            return 1; // botul b
    }
    void TrySeeBot()
    {
        List<Bot> botiPosibili = new List<Bot>();
        foreach(Bot partnerBot in GameManager.Instance.listaBoti)
        {
            if (partnerBot != this && !inMeeting && !partnerBot.inMeeting && CanSeeObject(transform,partnerBot.transform) && CanSeeObject(partnerBot.transform,transform))
            {
                botiPosibili.Add(partnerBot);
            }
        }
        if (botiPosibili.Count == 0)
            return; /// nu a vazut pe nimeni care sa fie disponibil
        
        botiPosibili.Sort(CompareBot);

        foreach (Bot partnerBot in botiPosibili)
        {
            if (!inMeeting && !partnerBot.inMeeting)
            {
                Debug.Log(partnerBot.gameObject.name + " " + name + " " + inMeeting);
                Vector3 diferenta = partnerBot.transform.position - transform.position;/// vectorul de la botul curent la celelalt
                diferenta /= 2; // jumatatea distantei
                meetingPoint = transform.position + diferenta;
                Vector3 offset = diferenta.normalized * offsetMeeting;
                PrepareForMeeting(this, meetingPoint - offset); // botul curent
                PrepareForMeeting(partnerBot, meetingPoint + offset); // celelalt bot
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        TrySeeBot(); /// incearca sa gaseasca partener daca nu gaseste e fals
        if (inMeeting == false) /// daca nu se intalneste cu nimeni
        {
            //agent.isStopped = false;
            // daca nu are pozitie initiala
            if (moving == false)
            {
                if (randomLocations)
                    agent.SetDestination(RandomLoc());
                else
                    agent.destination = pozitii[current].transform.position;
                moving = true;
            }
            if (agent.remainingDistance < radius)
            {
                moving = false;
                current++;
                if (current >= pozitii.Length)
                {
                    current = 0;
                }
            }
        }

    }
    void DrawLineOfSight()
    {
        Gizmos.color = Color.green;
        Vector3[] cerc = new Vector3[viewAngle + 4];
        int cnt = 0;
        for (int unghi = -viewAngle / 2; unghi <= viewAngle / 2; unghi++)
        {
            Vector3 direction = Quaternion.Euler(0, unghi, 0) * transform.forward;
            if (realSightView)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, viewDistance))
                {
                    cerc[++cnt] = hit.point;
                }
                else
                {
                    cerc[++cnt] = transform.position + direction.normalized * viewDistance;
                }
            }
            else
            {
                cerc[++cnt] = transform.position + direction.normalized * viewDistance;
            }
        }
        Gizmos.DrawLine(transform.position, cerc[1]);
        Gizmos.DrawLine(transform.position, cerc[cnt]);
        for (int i = 1; i <= cnt - 1; i++)
            Gizmos.DrawLine(cerc[i], cerc[i + 1]);
    }
    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            if (inMeeting == true)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(agent.destination, 1f);
            }
            else
            {
                Gizmos.DrawLine(transform.position, agent.destination);
                Gizmos.DrawSphere(agent.destination, 1f);
            }
        }
        if(drawLines)
            DrawLineOfSight();
        if (meetingPoint != Vector3.zero)
        {
            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(meetingPoint, 3f);
        }
        
    }
}

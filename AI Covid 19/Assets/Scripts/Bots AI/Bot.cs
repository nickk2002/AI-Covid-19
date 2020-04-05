using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    #region public var movement
    [Header("General")]
    public bool infected;
    [SerializeField] GameObject head;
    [Range(0,10)]
    [SerializeField] float sociabalLevel;
    [Range(1, 10)]
    [SerializeField] float imunityLevel;

    [Header("Meeting Settings")]
    [SerializeField] bool inMeeting = false;
    [SerializeField] bool talking = false;
    [SerializeField] Bot meetingBot;
    [SerializeField] float offsetMeeting = 5;
    [SerializeField] float cooldownMeeting = 60;// dupa ce s-au intalnit asteapta 60 de secunde pana cand se mai intalnesc cu altii

    [Header("Talk Seetings")]
    [SerializeField] int talkDuration = 0;
    [SerializeField] float currentTalkTime = 0;
    

    [Header("Patrol type")]
    [SerializeField] bool patroling = false;
    [SerializeField] bool randomLocations = false;
    [SerializeField] float stoppingDistance = 3f;
    [SerializeField] GameObject[] pozitii;
    [SerializeField] GameObject posHolder;

    [Header("Bot view")]
    [SerializeField] int viewAngle = 60;
    [SerializeField] float viewDistance = 30;

    [Header("Line drawing gizmos")]
    [SerializeField] bool drawLines = false;
    [SerializeField] bool realSightView = false;

    [Header("Infection")]
    [SerializeField] public float infectionLevel;
    [SerializeField] float infectionRadius = 15;

    [Tooltip("Infection that grows by the value per minute")]
    [SerializeField] float infectionSpeed = 1f;
    [SerializeField] float infectionGrowthInterval = 3; /// 60 = un minut // testez cu 3 secunde

    [Header("UI")]
    [SerializeField] GameObject PlaceHolderInfectedUI;
    #endregion


    private Animator animator;
    private NavMeshAgent agent;

    private int currentIndexPatrol = 0;
    private Vector3 currentDestination;
    Vector3 meetingPoint;

    float currentCoughTime = 0;
    float coughInterval;
    float currentInfectionTime = 0;

    float lastFinishedMeetingTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddBot(this); // pun in Gamemanager bot-ul
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.autoBraking = false;
        if (posHolder != null)
        {
            pozitii = new GameObject[posHolder.transform.childCount];

            int i = 0;
            foreach (Transform child in posHolder.transform)
            {
                pozitii[i] = child.gameObject;
                i++;
            }
            Debug.LogWarning("Nu este setat Pos Holder");
        }
        else
            randomLocations = true;

        if (infected == true)
            StartInfection();
        coughInterval = Random.Range(3f, 10f);
    }
    Vector3 RandomLoc()
    {
        return new Vector3(Random.Range(-500, 500), 0, Random.Range(-500, 500));
    }
    void RandomWalk()
    {
        agent.SetDestination(RandomLoc());
    }
    bool CanSeeObject(Transform initial,Transform target)
    {
        Vector3 diferenta = target.position - initial.position;
       
        if (Vector3.Distance(initial.position,target.position) <= viewDistance)
        {
            if (Vector3.Angle(initial.forward,diferenta) <= viewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(initial.position, diferenta, out hit))
                {
                    if (hit.collider.gameObject.GetComponent<Bot>() != null)
                    {
                        return true;
                    }
                }
                else
                    return true;
             }
        }
        return false;
    }
    void PrepareForMeeting(Bot bot,Bot other,Vector3 currentMeetingPosition,int waitTime)
    {
        bot.agent.isStopped = false;
        bot.agent.destination = currentMeetingPosition;
        bot.meetingPoint = currentMeetingPosition;
        bot.inMeeting = true;
        bot.meetingBot = other;
        bot.talkDuration = waitTime;
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
        List<Bot> botiPosibili = new List<Bot>(); /// lista cu toti botii posibili
        foreach(Bot partnerBot in GameManager.Instance.listaBoti)
        {
            if (partnerBot != this && !inMeeting && !partnerBot.inMeeting 
                && CanSeeObject(head.transform,partnerBot.head.transform) 
                && CanSeeObject(partnerBot.head.transform,head.transform))
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
                //Debug.Log("se intalnesc cei doi boti: " + this.name + "si" + partnerBot.name);

                Vector3 diferenta = partnerBot.transform.position - transform.position;/// vectorul de la botul curent la celelalt
                diferenta /= 2; // jumatatea distantei
                Vector3 meetingPointHalf = transform.position + diferenta; // punctul de intalnire
                Vector3 offset = diferenta.normalized * offsetMeeting; // offset pt inalnire
                int randomValueBetweenRange = Random.Range(1, 10); // trebuie verificat sa fie la fel cu Range[1,10] sau daca e modificat in viitor

                // daca randomul este mai mic decat nivelul social al primului si este mai mic decat nivelul social al ceiluilat
                // atunci se pot intalni
                if (randomValueBetweenRange <= sociabalLevel && randomValueBetweenRange <= partnerBot.sociabalLevel)
                {

                    int randomTime = Random.Range(5, 10);  // aleg un numar random de secunde pentru timpul de intalnire
                    PrepareForMeeting(this, partnerBot, meetingPointHalf - offset, randomTime); // botul curent si celelalt
                    PrepareForMeeting(partnerBot, this, meetingPointHalf + offset, randomTime); // celelalt bot si botul curent
                    break; //am inchis
                }
            }
        }
    }
    public void StartInfection()
    {
        //Debug.Log("infect him");
        if (PlaceHolderInfectedUI != null)
        {
            InfectedUI ui = PlaceHolderInfectedUI.GetComponent<InfectedUI>();
            ui.imageDisplay.GetComponent<Image>().color = Color.red;
        }
        infectionLevel = 1;
    }
    void TryInfectOtherBot()
    {
        foreach(Bot possibleTarget in GameManager.Instance.listaBoti)
        {
            if (possibleTarget == this)
                continue;

            /// daca botul este infectat momentan poate sa infecteze alt bot care nu este infectat 
            /// TODO : si daca il vede.
            float dist = Vector3.Distance(transform.position, possibleTarget.gameObject.transform.position);
            if(possibleTarget.infectionLevel == 0 && dist <= infectionRadius)
            {
                possibleTarget.StartInfection();
            }
        }
    }
    void Cough()
    {
        float value = Random.Range(1, 100); // tusea are procent de 80%
        if (animator == null)
            return;
        if (currentCoughTime > coughInterval && value >= 80 && animator.GetBool("cough") == false)
        {
            currentCoughTime = 0;
            animator.SetTrigger("cough");
            TryInfectOtherBot();
            coughInterval = Random.Range(3f, 10f);
        }
        currentCoughTime += Time.deltaTime;
    }
    void EndMeeting(Bot bot)
    {
        bot.inMeeting = false;
        bot.patroling = false;
        bot.talking = false;
        bot.agent.isStopped = false;
        bot.lastFinishedMeetingTime = Time.time;
        bot.currentTalkTime = 0;
        bot.talkDuration = 0;
    }
    void Talk()
    {
        if (currentTalkTime > talkDuration)
        {
            EndMeeting(this);
            EndMeeting(meetingBot);
        }
        else
            currentTalkTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (infectionLevel > 0)
        {
            Cough();
            if (currentInfectionTime > infectionGrowthInterval) /// un minut
            {
                infectionLevel += infectionSpeed / imunityLevel;
                currentInfectionTime = 0;
            }
            currentInfectionTime += Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /// daca cooldown-ul a trecut de la ultima intalnire sau daca initial nu am avut nicio intalnire
        if (Time.time - lastFinishedMeetingTime >= cooldownMeeting || lastFinishedMeetingTime == 0)
        {
            TrySeeBot(); /// incearca sa gaseasca partener daca nu gaseste e fals
        }
        if (inMeeting == false) /// daca nu se intalneste cu nimeni
        {
            agent.isStopped = false;
            // daca nu are pozitie initiala
            if (patroling == false)
            {
                if (randomLocations)
                {
                    Vector3 randomLocation = RandomLoc();
                    agent.SetDestination(randomLocation);
                    currentDestination = randomLocation;
                }
                else
                {
                    agent.destination = pozitii[currentIndexPatrol].transform.position;
                    currentDestination = pozitii[currentIndexPatrol].transform.position;
                }
                patroling = true;
            }
            if (Vector3.Distance(transform.position, currentDestination) < stoppingDistance)
            {
                patroling = false;
                currentIndexPatrol++;
                if (currentIndexPatrol >= pozitii.Length)
                {
                    currentIndexPatrol = 0;
                }
            }
        }
        else
        {
            //Debug.Log(this.name + " is in meeting" + " " + Vector3.Distance(transform.position, meetingPoint) + " " + agent.remainingDistance);
            /// daca amandoi botii au ajuns la destinatie
            if (Vector3.Distance(this.transform.position, this.meetingPoint) < 2 &&
                Vector3.Distance(meetingBot.transform.position, meetingBot.meetingPoint) < 2)
            {
                talking = true; // incep sa vorbeasca
            }
            // daca amandoi vorbesc 
            if (this.talking == true && meetingBot.talking == true)
            {
                /// daca este in distanta
                if (agent.isStopped == false)
                {
                    /// daca inainte mergea acum il opresc
                    agent.isStopped = true;
                }
                Talk();
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
                Gizmos.color = Color.magenta;
            }
            else
            {
                Gizmos.DrawLine(transform.position, agent.destination);
                Gizmos.DrawSphere(agent.destination, 1f);
            }
        }
        if(drawLines)
            DrawLineOfSight();
    }
}

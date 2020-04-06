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
    [Tooltip("Sa se foloseasca doar inainte de a incepe jocul pentru a testa")]
    public bool infected;// daca este adevarat de la inceputul jocului, bot-ul va fi infectat din prima secunda, e folosit pt simulari
    [SerializeField] GameObject head; // gameobject considerat cap pentru bot
    [Range(0,10)]
    [SerializeField] float sociabalLevel; // cat de sociabil este un bot de la 1 la 10
    [Range(1, 10)]
    [SerializeField] float imunityLevel; // cat de imun este un bot de la 1 la 10

    // socilabilitatea influenteaza probabilitatea ca doi oameni sa vorbeasca
    // imunitatea influenteaza cat de repede creste boala. imunitate mare -> boala creste incet, imunitatea mica boala se dezvolta repede

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
        agent = GetComponent<NavMeshAgent>(); // iau Navmesh Agentul
        animator = GetComponent<Animator>(); // iau animatorul
        agent.autoBraking = false; // sa nu se opreasca cand se aproprie de destinatie

        if (posHolder != null)
        {
            if (posHolder.transform.childCount == 0)// daca se intampla ca cineva sa puna un obiect aleator ca si PosHolder care nu are copii
                Debug.LogError("Pos holder of bot : " + this.name + "has no other children");

            pozitii = new GameObject[posHolder.transform.childCount]; // initializez vectorul de pozitii cu cati copii are posHolder
            int i = 0;
            foreach (Transform child in posHolder.transform) // iterez prin fiecare copil din posHolder.transform (copii->transform)
            {
                pozitii[i] = child.gameObject; // vectorul de pozitii retine gameobject-uri asa ca .gameobject
                i++; // cresc indexul
            }
        }
        else 
        {
            // daca nu avem posHolder pus in inspector afisam un warning si folosim locatii random
            Debug.LogWarning("Nu este setat Pos Holder, Folosim pozitii random");
            randomLocations = true;
        }

        if (infected == true)
        {
            StartInfection();
            Debug.Log(infectionLevel);
        }
        
    }
    Vector3 RandomLoc()
    {
        return new Vector3(Random.Range(-500, 500), 0, Random.Range(-500, 500)); // locatie random pentru mapa mare.de 1000 * 1000 Scena : AISimulations
    }
    /// daca vede un obiect
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
    void PrepareForMeeting(Bot bot,Bot other,Vector3 currentMeetingPosition,int meetingDuration)
    {
        // pregatesc pentru intalnire botul bot si botul other pentru intalnire
        bot.agent.isStopped = false; // ma asigur ca nu sunt opriti
        bot.agent.destination = currentMeetingPosition; // pun destinatia
        bot.meetingPoint = currentMeetingPosition; // spun meeting point de vazut in inspector, nu fac nimic din cod cu el
        bot.inMeeting = true; // sunt in drum spre intalnire
        bot.meetingBot = other;// cu cine se intalneste
        bot.talkDuration = meetingDuration; // cat timp dureaza intalnirea
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
    void TryMeetBot()
    {
        List<Bot> botiPosibili = new List<Bot>(); /// lista cu toti botii posibili
        foreach(Bot partnerBot in GameManager.Instance.listaBoti)
        {
            /// daca botul pe care il caut este diferit de botul curent si daca botul curent nu este in meeting si 
            /// daca partnetBot la fel nu este in meeting il adaug la lista de boti posibili
            if (partnerBot != this && !inMeeting && !partnerBot.inMeeting 
                && CanSeeObject(head.transform,partnerBot.head.transform) 
                && CanSeeObject(partnerBot.head.transform,head.transform))
            {
                botiPosibili.Add(partnerBot);
            }
        }
        if (botiPosibili.Count == 0)
            return; /// nu a vazut pe nimeni care sa fie disponibil
        
        botiPosibili.Sort(CompareBot); /// sortez botii dupa distanta

        foreach (Bot partnerBot in botiPosibili)
        {
            if (!inMeeting && !partnerBot.inMeeting)/// daca nu sunt meeting nici botul curent nici partnerbot
            {
                //Debug.Log("se intalnesc cei doi boti: " + this.name + "si" + partnerBot.name);

                Vector3 diferenta = partnerBot.transform.position - transform.position;/// vectorul de la botul curent la celelalt
                diferenta /= 2; // jumatatea vectorului
                Vector3 meetingPointHalf = transform.position + diferenta; // punctul de intalnire la jumatate
                Vector3 offset = diferenta.normalized * offsetMeeting; // offset pt inalnire
                int randomValueBetweenRange = Random.Range(1, 10); // trebuie verificat sa fie la fel cu Range[1,10] sau daca e modificat in viitor

                // daca randomul este mai mic decat nivelul social al primului si este mai mic decat nivelul social al ceiluilat
                // atunci se pot intalni
                if (randomValueBetweenRange <= sociabalLevel && randomValueBetweenRange <= partnerBot.sociabalLevel)
                {

                    int randomTime = Random.Range(5, 10);  // aleg un numar random de secunde pentru timpul de intalnire
                    // this-> bot curent, partnetBot-> botul cu care se intalneste
                    PrepareForMeeting(this, partnerBot, meetingPointHalf - offset, randomTime); // pregatesc botul curent pentru intalnire
                    PrepareForMeeting(partnerBot, this, meetingPointHalf + offset, randomTime); // pregatesc celelalt bot pentru intalnire
                    break; // o data ce am gasit un partener de intalnire atunci inchid pentru ca numai are sens sa caut altul.
                }
            }
        }
    }
    public void StartInfection()
    {
        //Debug.Log("infect him");
        /// Daca am placeHolder pentru cerc rosu sau verde pus
        if (PlaceHolderInfectedUI != null)
        {
            InfectedUI ui = PlaceHolderInfectedUI.GetComponent<InfectedUI>();
            ui.imageDisplay.GetComponent<Image>().color = Color.red; // pun culoarea rosu pentru ca este infectat
        }
        infectionLevel = 1;// incep infectia
    }
    void TryInfectOtherBot()
    {
        foreach(Bot possibleTarget in GameManager.Instance.listaBoti)
        {
            if (possibleTarget == this) // daca botul possibleTarget nu este botul curent
                continue;
            /// daca botul este infectat momentan poate sa infecteze alt bot care nu este infectat intr-o anumita distanta
            /// TODO : si daca il vede. de vazut cum fcuntioneaza
            float dist = Vector3.Distance(transform.position, possibleTarget.gameObject.transform.position);
            if(possibleTarget.infectionLevel == 0 && dist <= infectionRadius)
            {
                possibleTarget.StartInfection();// incep infectia 
            }
        }
    }

    void Cough()
    {
        if (animator == null)
        {
            Debug.LogWarning("Nu am animator");
            return;
        }
        // mecanica de tusit
        // daca coughInterval este 0, adica inainte era 0 acum il initalizez cu un random intre 3 si 10. poate fi oricat sincer.
        if (coughInterval == 0)
        {
            coughInterval = Random.Range(3f, 10f);/// initializez un interval random pentru tusit, daca pun f la Random.Range() imi da un float
        }
        float value = Random.Range(1, 100); // tusea are procent de 80% . daca nu pun f la Random.Range() imi da un int intre 1 si 100
        // daca coughTime > coughInterval ( a trecut timpul de asteptat) , random-ul este peste 80% si el nu tuseste acum
        if (currentCoughTime > coughInterval && value >= 80 && animator.GetBool("cough") == false)
        {
            animator.SetTrigger("cough");// vezi animator 
            currentCoughTime = 0; // currentCoughTime = 0 pt ca reinitializez cronometrul
            TryInfectOtherBot(); // doar cand tuseste incearca sa infecteze alt bot
            coughInterval = Random.Range(3f, 10f); // al random interval pt incercat sa tuseasca
        }
        currentCoughTime += Time.deltaTime;// cronometrul creste cu Time.deltaTime
    }
    void EndMeeting(Bot bot)
    {
        // trebuie sa am grija sa resetez toate variabilele pentru amandoi botii, nu doar pentru unul dintr ei

        bot.agent.isStopped = false;// ma asiugur ca navmeshagentul este activ si poate merge
        bot.inMeeting = false; // nu mai este in intalnire
        bot.meetingBot = null; // nu are bot de intalnire, meetingBot nu e folosit in cod,este doar de testat in inspector
        bot.talkDuration = 0; // intalnirea dureaza 0 secunde, la fel e doar pt initializare sa fie 0 in inspector
        bot.currentTalkTime = 0;// resetez cronometrul de vorbit

        bot.patroling = false; // inapoi la starea initiala
        bot.talking = false; // numai vorbeste

        bot.lastFinishedMeetingTime = Time.time;  // intalnirea s-a terminat la timpul Time.time(cate secunde au trecut de la inceputul jocului)

    }
    void Talk()
    {

        if (currentTalkTime > talkDuration)
        {
            EndMeeting(this);// resetez totul pentru botul curent
            EndMeeting(meetingBot);// resetez totul pentru botul cu care se intalneste
        }
        else
            currentTalkTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (infectionLevel > 0)
        {
            Cough();// tuseste
            if (currentInfectionTime > infectionGrowthInterval) /// un minut
            {
                infectionLevel += infectionSpeed / imunityLevel; // creste invers proportional cu imunitatea
                currentInfectionTime = 0;// resetez cronomoetrul de crescut infectia
            }else
                currentInfectionTime += Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /// daca cooldown-ul a trecut de la ultima intalnire sau daca initial nu am avut nicio intalnire
        if (Time.time - lastFinishedMeetingTime >= cooldownMeeting || lastFinishedMeetingTime == 0)
        {
            TryMeetBot(); /// incearca sa gaseasca partener daca nu gaseste e fals
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

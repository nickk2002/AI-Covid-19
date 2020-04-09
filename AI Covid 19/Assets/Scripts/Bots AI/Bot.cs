using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AnimateAI))]
[Serializable]
// the core component of the AI 400+ lines of code, so it will take a while to read everything
public class Bot : MonoBehaviour
{

    // I use header to make a nice looking inspector, and SerializeField to make private variables be visible in the inspector
    #region public vars
    [Header("General")]
    [Tooltip("This is setting can be used for play testing")]
    public bool alreadyInfected;// if this option is enabled in the inspector the bot will start the game already infected
    [SerializeField] GameObject head; // gameobject for head used for detecting if two bots see each other
    [Range(0,10)] // a cool feature for modifying in the inspector with a slider
    [SerializeField] int sociableLevel; // how sociable is a bot from 0 to 10, 0 = avoids contact, 10 = very sociable
    [Range(1, 10)]
    [SerializeField] int immunityLevel; // how immune is a bot from a scale from 1 to 10, if this agent is already infected then the ilness will
                                        // develop differently according to the immunityLevel, if it is low(1,2 for ex), then the virus will grow fast, if the immunityLevel is 10
                                        // then the infection will grow slow
    // Quick recap :)
    // sociability -> Influences the probability of two bots meeting.
    // Two bots can meet under two conditions : they see each other and their sociableLevel is taken into a probability (see TryMeetBot)
    // Immunity -> how fast/slow the disease develops

    [Header("Bot view")] 
    [SerializeField] int viewAngle = 60; /// the angle a bot can see
    [SerializeField] float viewDistance = 30; // the distance(TODO : sociabalLevel should influence viewDistance

    [Header("Line drawing gizmos")]
    [SerializeField] bool drawLines = false;// if you want to draw in the scene the line of sight for each bot
    [SerializeField] bool realSightView = false;// if you want to draw in the scene the accurate line of sight(influenced by walls etc)

    [Header("Meeting Settings")]
    [SerializeField] bool inMeeting = false;// if this bot is currently on the way to meet another bot
    [SerializeField] bool talking = false;// if the bot is talking (no animations yet)
    [SerializeField] Bot meetingBot; // shows in the inspector who is the dialog partner, there is no need to modify it in the inspector
    [SerializeField] float offsetMeeting = 5;// an offset which says the distance between them when they are talking
    [SerializeField] float cooldownMeeting = 60;// after this bot has left the meeting he has a cooldown untill he finds another meeting
                                               // TODO : make cooldownMeeting be determined by sociableLevel
    
    [Header("Talk Seetings")]
    [SerializeField] int talkDuration = 0; // talk duration
    [SerializeField] float currentTalkTime = 0; // the current elasped time of talking (here for debugging)
    
    [Header("Patrol type")]
    [SerializeField] bool patroling = false; // if he is patroling
    [SerializeField] bool randomLocations = false; // if he uses randomLocations
    [SerializeField] float randomRange = 10f;// the range of the random
    [SerializeField] float stoppingDistance = 3f;// the distance in which the agent stops and moves to the next position
    [SerializeField] GameObject[] patrolPositions;// array holding patrol positions
    [SerializeField] GameObject posHolder; // This is used for an easier way to set patrol points
    // you create an empty object with any name you want, then create empty children that represent actual patrol positions
    // assign you empty object to posHolder in the inspector (see BasicScene.unity for details)

    [Header("Infection")]
    [SerializeField] public float infectionLevel;
    [SerializeField] float infectionSpeed = 1f;
    [SerializeField] float growthInterval = 3; /// infection grows by 1 unit every three seconds

    [Header("Coughing")]
    [SerializeField] float currentCoughTime = 0;
    [SerializeField] float coughInterval;
    [SerializeField] float infectDist = 10; // if this bot coughs, the distance of the virus transmission
    // if bot1 coughs and bot2 is in 10m distance then bot2 gets infected too

    [Header("UI")]
    [SerializeField] GameObject PlaceHolderInfectedUI; // for the circle UI
    #endregion


    private Animator animator;// animator
    private NavMeshAgent agent;// navmeshagent (if you don't know much about it it's easy) . all the functions are intuitive

    private int currentIndexPatrol = 0;
    private Vector3 currentDestination;
    Vector3 meetingPoint;
    List<Tuple<Bot, float>> listIgnoredBots = new List<Tuple<Bot, float>>();


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

            patrolPositions = new GameObject[posHolder.transform.childCount]; // initializez vectorul de pozitii cu cati copii are posHolder
            int i = 0;
            foreach (Transform child in posHolder.transform) // iterez prin fiecare copil din posHolder.transform (copii->transform)
            {
                patrolPositions[i] = child.gameObject; // vectorul de pozitii retine gameobject-uri asa ca .gameobject
                i++; // cresc indexul
            }
        }
        else 
        {
            // daca nu avem posHolder pus in inspector afisam un warning si folosim locatii random
            Debug.LogWarning("Nu este setat Pos Holder, Folosim pozitii random");
            randomLocations = true;
        }

        if (alreadyInfected == true)
        {
            StartInfection();
        }
        
    }
    Vector3 RandomLoc()
    {
        // locatie random pentru mapa mare.de 1000 * 1000 Scena : AISimulations
        return new Vector3(UnityEngine.Random.Range(-randomRange, randomRange), 0, UnityEngine.Random.Range(-randomRange, randomRange)); 
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
        List<Bot> possibleBots = new List<Bot>(); /// lista cu toti botii posibili
        foreach(Bot partnerBot in GameManager.Instance.listBots)
        {
            if(listIgnoredBots.Find(bot => bot.Item1 == partnerBot) != null)// daca botul partner bot este ignorat
            {
                float startIgnoringTime = listIgnoredBots.Find(bot => bot.Item1 == partnerBot).Item2;
                if(Time.time - startIgnoringTime < 10f)
                {
                    continue;
                }
                else
                {
                    listIgnoredBots.Remove(Tuple.Create(partnerBot,startIgnoringTime));
                }
            }
            /// daca botul pe care il caut este diferit de botul curent si daca botul curent nu este in meeting si 
            /// daca partnetBot la fel nu este in meeting il adaug la lista de boti posibili
            if (partnerBot != this && !inMeeting && !partnerBot.inMeeting 
                && CanSeeObject(head.transform,partnerBot.head.transform) 
                && CanSeeObject(partnerBot.head.transform,head.transform))
            {
                possibleBots.Add(partnerBot);
            }
        }
        if (possibleBots.Count == 0)
            return; /// nu a vazut pe nimeni care sa fie disponibil
        
        possibleBots.Sort(CompareBot); /// sortez botii dupa distanta

        foreach (Bot partnerBot in possibleBots)
        {
            /// daca nu sunt meeting nici botul curent nici partnerbot si daca celalat bot nu l-a ignorat pe cel curent
            /// adica daca partnerbot.lista nu are un tuple care sa aiba botul curent ca si item1
            if (!inMeeting && !partnerBot.inMeeting && partnerBot.listIgnoredBots.Find(bot => this == bot.Item1) == null)
            {
                int randomValueBetweenRange = UnityEngine.Random.Range(1, 10); // trebuie verificat sa fie la fel cu Range[1,10] sau daca e modificat in viitor
                // daca randomul este mai mic decat nivelul social al primului si este mai mic decat nivelul social al ceiluilat
                // atunci se pot intalni
                if (randomValueBetweenRange <= sociableLevel && randomValueBetweenRange <= partnerBot.sociableLevel)
                {
                    //Debug.Log("se intalnesc cei doi boti: " + this.name + "si" + partnerBot.name);

                    Vector3 diferenta = partnerBot.transform.position - transform.position;/// vectorul de la botul curent la celelalt
                    diferenta /= 2; // jumatatea vectorului
                    Vector3 meetingPointHalf = transform.position + diferenta; // punctul de intalnire la jumatate
                    Vector3 offset = diferenta.normalized * offsetMeeting; // offset pt inalnire

                    int randomTime = UnityEngine.Random.Range(5, 10);  // aleg un numar random de secunde pentru timpul de intalnire
                    // this-> bot curent, partnetBot-> botul cu care se intalneste
                    PrepareForMeeting(this, partnerBot, meetingPointHalf - offset, randomTime); // pregatesc botul curent pentru intalnire
                    PrepareForMeeting(partnerBot, this, meetingPointHalf + offset, randomTime); // pregatesc celelalt bot pentru intalnire
                    break; // o data ce am gasit un partener de intalnire atunci inchid pentru ca numai are sens sa caut altul.
                }
                else
                {
                    
                    listIgnoredBots.Add(Tuple.Create(partnerBot, Time.time)); 
                    // ignor botul pentru un anumit timp, retin timpul la care a inceput sa fie ignorat
                    
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
        foreach(Bot possibleTarget in GameManager.Instance.listBots)
        {
            if (possibleTarget == this) // daca botul possibleTarget nu este botul curent
                continue;
            /// daca botul este infectat momentan poate sa infecteze alt bot care nu este infectat intr-o anumita distanta
            /// TODO : si daca il vede. de vazut cum fcuntioneaza
            float dist = Vector3.Distance(transform.position, possibleTarget.gameObject.transform.position);
            if(possibleTarget.infectionLevel == 0 && dist <= infectDist)
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
            coughInterval = UnityEngine.Random.Range(3f, 10f);/// initializez un interval random pentru tusit, daca pun f la Random.Range() imi da un float
            //Debug.Log(coughInterval);
        }
        
        // daca coughTime > coughInterval ( a trecut timpul de asteptat) 
        if (currentCoughTime > coughInterval && animator.GetBool("cough") == false)
        {
            //Debug.Log("try to cough");
            float value = UnityEngine.Random.Range(1f, 100f);

            coughInterval = UnityEngine.Random.Range(3f, 10f); // al random interval pt incercat sa tuseasca
            currentCoughTime = 0; // currentCoughTime = 0 pt ca reinitializez cronometrul

            // in functie de infectie tuseste mai des daca infection Level e 80 atunci are 80 % sanse sa tuseasca
            // doar daca probailitatea este buna atunci poate sa tuseasca efectiv
            if (value <= infectionLevel) {//infectionLevel
                animator.SetTrigger("cough");// vezi animator 
                TryInfectOtherBot(); // doar cand tuseste incearca sa infecteze alt bot
            }
        }
        currentCoughTime += Time.deltaTime;// cronometrul creste cu Time.deltaTime
    }
    void EndMeeting(Bot bot)
    {
        // trebuie sa am grija sa resetez toate variabilele pentru amandoi botii, nu doar pentru unul dintr ei

        bot.agent.isStopped = false;// ma asiugur ca navmeshagentul este activ si poate merge
        bot.inMeeting = false; // nu mai este in intalnire

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
            if (currentInfectionTime > growthInterval) /// un minut
            {
                infectionLevel += infectionSpeed / immunityLevel; // creste invers proportional cu imunitatea
                infectionLevel = Mathf.Min(infectionLevel, 100f); // infectia maxima e 100
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
                    agent.destination = patrolPositions[currentIndexPatrol].transform.position;
                    currentDestination = patrolPositions[currentIndexPatrol].transform.position;
                }
                patroling = true;
            }
            if (Vector3.Distance(transform.position, currentDestination) < stoppingDistance)
            {
                patroling = false;
                currentIndexPatrol++;
                if (currentIndexPatrol >= patrolPositions.Length)
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
                meetingBot.talking = true;
                agent.isStopped = true;
                meetingBot.agent.isStopped = true;
            }
            // daca amandoi vorbesc 
            if (this.talking == true && meetingBot.talking == true)
            {               
                Talk();
            }
            
        }

    }
    void DrawLineOfSight()
    {
        Gizmos.color = Color.green;
        Vector3[] circle = new Vector3[viewAngle + 4];
        int index = 0;
        for (int angle = -viewAngle / 2; angle <= viewAngle / 2; angle++)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            if (realSightView)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, viewDistance))
                {
                    circle[++index] = hit.point;
                }
                else
                {
                    circle[++index] = transform.position + direction.normalized * viewDistance;
                }
            }
            else
            {
                circle[++index] = transform.position + direction.normalized * viewDistance;
            }
        }
        Gizmos.DrawLine(transform.position, circle[1]);
        Gizmos.DrawLine(transform.position, circle[index]);
        for (int i = 1; i <= index - 1; i++)
            Gizmos.DrawLine(circle[i], circle[i + 1]);
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
                //Gizmos.DrawLine(transform.position, agent.destination);
                //Gizmos.DrawSphere(agent.destination, 1f);
            }
        }
        if(drawLines)
            DrawLineOfSight();
    }
}

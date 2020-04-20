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
    [SerializeField] int cautiousLevel; // how aware he is of the virus -> he avoids others and goes to the doctor when there are symtomps
    // level 1 means that he behaves normal, he does nothing special to protect him, level 10 -> keeps distance between others.and goes to the doctor
    // as soon as he has symptoms of the illness (coughing at the moment)
    // level 5 is in the middle
    // Quick recap :)
    // sociability -> Influences the probability of two bots meeting.
    // Two bots can meet under two conditions : they see each other and their sociableLevel is taken into a probability (see TryMeetBot)
    // Immunity -> how fast/slow the disease develops

    [Header("Bot view")] 
    [SerializeField] int viewAngle = 60; /// the angle a bot can see
    [SerializeField] float viewDistance = 30; // the distance(TODO : sociabalLevel should influence viewDistance) (rezolvat in Start())


    [Header("Line drawing gizmos")]
    [SerializeField] bool drawLines = false;// if you want to draw in the scene the line of sight for each bot
    [SerializeField] bool realSightView = false;// if you want to draw in the scene the accurate line of sight(influenced by walls etc)

    [Header("Meeting Settings")]
    [SerializeField] bool inMeeting = false;// if this bot is currently on the way to meet another bot
    [SerializeField] bool talking = false;// if the bot is talking (no animations yet)
    [SerializeField] Bot meetingBot; // shows in the inspector who is the dialog partner, there is no need to modify it in the inspector
    [SerializeField] float offsetMeeting = 5;// an offset which says the distance between them when they are talking
    [SerializeField] float cooldownMeeting = 60;// after this bot has left the meeting he has a cooldown untill he finds another meeting
                                               // TODO : make cooldownMeeting be determined by sociableLevel ( rezolvat in Start)
                                               // is set by the AIManger initially, can be modified in inspector
    
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
    [SerializeField] public float infectionLevel;// from 1 to 10
    [SerializeField] float infectionSpeed = 0.1f;
    [SerializeField] float growthInterval = 3; /// infection grows by 1 unit every three seconds

    [Header("Coughing")]
    [SerializeField] float maxCoughInterval = 20;
    [SerializeField] float minCoughInterval = 1;
    [SerializeField] bool pureRandom = false;
    [SerializeField] float coughInterval; // random value between [maxCoughInverval,minCoughInterval]
    [SerializeField] float currentCoughTime = 0; // do not change
    [SerializeField] float infectDist = 10; // if this bot coughs, the distance of the virus transmission
    
    // if bot1 coughs and bot2 is in 10m distance then bot2 gets infected too

    [Header("UI")]
    [SerializeField] GameObject PlaceHolderInfectedUI; // for the circle UI
    #endregion

    public enum State
    {
        Patrol,
        Meet,
        Hospital
    }
    public bool cured = false;
    public State currentState;

    private NavMeshObstacle obstacle;
    private Animator animator;// animator
    private NavMeshAgent agent;// navmeshagent (if you don't know much about it it's easy) . all the functions are intuitive

    private int currentIndexPatrol = 0;
    private Vector3 currentDestination;
    Vector3 meetingPoint;
    List<Tuple<Bot, float>> listIgnoredBots = new List<Tuple<Bot, float>>();
    
    private int bedIndex;
    private Hospital hospital;

    float currentInfectionTime = 0;
    float lastFinishedMeetingTime = 0;

    void SetUpPosHolder()
    {
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
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(name);
        currentState = State.Patrol;
        GameManager.instance.AddBot(this); // pun in Gamemanager bot-ul

        cooldownMeeting = AIManager.instance.maxMeetingCooldown / sociableLevel;
        float ratie = (AIManager.instance.maxViewDistance - AIManager.instance.minViewDistance) / 9f; 
        viewDistance = AIManager.instance.minViewDistance + (sociableLevel - 1) * ratie;

        agent = GetComponent<NavMeshAgent>(); // iau Navmesh Agentul
        animator = GetComponent<Animator>(); // iau animatorul
        obstacle = GetComponent<NavMeshObstacle>(); // iau obstacolul ca sa il activez atunci cand vorbesc si se opresc
        obstacle.enabled = false;
        agent.autoBraking = false; // sa nu se opreasca cand se aproprie de destinatie

        SetUpPosHolder();
        if (alreadyInfected == true)
        {
            StartInfection();
        }
    }
    Vector3 RandomLoc()
    {
        // locatie random pentru mapa mare.de 1000 * 1000 Scena : AISimulations
        NavMeshHit hit;
        for(int i = 0; i < 30; i++)
        {
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-randomRange, randomRange), 0, UnityEngine.Random.Range(-randomRange, randomRange));
            if(NavMesh.SamplePosition(randomPosition,out hit, 1, NavMesh.AllAreas)){
                return hit.position;
            }
        }
        Debug.Log("Did not find anything");
        return new Vector3(UnityEngine.Random.Range(-randomRange, randomRange), 0, UnityEngine.Random.Range(-randomRange, randomRange));
    }
    #region meeting

    bool CanSeeObject(Transform initial,Transform target,float dist,float angle)
    {
        Vector3 diferenta = target.position - initial.position;
        RaycastHit hit;
        if (Vector3.Distance(initial.position,target.position) <= dist)
        {
            if (Vector3.Angle(initial.forward,diferenta) <= angle / 2)
            {
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
        bot.currentState = State.Meet;
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
        foreach(Bot partnerBot in GameManager.instance.listBots)
        {
            if(listIgnoredBots.Find(bot => bot.Item1 == partnerBot) != null)// daca botul partner bot este ignorat
            {
                float startIgnoringTime = listIgnoredBots.Find(bot => bot.Item1 == partnerBot).Item2;
                /// daca sa zicem bot1 si bot2 incearca sa se intalneasca dar nu reusesc din cauza probabilitatii
                ///  atunci se ignora timp de 10 pana cand pot sa mai incerce inca o data sa se intalneasca
                ///  10 secunde e timp standard,
                if (Time.time - startIgnoringTime < 10f)
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
                && CanSeeObject(head.transform,partnerBot.head.transform,viewDistance,viewAngle) 
                && CanSeeObject(partnerBot.head.transform,head.transform,partnerBot.viewDistance,partnerBot.viewAngle))
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
                    Vector3 offset = diferenta.normalized * offsetMeeting / 2; // offset pt inalnire

                    int average = (sociableLevel + partnerBot.sociableLevel) / 2;
                    int sum = sociableLevel + partnerBot.sociableLevel;
                    int randomTime = UnityEngine.Random.Range(average, sum);  // aleg un numar random de secunde pentru timpul de intalnire
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
    #endregion
    #region coughing + infecting others
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
        foreach(Bot possibleTarget in GameManager.instance.listBots)
        {
            if (possibleTarget == this || possibleTarget.cured == true) // daca botul possibleTarget nu este botul curent si daca nu e vindecat deja  
                continue;
            /// daca botul este infectat momentan poate sa infecteze alt bot care nu este infectat intr-o anumita distanta
            /// TODO : si daca il vede. de vazut cum fcuntioneaza,
            // Varianta vechie bazzata doar pe distanta:
            //float dist = Vector3.Distance(transform.position, possibleTarget.gameObject.transform.position);
            //if(possibleTarget.infectionLevel == 0 && dist <= infectDist)
            //{
            //    possibleTarget.StartInfection();// incep infectia 
            //}
            // varianta noua:
            if(possibleTarget.infectionLevel == 0 && CanSeeObject(transform,possibleTarget.transform, infectDist, viewAngle))
            {
                possibleTarget.StartInfection();
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
        // daca coughInterval este 0, adica inainte era 0 acum il initalizez cu un random intre valmin si valmax
        if (coughInterval == 0)
        {
            if (!pureRandom)
            {
                float coughVal = GameManager.instance.coughCurve.Evaluate(infectionLevel);// evaluate from gamemanager  function
                coughInterval = UnityEngine.Random.Range(coughVal - 1, coughVal);// add a bit of random 
            }else
                coughInterval = UnityEngine.Random.Range(minCoughInterval, maxCoughInterval);
        }
        // if the bot is not currently coughing
        if (animator.GetBool("cough") == false)
        {
            // daca coughTime > coughInterval ( a trecut timpul de asteptat) 
            if (currentCoughTime > coughInterval)
            {
                if (!pureRandom)
                {
                    float coughVal = GameManager.instance.coughCurve.Evaluate(infectionLevel);// evaluate from a function
                    coughInterval = UnityEngine.Random.Range(coughVal - 1, coughVal);// add a bit of random 
                }
                else
                    coughInterval = UnityEngine.Random.Range(minCoughInterval, maxCoughInterval);
                currentCoughTime = 0; // reinitializez cronometrul          
                animator.SetTrigger("cough");// vezi animator 
                TryInfectOtherBot(); // doar cand tuseste incearca sa infecteze alt bot
            }
            currentCoughTime += Time.deltaTime;// cronometrul creste cu Time.deltaTime
        }
    }
    #endregion
    void EndMeeting(Bot bot)
    {
        // trebuie sa am grija sa resetez toate variabilele pentru amandoi botii, nu doar pentru unul dintr ei
        bot.obstacle.enabled = false;
        bot.agent.enabled = true;
        bot.agent.isStopped = false;// ma asiugur ca navmeshagentul este activ si poate merge
        bot.inMeeting = false; // nu mai este in intalnire

        bot.talkDuration = 0; // intalnirea dureaza 0 secunde, la fel e doar pt initializare sa fie 0 in inspector
        bot.currentTalkTime = 0;// resetez cronometrul de vorbit

        bot.currentState = State.Patrol;
        bot.patroling = false; // inapoi la starea initiala
        bot.talking = false; // numai vorbeste

        bot.lastFinishedMeetingTime = Time.time;  // intalnirea s-a terminat la timpul Time.time(cate secunde au trecut de la inceputul jocului)
        bot.animator.SetBool("talking",false);
    }
    void Talk()
    {
        if (currentTalkTime == 0)
            Debug.Log("start talking from bot script");
        if (currentTalkTime > talkDuration)
        {
            EndMeeting(this);// resetez totul pentru botul curent
            EndMeeting(meetingBot);// resetez totul pentru botul cu care se intalneste
            
        }
        else
            currentTalkTime += Time.deltaTime;
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if(talking == true)
        {
            animator.SetLookAtPosition(meetingBot.head.transform.position);
            animator.SetLookAtWeight(0.2f);
        }
    }

    private void LateUpdate()
    {
        if (infectionLevel > 0)
        {
            Cough();// tuseste
            if (currentInfectionTime > growthInterval) /// un minut
            {
                infectionLevel += infectionSpeed / immunityLevel; // creste invers proportional cu imunitatea
                infectionLevel = Mathf.Min(infectionLevel, 10); // infectia maxima e 10
                currentInfectionTime = 0;// resetez cronomoetrul de crescut infectia
            }else
                currentInfectionTime += Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("here every update! haha");
        Debug.Log("Ce mai faci");
        if(currentState == State.Patrol)
        {
            if(agent.isStopped == true)
                agent.isStopped = false;
            if (agent.enabled == false)
                agent.enabled = true;
            // in caz ca pe viitor
            if(patroling == false || Vector3.Distance(a: transform.position,b: currentDestination) < stoppingDistance)
            {
                if (randomLocations)
                {
                    Vector3 randomLocation = RandomLoc();
                    agent.SetDestination(target: randomLocation);
                    currentDestination = randomLocation;
                }
                else
                {    
                    agent.destination = patrolPositions[currentIndexPatrol].transform.position;
                    currentDestination = patrolPositions[currentIndexPatrol].transform.position;
                    currentIndexPatrol++;
                    if (currentIndexPatrol >= patrolPositions.Length)
                        currentIndexPatrol = 0;
                }
                patroling = true;
            }
            // Tranzitia 1 in starea de MEET
            /// daca cooldown-ul a trecut de la ultima intalnire sau daca initial nu am avut nicio intalnire
            if (Time.time - lastFinishedMeetingTime >= cooldownMeeting || lastFinishedMeetingTime == 0)
            {
                TryMeetBot(); /// incearca sa gaseasca partener 
            }
            /// Tranzitia 2 in starea de GOTOHOSPITAL
            if (infectionLevel > 5) // un anumit nivel
            {
                // find a free hospital place
                for (int i = 0; i < GameManager.instance.listHospitals.Count; i++)
                {
                    Hospital currentHospital = GameManager.instance.listHospitals[index: i];
                    if (currentHospital.Free())
                    {
                        Tuple<Vector3, int> tuple = currentHospital.BedPosition();
                        if (tuple.Item2 != -1)
                        {
                            currentDestination = tuple.Item1;
                            bedIndex = tuple.Item2;
                            hospital = currentHospital;
                            agent.SetDestination(target: currentDestination);
                            currentState = State.Hospital;
                            break;
                        }
                    }
                }
            }
        }else if(currentState == State.Meet)
        {
            if (Vector3.Distance(a: transform.position, b: meetingPoint) < 1f)
                agent.enabled = false;
            if(!agent.enabled  && !meetingBot.agent.enabled)
            {
                talking = true;
                obstacle.enabled = true;
                animator.SetBool(name: "talking", value: true);
                Talk();
            }
        }else if(currentState == State.Hospital)
        {
            if(Vector3.Distance(a: transform.position, b: currentDestination) < 5f)
            {
                infectionLevel = 0;
                hospital.LeaveBed(index: bedIndex);
                patroling = false;
                cured = true;
                currentState = State.Patrol;
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
                Gizmos.DrawLine(transform.position, agent.destination);
                Gizmos.DrawSphere(agent.destination, 1f);
            }
        }
        if(drawLines)
            DrawLineOfSight();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum State
{
    None,
    Patrol,
    Meet,
    Hospital,
    AnyAction,
}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AnimateAI))]
// the core component of the AI 400+ lines of code, so it will take a while to read everything
public class Bot : MonoBehaviour
{
    #region public vars
    public static List<Bot> listBots = new List<Bot>();
    [ShowOnly, SerializeField] string actionName = "No Action";

    [NonSerialized] public List<BotAction> reflectionActions /*{ get; private set; }*/ = new List<BotAction>();
    [NonSerialized] public List<string> reflectionFields /*{ get; private set; }*/ = new List<string>();
    [HideInInspector] public List<bool> toggleList = new List<bool>();
    [HideInInspector] public List<BotAction> actionList = new List<BotAction>();

    [ShowOnly, SerializeField] State currentState;
    public BotAction washHandsAction = new BotAction { };
    public BotAction typingAction = new BotAction { };
    public BotAction lookAtSphereAction = new BotAction { };
    public BotAction actiuneAction = new BotAction { };

    [Header("General")]
    [Tooltip("This is setting can be used for play testing")]
    // I use header to make a nice looking inspector, and SerializeField to make private variables be visible in the inspector
    public bool alreadyInfected;// if this option is enabled in the inspector the bot will start the game already infected
    [SerializeField] GameObject head; // gameobject for head used for detecting if two bots see each other
    [Range(0, 10)] // a cool feature for modifying in the inspector with a slider
    [SerializeField] int sociableLevel; // how sociable is a bot from 0 to 10, 0 = avoids contact, 10 = very sociable
    [Range(1, 10)]
    [SerializeField] int immunityLevel; // how immune is a bot from a scale from 1 to 10, if this agent is already infected then the ilness will
                                        // develop differently according to the immunityLevel, if it is low(1,2 for ex), then the virus will grow fast, if the immunityLevel is 10
                                        // then the infection will grow slow
    [SerializeField] int cautiousLevel; // how aware he is of the virus -> he avoids others and goes to the doctor when there are symtomps
    [SerializeField] float infectionDist = 10; // if this bot coughs, the distance of the virus transmission
                                               // level 1 means that he behaves normal, he does nothing special to protect him, level 10 -> keeps distance between others.and goes to the doctor
                                               // as soon as he has symptoms of the illness (coughing at the moment)
                                               // level 5 is in the middle
                                               // Quick recap :)
                                               // sociability -> Influences the probability of two bots meeting.
                                               // Two bots can meet under two conditions : they see each other and their sociableLevel is taken into a probability (see TryMeetBot)
                                               // Immunity -> how fast/slow the disease develops
    [ShowOnly] public float infectionLevel;// from 1 to 10

    // you create an empty object with any name you want, then create empty children that represent actual patrol positions
    // assign you empty object to posHolder in the inspector (see BasicScene.unity for details)
    [Header("Patrol Settings")]
    [SerializeField] bool randomLocations = false; // if he uses randomLocations
    [SerializeField] float randomRange = 10f;// the range of the random
    [SerializeField] float stoppingDistance = 3f;// the distance in which the agent stops and moves to the next position
    [SerializeField] GameObject[] patrolPositions;// array holding patrol positions
    [SerializeField] GameObject posHolder; // This is used for an easier way to set patrol points

    [Header("Infection")]
    [SerializeField] float infectionSpeed = 0.1f;
    [SerializeField] float growthInterval = 3; /// infection grows by 1 unit every three seconds
    [ShowOnly] public bool cured = false;

    [Header("Meeting Settings")]
    [SerializeField] float offsetMeeting = 5;// an offset which says the distance between them when they are talking
    [SerializeField] float cooldownMeeting = 60;// after this bot has left the meeting he has a cooldown untill he finds another meeting
                                                // TODO : make cooldownMeeting be determined by sociableLevel ( rezolvat in Start)
                                                // is set by the AIManger initially, can be modified in inspector
    [ShowOnly, SerializeField] bool inMeeting = false;// if this bot is currently on the way to meet another bot
    [ShowOnly, SerializeField] bool talking = false;// if the bot is talking (no animations yet)
    [ShowOnly, SerializeField] Bot meetingBot; // shows in the inspector who is the dialog partner, there is no need to modify it in the inspector

    [Header("Talk Seetings")]
    [ShowOnly, SerializeField] int talkDuration = 0; // talk duration
    [ShowOnly, SerializeField] float currentTalkTime = 0; // the current elasped time of talking (here for debugging)


    [Header("Coughing")]
    [ShowOnly, SerializeField] float coughInterval; // random value between [maxCoughInverval,minCoughInterval]
    [ShowOnly, SerializeField] float currentCoughTime = 0; // do not change

    [Header("Bot view")]

    [ShowOnly, SerializeField] int viewAngle = 60; /// the angle a bot can see
    [ShowOnly, SerializeField] float viewDistance = 30; // the distance(TODO : sociabalLevel should influence viewDistance) (rezolvat in Start())
    [SerializeField] bool drawLines = false;// if you want to draw in the scene the line of sight for each bot
    [SerializeField] bool realSightView = false;// if you want to draw in the scene the accurate line of sight(influenced by walls etc)
    // if bot1 coughs and bot2 is in 10m distance then bot2 gets infected too

    [Header("UI")]
    [SerializeField] BotUI uiManager;
    [SerializeField] GameObject PlaceHolderInfectedUI; // for the circle UI


    #endregion
    private State lastState = State.None;
    private NavMeshObstacle obstacle;
    private Animator animator;// animator
    private NavMeshAgent agent;// navmeshagent (if you don't know much about it it's easy) . all the functions are intuitive

    private int currentIndexPatrol = 0;
    private Vector3 currentDestination;
    Vector3 meetingPoint;
    List<Tuple<Bot, float>> listIgnoredBots = new List<Tuple<Bot, float>>();

    private bool startPatroling;

    private int bedIndex;
    private Vector3 bedPosition;
    private Hospital hospital;
    private ActionPlace lastPlace;

    float currentInfectionTime = 0;
    float lastFinishedMeetingTime = 0;
    float lastFinishedAnyActionTime = 0;
    private BotAction currentAction = null;

    private bool startAction;
    private bool actionPathPending;

    private InfectedUI ui;

    public static int CountNumberInfected()
    {
        int cnt = 0;
        foreach (Bot bot in listBots)
        {
            if (bot.infectionLevel > 0)
                cnt++;
        }
        return cnt;
    }

    #region actionList save/update add clear
    public void ClearList()
    {
        actionList.Clear();
    }
    public void SetUpReflection()
    {
        if (toggleList.Count < reflectionActions.Count)
        {
            int dif = reflectionActions.Count - toggleList.Count;
            for (int i = 1; i <= dif; i++)
            {
                toggleList.Add(false);
            }
        }
        foreach (FieldInfo field in typeof(Bot).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default))
        {
            string nume = field.Name;
            if (nume.EndsWith("Action") && field.FieldType == typeof(BotAction) && field.IsPrivate == false)
            {
                BotAction action = field.GetValue(this) as BotAction;
                action.name = nume;
                field.SetValue(this, action);

                if (reflectionActions.Contains(action) == false)
                {
                    reflectionActions.Add(action);
                }
            }
            else
            {
                if (reflectionFields.Contains(nume) == false)
                    reflectionFields.Add(nume);
            }
        }
        Debug.Log($"There are {reflectionActions.Count} actions");
    }
    public void SavePreset()
    {
        SaveSystem.SaveBotAsJson(reflectionActions);
        foreach (Bot bot in FindObjectsOfType<Bot>())
        {
            bot.UpdateGizmosUI(false);
        }
    }
    public void UpdatePreset(List<BotAction>actionList = null)
    {
        if (UnityEditor.EditorApplication.isPlaying)
            return;
        if (actionList == null) // updates from JSON
            reflectionActions = SaveSystem.LoadBotActions();
        else
            reflectionActions = actionList;
        FieldInfo[] fields = typeof(Bot).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default);
        int i = 0;
        foreach (FieldInfo field in fields)
        {
            if (field.Name.EndsWith("Action") && field.FieldType == typeof(BotAction) && field.IsPrivate == false)
            {
                UnityEditor.Undo.RecordObject(gameObject, "descriptive name of this operation");
                BotAction action = field.GetValue(this) as BotAction;
                field.SetValue(this, reflectionActions[i++]);
                UpdateGizmosUI(true);

                UnityEditor.Undo.RecordObject(gameObject, "descriptive name of this operation");
                UnityEditor.EditorUtility.SetDirty(gameObject);
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);

            }
        }
    }
    public void UpdateAllOthers()
    {
        var list = FindObjectsOfType<Bot>();
        foreach (var item in list)
        {
            if (item != null && item != this)
            {
                item.UpdatePreset(this.reflectionActions);
            }
        }
    }
    public void UpdateGizmosUI(bool upToDate)
    {
        if (uiManager == null)
        {
            Debug.LogWarning("No UI set for this bot", this);
            return;
        }
        if (upToDate)
            uiManager.LoadUpdated();
        else
            uiManager.LoadNotUpdated();
    }
    public void AddAction(BotAction action)
    {
        if (actionList.Find(x => x == action) == null)
        {
            actionList.Add(action);
        }
    }
    public void RemoveAction(BotAction action)
    {

        if (actionList.Count == 0) return;
        if (actionList.Contains(action) == true)
        {
            actionList.Remove(action);
        }
    }
    private int CompareAction(BotAction a, BotAction b)
    {

        // pt a compara botii a si b in functie de distanta. returneaza bot-ul cel mai apropiat de cel curent
        float pa = a.probability;
        float pb = b.probability;
        if (pa == pb)
            return 0;
        if (pa < pb)
            return -1; // botul a
        else
            return 1; // botul b
    }
    #endregion
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
    public static void ClearBots()
    {
        listBots.Clear();
    }

    Vector3 RandomLoc()
    {
        // locatie random pentru mapa mare.de 1000 * 1000 Scena : AISimulations
        NavMeshHit hit;
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-randomRange, randomRange), 0, UnityEngine.Random.Range(-randomRange, randomRange));
            if (NavMesh.SamplePosition(randomPosition, out hit, 1, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        Debug.Log("Did not find anything");
        return new Vector3(UnityEngine.Random.Range(-randomRange, randomRange), 0, UnityEngine.Random.Range(-randomRange, randomRange));
    }
    #region meeting

    bool CanSeeObject(Transform initial, Transform target, float dist, float angle)
    {
        Vector3 diferenta = target.position - initial.position;
        RaycastHit hit;
        if (Vector3.Distance(initial.position, target.position) <= dist)
        {
            if (Vector3.Angle(initial.forward, diferenta) <= angle / 2)
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
    void PrepareForMeeting(Bot bot, Bot other, Vector3 currentMeetingPosition, int meetingDuration)
    {
        bot.lastState = bot.currentState;
        bot.currentState = State.Meet;
        // pregatesc pentru intalnire botul bot si botul other pentru intalnire
        bot.agent.isStopped = false; // ma asigur ca nu sunt opriti
        bot.agent.destination = currentMeetingPosition; // pun destinatia
        bot.meetingPoint = currentMeetingPosition; // spun meeting point de vazut in inspector, nu fac nimic din cod cu el
        bot.inMeeting = true; // sunt in drum spre intalnire
        bot.meetingBot = other;// cu cine se intalneste
        bot.talkDuration = meetingDuration; // cat timp dureaza intalnirea
    }

    private int CompareBot(Bot a, Bot b)
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
        foreach (Bot partnerBot in Bot.listBots)
        {
            if (listIgnoredBots.Find(bot => bot.Item1 == partnerBot) != null)// daca botul partner bot este ignorat
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
                    listIgnoredBots.Remove(Tuple.Create(partnerBot, startIgnoringTime));
                }
            }
            /// daca botul pe care il caut este diferit de botul curent si daca botul curent nu este in meeting si 
            /// daca partnetBot la fel nu este in meeting il adaug la lista de boti posibili
            if (partnerBot != this && !inMeeting && !partnerBot.inMeeting
                && CanSeeObject(head.transform, partnerBot.head.transform, viewDistance, viewAngle)
                && CanSeeObject(partnerBot.head.transform, head.transform, partnerBot.viewDistance, partnerBot.viewAngle))
            {
                Debug.Log("<color=red>Can see each other</color>");
                possibleBots.Add(partnerBot);
            }
        }
        if (possibleBots.Count == 0)
            return; /// nu a vazut pe nimeni care sa fie disponibil
        Debug.Log("sunt : " + possibleBots.Count);
        possibleBots.Sort(CompareBot); /// sortez botii dupa distanta

        foreach (Bot partnerBot in possibleBots)
        {
            /// daca nu sunt meeting nici botul curent nici partnerbot si daca celalat bot nu l-a ignorat pe cel curent
            /// adica daca partnerbot.lista nu are un tuple care sa aiba botul curent ca si item1
            if (!inMeeting && !partnerBot.inMeeting && partnerBot.listIgnoredBots.Find(bot => this == bot.Item1) == null &&
                currentState != State.AnyAction && partnerBot.currentState != State.AnyAction)
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
    #region infect + ui
    public void StartInfection()
    {
        //Debug.Log("infect him");
        /// Daca am placeHolder pentru cerc rosu sau verde pus
        if (PlaceHolderInfectedUI != null)
        {
            ui.imageDisplay.GetComponent<Image>().color = Color.red; // pun culoarea rosu pentru ca este infectat
        }
        infectionLevel = 1;// incep infectia
    }

    private void DrawUIPanel()
    {
        if (PlaceHolderInfectedUI != null /* && Vector3.Distance(transform.position,Player.Instance.mainCamera.transform.position) < 30f*/)
        {
            Vector3 desiredPos = transform.position + new Vector3(0, 10, 0);
            Vector3 actualPos = Camera.main.WorldToScreenPoint(desiredPos);
            if (actualPos.z > 0)
            {
                ui.panel.transform.position = actualPos;
                ui.panel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                ui.panel.SetActive(true);
            }
        }
    }
    private void OnMouseEnter()
    {
        DrawUIPanel();
    }

    private void OnMouseExit()
    {
        if (PlaceHolderInfectedUI != null)
        {
            ui.panel.SetActive(false);
        }
    }

    void TryInfectOtherBot()
    {
        foreach (Bot possibleTarget in Bot.listBots)
        {
            // daca botul possibleTarget nu este botul curent si daca nu e vindecat deja  
            if (possibleTarget == this || possibleTarget.cured == true)
                continue;
            // varianta noua bazata doar daca il vede bazat pe infectionDist care este setat in inspector
            // in viitor ar trebui ca infectDist sa fie o variabila statica cu o valaore definita pt toti botii
            if (possibleTarget.infectionLevel == 0 && CanSeeObject(transform, possibleTarget.transform, infectionDist, viewAngle))
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
            float coughVal = AIManager.instance.coughCurve.Evaluate(infectionLevel);// evaluate from gamemanager  function
            coughInterval = UnityEngine.Random.Range(coughVal - 1, coughVal);// add a bit of random 
        }
        // if the bot is not currently coughing
        if (animator.GetBool("cough") == false)
        {
            // daca coughTime > coughInterval ( a trecut timpul de asteptat) 
            if (currentCoughTime > coughInterval)
            {
                float coughVal = AIManager.instance.coughCurve.Evaluate(infectionLevel);// evaluate from a function
                coughInterval = UnityEngine.Random.Range(coughVal - 1, coughVal);// add a bit of random 
                currentCoughTime = 0; // reinitializez cronometrul          
                animator.SetTrigger("cough");// vezi animator 
                TryInfectOtherBot(); // doar cand tuseste incearca sa infecteze alt bot
            }
            currentCoughTime += Time.deltaTime;// cronometrul creste cu Time.deltaTime
        }
    }

    private void LateUpdate()
    {
        if (infectionLevel > 0)
        {

            Image image = new List<Image>(ui.panel.GetComponentsInChildren<Image>()).Find(img => img.type == Image.Type.Filled);
            image.fillAmount = infectionLevel / 10;
            Debug.Log(image.name);

            Cough();// tuseste
            if (currentInfectionTime > growthInterval) /// un minut
            {
                infectionLevel += infectionSpeed / immunityLevel; // creste invers proportional cu imunitatea
                infectionLevel = Mathf.Min(infectionLevel, 10); // infectia maxima e 10
                currentInfectionTime = 0;// resetez cronomoetrul de crescut infectia
            }
            else
                currentInfectionTime += Time.deltaTime;
        }
        if (infectionLevel > 0)
        {
            Cough();// tuseste
            if (currentInfectionTime > growthInterval) /// un minut
            {
                infectionLevel += infectionSpeed / immunityLevel; // creste invers proportional cu imunitatea
                infectionLevel = Mathf.Min(infectionLevel, 10); // infectia maxima e 10
                currentInfectionTime = 0;// resetez cronomoetrul de crescut infectia
            }
            else
                currentInfectionTime += Time.deltaTime;
        }
    }
    #endregion
    #region talking
    void EndMeeting(Bot bot)
    {
        bot.currentState = bot.lastState;
        // trebuie sa am grija sa resetez toate variabilele pentru amandoi botii, nu doar pentru unul dintr ei
        bot.obstacle.enabled = false;// obstacolul este fals
        bot.agent.enabled = true;
        bot.agent.isStopped = false;// ma asiugur ca navmeshagentul este activ si poate merge
        bot.inMeeting = false; // nu mai este in intalnire

        bot.talkDuration = 0; // intalnirea dureaza 0 secunde, la fel e doar pt initializare sa fie 0 in inspector
        bot.currentTalkTime = 0;// resetez cronometrul de vorbit

        bot.currentState = State.Patrol;
        bot.startPatroling = false; // inapoi la starea initiala
        bot.talking = false; // numai vorbeste

        bot.lastFinishedMeetingTime = Time.time;  // intalnirea s-a terminat la timpul Time.time(cate secunde au trecut de la inceputul jocului)
        bot.animator.SetBool("talking", false);
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
    private void OnAnimatorIK(int layerIndex)
    {
        if (talking == true)
        {
            animator.SetLookAtPosition(meetingBot.head.transform.position);
            animator.SetLookAtWeight(0.2f);
        }
    }
    #endregion

    void EndAction()
    {
        Debug.Log("<color=red>ended action of " + actionName + "</color>");
        lastFinishedAnyActionTime = Time.time;
        agent.isStopped = false;
        startAction = false;
        actionPathPending = false;
        lastPlace.occupied = false;
        lastPlace.bot = null;
    }
    IEnumerator AbstractCoroutine(System.Action OnStart, System.Action OnEnd, float waitTime)
    {
        OnStart?.Invoke();
        agent.isStopped = true;
        yield return new WaitForSeconds(waitTime);
        agent.isStopped = false;
        OnEnd?.Invoke();
        EndAction();
    }

    public static void RandomShuffle<T>(IList<T> lista)
    {
        int n = lista.Count;
        while (n > 0)
        {
            n--;
            int temp = UnityEngine.Random.Range(0, n);
            T value = lista[temp];
            lista[temp] = lista[n];
            lista[n] = value;
        }
    }
    bool TryFindNewAction()
    {
        // if there is are any possible actions and there is no other action pending
        if (actionList.Count > 0 && actionPathPending == false)
        {
            for (int i = 0; i < actionList.Count; i++)
                Debug.Assert(actionList[i] != null, "actiunea este nula"); // the action must not be null
            float random = UnityEngine.Random.value; // sa schimb in random mai incolo
            actionList.Sort(CompareAction);// sort the action list based on the probability

            for (int i = 0; i < actionList.Count; i++)
            {
                BotAction action = actionList[i];
                if (random < action.probability)// if the random satisfies
                {
                    Place place = action.place;
                    Debug.Log("trying to go to place <color=green> </color>" + place);
                    RandomShuffle(ActionPlace.dictionary[place]);// random shuffle the possible targets 
                    foreach (ActionPlace possiblePlace in ActionPlace.dictionary[place])
                    {
                        Debug.Assert(possiblePlace != null);// it must not be null
                        // the bot in there is null and is not occupied and is different from the last action
                        if (possiblePlace.occupied == false && possiblePlace.bot == null && possiblePlace != lastPlace)
                        {
                            currentState = State.AnyAction;// enter the any action state
                            possiblePlace.occupied = true; // occupy the action
                            possiblePlace.bot = this;
                            lastPlace = possiblePlace;
                            currentAction = action; // set the currentAction

                            currentDestination = action.position + possiblePlace.transform.position;// set the destination to the relative pos
                            agent.SetDestination(currentDestination);
                            Debug.Log("<color=red>botul : " + name + " a reusit sa intre in actiunea : " + currentAction.name + "</color>", this);
                            Debug.Log("<color=red>destinatia este</color>" + currentDestination, possiblePlace);
                            actionPathPending = true;
                            return true;
                        }
                    }
                }
                else
                {
                    //Debug.Log($"<color=red> " + action.name + "X</color>");
                }
            }
        }
        return false;
    }

    void Start()
    {

        currentState = State.Patrol;
        listBots.Add(this);

        cooldownMeeting = AIManager.instance.maxMeetingCooldown / sociableLevel;
        float ratie = (AIManager.instance.maxViewDistance - AIManager.instance.minViewDistance) / 9f;
        viewDistance = AIManager.instance.minViewDistance + (sociableLevel - 1) * ratie;

        agent = GetComponent<NavMeshAgent>(); // iau Navmesh Agentul
        animator = GetComponent<Animator>(); // iau animatorul

        obstacle = GetComponent<NavMeshObstacle>(); // iau obstacolul ca sa il activez atunci cand vorbesc si se opresc
        obstacle.enabled = false;

        agent.autoBraking = false; // sa nu se opreasca cand se aproprie de destinatie

        if (head == null)
            head = gameObject;
        if (PlaceHolderInfectedUI != null)
            ui = PlaceHolderInfectedUI.GetComponent<InfectedUI>();
        SetUpPosHolder();
        if (alreadyInfected == true)
        {
            StartInfection();
        }
        UnityEngine.Random.InitState(10);
        if (uiManager == null)
            uiManager = GetComponentInChildren<BotUI>();
        if (uiManager != null)
            uiManager.SetGizmosName(name);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                actionName = "Noname";
                if (agent.isOnNavMesh == false)
                    Debug.LogError("nu este pe navmesh", this);
                if (agent.isStopped == true)
                    agent.isStopped = false;
                if (agent.enabled == false)
                    agent.enabled = true;
                // in caz ca agentul intra in stare de patrol iar agent este oprit/dezactivat
                // aleg destinatia
                // folosesc startPatroling pentru a putea sa intru in starea de patrol la prima iteratie
                if (startPatroling == false || Vector3.Distance(a: transform.position, b: currentDestination) < stoppingDistance)
                {
                    if (randomLocations) // aleg pozitii random
                    {
                        Vector3 randomLocation = RandomLoc(); // o functie care returneaza o pozitie random valida in pe baza randomRange
                        currentDestination = randomLocation;
                        // folosesc currentDestination care este de tip Vector3 pentru a retine mereu care este destinatie curenta
                        // uneori agent.destination nu imi spune care este exact destinatia 
                        agent.SetDestination(target: currentDestination);
                    }
                    else
                    {
                        // aleg pozitiile din patrolPositions care este setat in inspector
                        agent.SetDestination(patrolPositions[currentIndexPatrol].transform.position);
                        currentDestination = patrolPositions[currentIndexPatrol].transform.position;// nu uit sa pun currentDestination;
                        currentIndexPatrol++; // cresc indexul din vector
                        if (currentIndexPatrol >= patrolPositions.Length)
                            currentIndexPatrol = 0; // resetez indexul
                    }
                    startPatroling = true;
                }

                // Tranzitia 1 in starea de MEET
                /// daca cooldown-ul a trecut de la ultima intalnire sau daca initial nu am avut nicio intalnire
                if (Time.time - lastFinishedMeetingTime >= cooldownMeeting || lastFinishedMeetingTime == 0)
                {
                    if (!cured || (cured == true && Vector3.Distance(transform.position, bedPosition) > 100f))
                    {   // daca nu e vindecat sau daca e vindecat si e departe de spital{
                        TryMeetBot(); /// incearca sa gaseasca partener 
                        if (inMeeting)
                            return;
                    }
                }
                /// Tranzitia 2 in starea de GOTOHOSPITAL
                if (infectionLevel > 5) // un anumit nivel
                {
                    // find a free hospital place
                    foreach (Hospital currentHospital in Hospital.listHospitals)
                    {
                        if (currentHospital.Free())
                        {
                            // tuple care reprezinta pozitia unde se afla patul si indexul din vectorul de paturi din clasa de Hospital
                            // folosesc si indexul pentru a putea "elibera" patul mai incolo, daca nu as retine si indexul ar fi mai greu
                            // clasa Hospital retine o lista cu toate spitalele din mapa
                            // de asemenea o lista de in care indentifica fiecare pozitie in spatiu(vector3) cu un index al patului
                            Tuple<Vector3, int> tuple = currentHospital.BedPosition();
                            if (tuple.Item2 != -1)
                            {
                                currentDestination = tuple.Item1;
                                bedIndex = tuple.Item2;
                                hospital = currentHospital;
                                bedPosition = currentDestination;
                                agent.SetDestination(target: currentDestination);
                                currentState = State.Hospital;
                                return;
                            }
                        }
                    }
                }
                // Tranzitia 3 in stareaFind Action
                Debug.Log("tranzitia 3");
                TryFindNewAction();

                break;
            case State.Meet:
                if (Vector3.Distance(a: transform.position, b: meetingPoint) < 1f)
                    agent.enabled = false;
                if (!agent.enabled && !meetingBot.agent.enabled)
                {
                    Debug.Log("Enabling obstacle for : " + name);
                    talking = true;
                    obstacle.enabled = true;
                    animator.SetBool(name: "talking", value: true);
                    Talk();
                }
                break;
            case State.Hospital:
                if (Vector3.Distance(a: transform.position, b: currentDestination) < 5f)
                {
                    infectionLevel = 0;
                    hospital.LeaveBed(index: bedIndex);
                    startPatroling = false;
                    cured = true;
                    currentState = State.Patrol;
                }
                break;
            case State.AnyAction:
                if (currentAction != null)
                    actionName = currentAction.name;
                if (startAction == false && actionPathPending == false)// if there is no action right now
                {
                    if (TryFindNewAction() == false) // if we fail to find any action then go to patrol
                    {
                        currentState = State.Patrol;
                        startPatroling = true;
                    }
                }
                if (actionPathPending == true)
                {
                    // if the bot is going towards the action destination
                    if (startAction == false && Vector3.Distance(transform.position, currentDestination) < currentAction.stopDistance + 0.1f)
                    {
                        startAction = true; // now the action starts!
                                            // snap to the disired position and rotation
                        transform.position = currentDestination;
                        transform.rotation = currentAction.rotation;
                        agent.isStopped = true;
                        System.Action start = null;
                        System.Action end = null;
                        int waitTime;

                        Debug.Log("<color=green> Start Action " + currentAction.name + "</color>");
                        if (currentAction.name == typingAction.name)
                        {
                            Debug.Log("<color=green>entered typing</color>");
                            start = () => animator.SetBool("typing", true);
                            end = () => animator.SetBool("typing", false);
                            waitTime = UnityEngine.Random.Range(10, 15);
                        }
                        else if (currentAction.name == lookAtSphereAction.name)
                        {
                            Debug.Log("<color=green>entered looking</color>");
                            start = () =>
                            {
                                transform.LookAt(currentAction.targetTransform);
                            };
                            waitTime = 5;
                        }
                        else
                        {
                            Debug.Assert(currentAction.name != "typingAction" && currentAction.name != "lookAtSphereAction");
                            Debug.Log("<color=green>entered other</color>", this);
                            waitTime = 8;
                        }
                        StartCoroutine(AbstractCoroutine(start, end, waitTime));

                    }
                }
                break;
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
                Gizmos.DrawSphere(agent.destination, 0.2f);
                Gizmos.color = Color.magenta;
            }
            else
            {
                Gizmos.DrawLine(transform.position, agent.destination);
                Gizmos.DrawSphere(agent.destination, 0.2f);
            }
        }
        //Debug.Log("event has in gizmos for " + name + " : " + GetListenerNumber(loadEvent) + "listeners");
        if (drawLines)
            DrawLineOfSight();

    }
}

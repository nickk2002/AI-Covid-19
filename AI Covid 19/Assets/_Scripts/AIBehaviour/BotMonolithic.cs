using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Covid19.AIBehaviour.Actions;
using Covid19.AIBehaviour.Animations;
using Covid19.AIBehaviour.SaveData;
using Covid19.AIBehaviour.StateMachine;
using Covid19.AIBehaviour.UI;
using Covid19.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Covid19.AIBehaviour
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AnimateAI))]
    // the core component of the AI 400+ lines of code, so it will take a while to read everything
    public class BotMonolithic : MonoBehaviour
    {
        #region public vars

        public static List<BotMonolithic> ListBotMonolithics = new List<BotMonolithic>();

        [FormerlySerializedAs("_actionName")] [ShowOnly] [SerializeField]
        private string actionName = "No Action";

        [NonSerialized] public List<BotAction> ReflectionActions /*{ get; private set; }*/ = new List<BotAction>();
        [NonSerialized] public List<string> ReflectionFields /*{ get; private set; }*/ = new List<string>();
        [HideInInspector] public List<bool> toggleList = new List<bool>();
        [HideInInspector] public List<BotAction> actionList = new List<BotAction>();

        [FormerlySerializedAs("_currentState")] [ShowOnly] [SerializeField]
        private State currentState;

        [FormerlySerializedAs("_washHandsAction")]
        public BotAction washHandsAction = new BotAction { };

        [FormerlySerializedAs("_typingAction")]
        public BotAction typingAction = new BotAction { };

        [FormerlySerializedAs("_lookAtSphereAction")]
        public BotAction lookAtSphereAction = new BotAction { };

        [FormerlySerializedAs("_actiuneAction")]
        public BotAction actiuneAction = new BotAction { };

        public Vector3 CurrentDestination { get; set; }
        public GameObject[] PatrolPositions => patrolPositions;
        public float RandomRange => randomRange;
        public float StoppingDistance => stoppingDistance;
        public bool RandomLocations => randomLocations;

        [Header("General")] [Tooltip("This is setting can be used for play testing")]
        // I use header to make a nice looking inspector, and SerializeField to make private variables be visible in the inspector
        public bool
            alreadyInfected; // if this option is enabled in the inspector the bot will start the game already infected

        [FormerlySerializedAs("_head")] [SerializeField]
        private GameObject head; // gameobject for head used for detecting if two bots see each other

        [FormerlySerializedAs("_sociableLevel")]
        [Range(0, 10)] // a cool feature for modifying in the inspector with a slider
        [SerializeField]
        private int sociableLevel; // how sociable is a bot from 0 to 10, 0 = avoids contact, 10 = very sociable

        [FormerlySerializedAs("_immunityLevel")] [Range(1, 10)] [SerializeField]
        private int
            immunityLevel; // how immune is a bot from a scale from 1 to 10, if this agent is already infected then the ilness will

        // develop differently according to the immunityLevel, if it is low(1,2 for ex), then the virus will grow fast, if the immunityLevel is 10
        // then the infection will grow slow
        [FormerlySerializedAs("_cautiousLevel")] [SerializeField]
        private int
            cautiousLevel; // how aware he is of the virus -> he avoids others and goes to the doctor when there are symtomps

        [FormerlySerializedAs("_infectionDist")] [SerializeField]
        private float infectionDist = 10; // if this bot coughs, the distance of the virus transmission

        // level 1 means that he behaves normal, he does nothing special to protect him, level 10 -> keeps distance between others.and goes to the doctor
        // as soon as he has symptoms of the illness (coughing at the moment)
        // level 5 is in the middle
        // Quick recap :)
        // sociability -> Influences the probability of two bots meeting.
        // Two bots can meet under two conditions : they see each other and their sociableLevel is taken into a probability (see TryMeetBotMonolithic)
        // Immunity -> how fast/slow the disease develops
        [FormerlySerializedAs("_infectionLevel")] [ShowOnly]
        public float infectionLevel; // from 1 to 10

        // you create an empty object with any name you want, then create empty children that represent actual patrol positions
        // assign you empty object to posHolder in the inspector (see BasicScene.unity for details)
        [FormerlySerializedAs("_randomLocations")] [Header("Patrol Settings")] [SerializeField]
        private bool randomLocations = false; // if he uses randomLocations

        [FormerlySerializedAs("_randomRange")] [SerializeField]
        private float randomRange = 10f; // the range of the random

        [FormerlySerializedAs("_stoppingDistance")] [SerializeField]
        private float stoppingDistance = 3f; // the distance in which the agent stops and moves to the next position

        [FormerlySerializedAs("_patrolPositions")] [SerializeField]
        private GameObject[] patrolPositions; // array holding patrol positions

        [FormerlySerializedAs("_posHolder")] [SerializeField]
        private GameObject posHolder; // This is used for an easier way to set patrol points

        [FormerlySerializedAs("_infectionSpeed")] [Header("Infection")] [SerializeField]
        private float infectionSpeed = 0.1f;

        [FormerlySerializedAs("_growthInterval")] [SerializeField]
        private float growthInterval = 3;

        /// infection grows by 1 unit every three seconds
        [ShowOnly] public bool cured = false;

        [FormerlySerializedAs("_offsetMeeting")] [Header("Meeting Settings")] [SerializeField]
        private float offsetMeeting = 5; // an offset which says the distance between them when they are talking

        [FormerlySerializedAs("_cooldownMeeting")] [SerializeField]
        private float
            cooldownMeeting =
                60; // after this bot has left the meeting he has a cooldown untill he finds another meeting

        // TODO : make cooldownMeeting be determined by sociableLevel ( rezolvat in Start)
        // is set by the AIManger initially, can be modified in inspector
        [FormerlySerializedAs("_inMeeting")] [ShowOnly] [SerializeField]
        private bool inMeeting = false; // if this bot is currently on the way to meet another bot

        [FormerlySerializedAs("_talking")] [ShowOnly] [SerializeField]
        private bool talking = false; // if the bot is talking (no animations yet)

        [FormerlySerializedAs("_meetingBotMonolithic")] [ShowOnly] [SerializeField]
        private BotMonolithic
            meetingBotMonolithic; // shows in the inspector who is the dialog partner, there is no need to modify it in the inspector

        [FormerlySerializedAs("_talkDuration")] [Header("Talk Seetings")] [ShowOnly] [SerializeField]
        private int talkDuration = 0; // talk duration

        [FormerlySerializedAs("_currentTalkTime")] [ShowOnly] [SerializeField]
        private float currentTalkTime = 0; // the current elasped time of talking (here for debugging)


        [FormerlySerializedAs("_coughInterval")] [Header("Coughing")] [ShowOnly] [SerializeField]
        private float coughInterval; // random value between [maxCoughInverval,minCoughInterval]

        [FormerlySerializedAs("_currentCoughTime")] [ShowOnly] [SerializeField]
        private float currentCoughTime = 0; // do not change

        [FormerlySerializedAs("_viewAngle")] [Header("BotMonolithic view")] [ShowOnly] [SerializeField]
        private int viewAngle = 60;

        /// the angle a bot can see
        [FormerlySerializedAs("_viewDistance")] [ShowOnly] [SerializeField]
        private float
            viewDistance = 30; // the distance(TODO : sociabalLevel should influence viewDistance) (rezolvat in Start())

        [FormerlySerializedAs("_drawLines")] [SerializeField]
        private bool drawLines = false; // if you want to draw in the scene the line of sight for each bot

        [FormerlySerializedAs("_realSightView")] [SerializeField]
        private bool
            realSightView =
                false; // if you want to draw in the scene the accurate line of sight(influenced by walls etc)
        // if bot1 coughs and bot2 is in 10m distance then bot2 gets infected too

        [FormerlySerializedAs("_uiManager")] [Header("UI")] [SerializeField]
        private BotUI uiManager;

        [FormerlySerializedAs("_placeHolderInfectedUI")] [SerializeField]
        private GameObject placeHolderInfectedUI; // for the circle UI

        #endregion

        private State _lastState = State.None;
        private NavMeshObstacle _obstacle;
        private Animator _animator; // animator

        private NavMeshAgent
            _agent; // navmeshagent (if you don't know much about it it's easy) . all the functions are intuitive

        private int _currentIndexPatrol = 0;
        private Vector3 _currentDestination;

        private Vector3 _meetingPoint;
        private List<Tuple<BotMonolithic, float>> _listIgnoredBotMonolithics = new List<Tuple<BotMonolithic, float>>();

        private bool _startPatroling;

        private int _bedIndex;
        private Vector3 _bedPosition;
        private Hospital _hospital;
        private ActionPlace _lastPlace;

        private float _currentInfectionTime = 0;
        private float _lastFinishedMeetingTime = 0;
        private float _lastFinishedAnyActionTime = 0;
        private BotAction _currentAction = null;

        private bool _startAction;
        private bool _actionPathPending;

        private InfectedUI _ui;

        public static int CountNumberInfected()
        {
            var cnt = 0;
            foreach (var bot in ListBotMonolithics)
                if (bot.infectionLevel > 0)
                    cnt++;
            return cnt;
        }

        public static void ClearBotMonolithics()
        {
            ListBotMonolithics.Clear();
        }

        #region actionList save/update add clear

        public void ClearList()
        {
            actionList.Clear();
        }

        public void SetUpReflection()
        {
            if (toggleList.Count < ReflectionActions.Count)
            {
                var dif = ReflectionActions.Count - toggleList.Count;
                for (var i = 1; i <= dif; i++) toggleList.Add(false);
            }

            foreach (var field in typeof(BotMonolithic).GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default))
            {
                var nume = field.Name;
                if (nume.EndsWith("Action") && field.FieldType == typeof(BotAction) && field.IsPrivate == false)
                {
                    var action = field.GetValue(this) as BotAction;
                    action.name = nume;
                    field.SetValue(this, action);

                    if (ReflectionActions.Contains(action) == false) ReflectionActions.Add(action);
                }
                else
                {
                    if (ReflectionFields.Contains(nume) == false)
                        ReflectionFields.Add(nume);
                }
            }

            Debug.Log($"There are {ReflectionActions.Count} actions");
        }

        public void SavePreset()
        {
            SaveSystem.SaveBotAsJson(ReflectionActions);
            foreach (var bot in FindObjectsOfType<BotMonolithic>()) bot.UpdateGizmosUI(false);
        }

        public void UpdatePreset(List<BotAction> actionList = null)
        {
            if (UnityEditor.EditorApplication.isPlaying)
                return;
            if (actionList == null) // updates from JSON
                ReflectionActions = SaveSystem.LoadBotActions();
            else
                ReflectionActions = actionList;
            var fields = typeof(BotMonolithic).GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                         BindingFlags.Public | BindingFlags.Default);
            var i = 0;
            foreach (var field in fields)
                if (field.Name.EndsWith("Action") && field.FieldType == typeof(BotAction) && field.IsPrivate == false)
                {
                    UnityEditor.Undo.RecordObject(gameObject, "descriptive name of this operation");
                    var action = field.GetValue(this) as BotAction;
                    field.SetValue(this, ReflectionActions[i++]);
                    UpdateGizmosUI(true);

                    UnityEditor.Undo.RecordObject(gameObject, "descriptive name of this operation");
                    UnityEditor.EditorUtility.SetDirty(gameObject);
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
        }

        public void UpdateAllOthers()
        {
            var list = FindObjectsOfType<BotMonolithic>();
            foreach (var item in list)
                if (item != null && item != this)
                    item.UpdatePreset(ReflectionActions);
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
            if (actionList.Find(x => x == action) == null) actionList.Add(action);
        }

        public void RemoveAction(BotAction action)
        {
            if (actionList.Count == 0) return;
            if (actionList.Contains(action)) actionList.Remove(action);
        }

        private int CompareAction(BotAction a, BotAction b)
        {
            // pt a compara botii a si b in functie de distanta. returneaza bot-ul cel mai apropiat de cel curent
            var pa = a.probability;
            var pb = b.probability;
            if (pa == pb)
                return 0;
            if (pa < pb)
                return -1; // botul a
            else
                return 1; // botul b
        }

        #endregion

        private void SetUpPosHolder()
        {
            if (posHolder != null)
            {
                if (posHolder.transform.childCount == 0
                ) // daca se intampla ca cineva sa puna un obiect aleator ca si PosHolder care nu are copii
                    Debug.LogError("Pos holder of bot : " + name + "has no other children");

                patrolPositions =
                    new GameObject[posHolder.transform
                        .childCount]; // initializez vectorul de pozitii cu cati copii are posHolder
                var i = 0;
                foreach (Transform child in posHolder.transform
                ) // iterez prin fiecare copil din posHolder.transform (copii->transform)
                {
                    patrolPositions[i] =
                        child.gameObject; // vectorul de pozitii retine gameobject-uri asa ca .gameobject
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


        #region meeting

        private bool CanSeeObject(Transform initial, Transform target, float dist, float angle)
        {
            var diferenta = target.position - initial.position;
            RaycastHit hit;
            if (Vector3.Distance(initial.position, target.position) <= dist)
                if (Vector3.Angle(initial.forward, diferenta) <= angle / 2)
                {
                    if (Physics.Raycast(initial.position, diferenta, out hit))
                    {
                        if (hit.collider.gameObject.GetComponent<BotMonolithic>() != null) return true;
                    }
                    else
                    {
                        return true;
                    }
                }

            return false;
        }

        private void PrepareForMeeting(BotMonolithic bot, BotMonolithic other, Vector3 currentMeetingPosition,
            int meetingDuration)
        {
            bot._lastState = bot.currentState;
            bot.currentState = State.Meet;
            // pregatesc pentru intalnire botul bot si botul other pentru intalnire
            bot._agent.isStopped = false; // ma asigur ca nu sunt opriti
            bot._agent.destination = currentMeetingPosition; // pun destinatia
            bot._meetingPoint =
                currentMeetingPosition; // spun meeting point de vazut in inspector, nu fac nimic din cod cu el
            bot.inMeeting = true; // sunt in drum spre intalnire
            bot.meetingBotMonolithic = other; // cu cine se intalneste
            bot.talkDuration = meetingDuration; // cat timp dureaza intalnirea
        }

        private int CompareBotMonolithic(BotMonolithic a, BotMonolithic b)
        {
            // pt a compara botii a si b in functie de distanta. returneaza bot-ul cel mai apropiat de cel curent
            var dista = Vector3.Distance(transform.position, a.gameObject.transform.position);

            var distb = Vector3.Distance(transform.position, b.gameObject.transform.position);
            if (dista == distb)
                return 0;
            if (dista < distb)
                return -1; // botul a
            else
                return 1; // botul b
        }

        private void TryMeetBotMonolithic()
        {
            var possibleBotMonolithics = new List<BotMonolithic>(); /// lista cu toti botii posibili
            foreach (var partnerBotMonolithic in ListBotMonolithics)
            {
                if (_listIgnoredBotMonolithics.Find(bot => bot.Item1 == partnerBotMonolithic) != null
                ) // daca botul partner bot este ignorat
                {
                    var startIgnoringTime =
                        _listIgnoredBotMonolithics.Find(bot => bot.Item1 == partnerBotMonolithic).Item2;
                    /// daca sa zicem bot1 si bot2 incearca sa se intalneasca dar nu reusesc din cauza probabilitatii
                    ///  atunci se ignora timp de 10 pana cand pot sa mai incerce inca o data sa se intalneasca
                    ///  10 secunde e timp standard,
                    if (Time.time - startIgnoringTime < 10f)
                        continue;
                    else
                        _listIgnoredBotMonolithics.Remove(Tuple.Create(partnerBotMonolithic, startIgnoringTime));
                }

                /// daca botul pe care il caut este diferit de botul curent si daca botul curent nu este in meeting si 
                /// daca partnetBotMonolithic la fel nu este in meeting il adaug la lista de boti posibili
                if (partnerBotMonolithic != this && !inMeeting && !partnerBotMonolithic.inMeeting
                    && CanSeeObject(head.transform, partnerBotMonolithic.head.transform, viewDistance, viewAngle)
                    && CanSeeObject(partnerBotMonolithic.head.transform, head.transform,
                        partnerBotMonolithic.viewDistance, partnerBotMonolithic.viewAngle))
                {
                    Debug.Log("<color=red>Can see each other</color>");
                    possibleBotMonolithics.Add(partnerBotMonolithic);
                }
            }

            if (possibleBotMonolithics.Count == 0)
                return; /// nu a vazut pe nimeni care sa fie disponibil
            Debug.Log("sunt : " + possibleBotMonolithics.Count);
            possibleBotMonolithics.Sort(CompareBotMonolithic); /// sortez botii dupa distanta

            foreach (var partnerBotMonolithic in possibleBotMonolithics)
                /// daca nu sunt meeting nici botul curent nici partnerbot si daca celalat bot nu l-a ignorat pe cel curent
                /// adica daca partnerbot.lista nu are un tuple care sa aiba botul curent ca si item1
                if (!inMeeting && !partnerBotMonolithic.inMeeting &&
                    partnerBotMonolithic._listIgnoredBotMonolithics.Find(bot => this == bot.Item1) == null &&
                    currentState != State.AnyAction && partnerBotMonolithic.currentState != State.AnyAction)
                {
                    var randomValueBetweenRange =
                        UnityEngine.Random.Range(1,
                            10); // trebuie verificat sa fie la fel cu Range[1,10] sau daca e modificat in viitor
                    // daca randomul este mai mic decat nivelul social al primului si este mai mic decat nivelul social al ceiluilat
                    // atunci se pot intalni
                    if (randomValueBetweenRange <= sociableLevel &&
                        randomValueBetweenRange <= partnerBotMonolithic.sociableLevel)
                    {
                        //Debug.Log("se intalnesc cei doi boti: " + this.name + "si" + partnerBotMonolithic.name);

                        var diferenta =
                            partnerBotMonolithic.transform.position -
                            transform.position; /// vectorul de la botul curent la celelalt
                        diferenta /= 2; // jumatatea vectorului
                        var meetingPointHalf = transform.position + diferenta; // punctul de intalnire la jumatate
                        var offset = diferenta.normalized * offsetMeeting / 2; // offset pt inalnire

                        var average = (sociableLevel + partnerBotMonolithic.sociableLevel) / 2;
                        var sum = sociableLevel + partnerBotMonolithic.sociableLevel;
                        var randomTime =
                            UnityEngine.Random.Range(average,
                                sum); // aleg un numar random de secunde pentru timpul de intalnire
                        // this-> bot curent, partnetBotMonolithic-> botul cu care se intalneste
                        PrepareForMeeting(this, partnerBotMonolithic, meetingPointHalf - offset,
                            randomTime); // pregatesc botul curent pentru intalnire
                        PrepareForMeeting(partnerBotMonolithic, this, meetingPointHalf + offset,
                            randomTime); // pregatesc celelalt bot pentru intalnire
                        break; // o data ce am gasit un partener de intalnire atunci inchid pentru ca numai are sens sa caut altul.
                    }
                    else
                    {
                        _listIgnoredBotMonolithics.Add(Tuple.Create(partnerBotMonolithic, Time.time));
                        // ignor botul pentru un anumit timp, retin timpul la care a inceput sa fie ignorat
                    }
                }
        }

        #endregion

        #region infect + ui

        public void StartInfection()
        {
            //Debug.Log("infect him");
            /// Daca am placeHolder pentru cerc rosu sau verde pus
            if (placeHolderInfectedUI != null)
                _ui.imageDisplay.GetComponent<Image>().color = Color.red; // pun culoarea rosu pentru ca este infectat
            infectionLevel = 1; // incep infectia
        }

        private void DrawUIPanel()
        {
            if (
                placeHolderInfectedUI !=
                null /* && Vector3.Distance(transform.position,Player.Instance.mainCamera.transform.position) < 30f*/)
            {
                var desiredPos = transform.position + new Vector3(0, 10, 0);
                var actualPos = Camera.main.WorldToScreenPoint(desiredPos);
                if (actualPos.z > 0)
                {
                    _ui.panel.transform.position = actualPos;
                    _ui.panel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    _ui.panel.SetActive(true);
                }
            }
        }

        private void OnMouseEnter()
        {
            DrawUIPanel();
        }

        private void OnMouseExit()
        {
            if (placeHolderInfectedUI != null) _ui.panel.SetActive(false);
        }

        private Vector3 RandomLoc()
        {
            // locatie random pentru mapa mare.de 1000 * 1000 Scena : AISimulations
            NavMeshHit hit;
            for (var i = 0; i < 30; i++)
            {
                var randomPosition = new Vector3(UnityEngine.Random.Range(-randomRange, randomRange), 0,
                    UnityEngine.Random.Range(-randomRange, randomRange));
                if (NavMesh.SamplePosition(randomPosition, out hit, 1, NavMesh.AllAreas)) return hit.position;
            }

            Debug.Log("Did not find anything");
            return new Vector3(UnityEngine.Random.Range(-randomRange, randomRange), 0,
                UnityEngine.Random.Range(-randomRange, randomRange));
        }

        private void TryInfectOtherBotMonolithic()
        {
            foreach (var possibleTarget in ListBotMonolithics)
            {
                // daca botul possibleTarget nu este botul curent si daca nu e vindecat deja  
                if (possibleTarget == this || possibleTarget.cured)
                    continue;
                // varianta noua bazata doar daca il vede bazat pe infectionDist care este setat in inspector
                // in viitor ar trebui ca infectDist sa fie o variabila statica cu o valaore definita pt toti botii
                if (possibleTarget.infectionLevel == 0 &&
                    CanSeeObject(transform, possibleTarget.transform, infectionDist, viewAngle))
                    possibleTarget.StartInfection();
            }
        }

        private void Cough()
        {
            if (_animator == null)
            {
                Debug.LogWarning("Nu am animator");
                return;
            }

            // mecanica de tusit
            // daca coughInterval este 0, adica inainte era 0 acum il initalizez cu un random intre valmin si valmax
            if (coughInterval == 0)
            {
                var coughVal =
                    AIManager.Instance.coughCurve.Evaluate(infectionLevel); // evaluate from gamemanager  function
                coughInterval = UnityEngine.Random.Range(coughVal - 1, coughVal); // add a bit of random 
            }

            // if the bot is not currently coughing
            if (_animator.GetBool("cough") == false)
            {
                // daca coughTime > coughInterval ( a trecut timpul de asteptat) 
                if (currentCoughTime > coughInterval)
                {
                    var coughVal = AIManager.Instance.coughCurve.Evaluate(infectionLevel); // evaluate from a function
                    coughInterval = UnityEngine.Random.Range(coughVal - 1, coughVal); // add a bit of random 
                    currentCoughTime = 0; // reinitializez cronometrul          
                    _animator.SetTrigger("cough"); // vezi animator 
                    TryInfectOtherBotMonolithic(); // doar cand tuseste incearca sa infecteze alt bot
                }

                currentCoughTime += Time.deltaTime; // cronometrul creste cu Time.deltaTime
            }
        }

        private void LateUpdate()
        {
            if (infectionLevel > 0)
            {
                var image = new List<Image>(_ui.panel.GetComponentsInChildren<Image>()).Find(img =>
                    img.type == Image.Type.Filled);
                image.fillAmount = infectionLevel / 10;
                Debug.Log(image.name);

                Cough(); // tuseste
                if (_currentInfectionTime > growthInterval) /// un minut
                {
                    infectionLevel += infectionSpeed / immunityLevel; // creste invers proportional cu imunitatea
                    infectionLevel = Mathf.Min(infectionLevel, 10); // infectia maxima e 10
                    _currentInfectionTime = 0; // resetez cronomoetrul de crescut infectia
                }
                else
                {
                    _currentInfectionTime += Time.deltaTime;
                }
            }

            if (infectionLevel > 0)
            {
                Cough(); // tuseste
                if (_currentInfectionTime > growthInterval) /// un minut
                {
                    infectionLevel += infectionSpeed / immunityLevel; // creste invers proportional cu imunitatea
                    infectionLevel = Mathf.Min(infectionLevel, 10); // infectia maxima e 10
                    _currentInfectionTime = 0; // resetez cronomoetrul de crescut infectia
                }
                else
                {
                    _currentInfectionTime += Time.deltaTime;
                }
            }
        }

        #endregion

        #region talking

        private void EndMeeting(BotMonolithic bot)
        {
            bot.currentState = bot._lastState;
            // trebuie sa am grija sa resetez toate variabilele pentru amandoi botii, nu doar pentru unul dintr ei
            bot._obstacle.enabled = false; // obstacolul este fals
            bot._agent.enabled = true;
            bot._agent.isStopped = false; // ma asiugur ca navmeshagentul este activ si poate merge
            bot.inMeeting = false; // nu mai este in intalnire

            bot.talkDuration = 0; // intalnirea dureaza 0 secunde, la fel e doar pt initializare sa fie 0 in inspector
            bot.currentTalkTime = 0; // resetez cronometrul de vorbit

            bot.currentState = State.Patrol;
            bot._startPatroling = false; // inapoi la starea initiala
            bot.talking = false; // numai vorbeste

            bot._lastFinishedMeetingTime =
                Time.time; // intalnirea s-a terminat la timpul Time.time(cate secunde au trecut de la inceputul jocului)
            bot._animator.SetBool("talking", false);
        }

        private void Talk()
        {
            if (currentTalkTime > talkDuration)
            {
                EndMeeting(this); // resetez totul pentru botul curent
                EndMeeting(meetingBotMonolithic); // resetez totul pentru botul cu care se intalneste
            }
            else
            {
                currentTalkTime += Time.deltaTime;
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (talking)
            {
                _animator.SetLookAtPosition(meetingBotMonolithic.head.transform.position);
                _animator.SetLookAtWeight(0.2f);
            }
        }

        #endregion

        private void EndAction()
        {
            Debug.Log("<color=red>ended action of " + actionName + "</color>");
            _lastFinishedAnyActionTime = Time.time;
            _agent.isStopped = false;
            _startAction = false;
            _actionPathPending = false;
            _lastPlace.occupied = false;
            _lastPlace.bot = null;
        }

        private IEnumerator AbstractCoroutine(Action onStart, Action onEnd, float waitTime)
        {
            onStart?.Invoke();
            _agent.isStopped = true;
            yield return new WaitForSeconds(waitTime);
            _agent.isStopped = false;
            onEnd?.Invoke();
            EndAction();
        }

        public static void RandomShuffle<T>(IList<T> lista)
        {
            var n = lista.Count;
            while (n > 0)
            {
                n--;
                var temp = UnityEngine.Random.Range(0, n);
                var value = lista[temp];
                lista[temp] = lista[n];
                lista[n] = value;
            }
        }

        private bool TryFindNewAction()
        {
            // if there is are any possible actions and there is no other action pending
            if (actionList.Count > 0 && _actionPathPending == false)
            {
                for (var i = 0; i < actionList.Count; i++)
                    Debug.Assert(actionList[i] != null, "actiunea este nula"); // the action must not be null
                var random = UnityEngine.Random.value; // sa schimb in random mai incolo
                actionList.Sort(CompareAction); // sort the action list based on the probability

                for (var i = 0; i < actionList.Count; i++)
                {
                    var action = actionList[i];
                    if (random < action.probability) // if the random satisfies
                    {
                        var place = action.place;
                        Debug.Log("trying to go to place <color=green> </color>" + place);
                        RandomShuffle(ActionPlace.Dictionary[place]); // random shuffle the possible targets 
                        foreach (var possiblePlace in ActionPlace.Dictionary[place])
                        {
                            Debug.Assert(possiblePlace != null); // it must not be null
                            // the bot in there is null and is not occupied and is different from the last action
                            if (possiblePlace.occupied == false && possiblePlace.bot == null &&
                                possiblePlace != _lastPlace)
                            {
                                currentState = State.AnyAction; // enter the any action state
                                possiblePlace.occupied = true; // occupy the action
                                //possiblePlace.bot = this;
                                _lastPlace = possiblePlace;
                                _currentAction = action; // set the currentAction

                                CurrentDestination =
                                    action.position +
                                    possiblePlace.transform.position; // set the destination to the relative pos
                                _agent.SetDestination(CurrentDestination);
                                Debug.Log(
                                    "<color=red>botul : " + name + " a reusit sa intre in actiunea : " +
                                    _currentAction.name + "</color>", this);
                                Debug.Log("<color=red>destinatia este</color>" + CurrentDestination, possiblePlace);
                                _actionPathPending = true;
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

        private FiniteStateMachine _stateMachine;

        private void Start()
        {
            currentState = State.Patrol;
            ListBotMonolithics.Add(this);

            cooldownMeeting = AIManager.Instance.maxMeetingCooldown / sociableLevel;
            var ratie = (AIManager.Instance.maxViewDistance - AIManager.Instance.minViewDistance) / 9f;
            viewDistance = AIManager.Instance.minViewDistance + (sociableLevel - 1) * ratie;

            _agent = GetComponent<NavMeshAgent>(); // iau Navmesh Agentul
            _animator = GetComponent<Animator>(); // iau animatorul

            _obstacle =
                GetComponent<NavMeshObstacle>(); // iau obstacolul ca sa il activez atunci cand vorbesc si se opresc
            _obstacle.enabled = false;

            _agent.autoBraking = false; // sa nu se opreasca cand se aproprie de destinatie

            if (head == null)
                head = gameObject;
            if (placeHolderInfectedUI != null)
                _ui = placeHolderInfectedUI.GetComponent<InfectedUI>();
            SetUpPosHolder();
            if (alreadyInfected) StartInfection();
            UnityEngine.Random.InitState(10);
            if (uiManager == null)
                uiManager = GetComponentInChildren<BotUI>();
            if (uiManager != null)
                uiManager.SetGizmosName(name);
        }

        // Update is called once per frame
        private void Update()
        {
            //stateMachine.Update();
            switch (currentState)
            {
                case State.Patrol:
                    actionName = "Noname";
                    if (_agent.isOnNavMesh == false)
                        Debug.LogError("nu este pe navmesh", this);
                    if (_agent.isStopped)
                        _agent.isStopped = false;
                    if (_agent.enabled == false)
                        _agent.enabled = true;
                    // in caz ca agentul intra in stare de patrol iar agent este oprit/dezactivat
                    // aleg destinatia
                    // folosesc startPatroling pentru a putea sa intru in starea de patrol la prima iteratie
                    if (_startPatroling == false ||
                        Vector3.Distance(transform.position, CurrentDestination) < stoppingDistance)
                    {
                        if (randomLocations) // aleg pozitii random
                        {
                            var randomLocation =
                                RandomLoc(); // o functie care returneaza o pozitie random valida in pe baza randomRange
                            CurrentDestination = randomLocation;
                            // folosesc currentDestination care este de tip Vector3 pentru a retine mereu care este destinatie curenta
                            // uneori agent.destination nu imi spune care este exact destinatia 
                            _agent.SetDestination(CurrentDestination);
                        }
                        else
                        {
                            // aleg pozitiile din patrolPositions care este setat in inspector
                            _agent.SetDestination(patrolPositions[_currentIndexPatrol].transform.position);
                            CurrentDestination =
                                patrolPositions[_currentIndexPatrol].transform
                                    .position; // nu uit sa pun currentDestination;
                            _currentIndexPatrol++; // cresc indexul din vector
                            if (_currentIndexPatrol >= patrolPositions.Length)
                                _currentIndexPatrol = 0; // resetez indexul
                        }

                        _startPatroling = true;
                    }

                    // Tranzitia 1 in starea de MEET
                    /// daca cooldown-ul a trecut de la ultima intalnire sau daca initial nu am avut nicio intalnire
                    if (Time.time - _lastFinishedMeetingTime >= cooldownMeeting || _lastFinishedMeetingTime == 0)
                        if (!cured || cured && Vector3.Distance(transform.position, _bedPosition) > 100f)
                        {
                            // daca nu e vindecat sau daca e vindecat si e departe de spital{
                            TryMeetBotMonolithic(); /// incearca sa gaseasca partener 
                            if (inMeeting)
                                return;
                        }

                    /// Tranzitia 2 in starea de GOTOHOSPITAL
                    if (infectionLevel > 5) // un anumit nivel
                        // find a free hospital place
                        foreach (var currentHospital in Hospital.ListHospitals)
                            if (currentHospital.Free())
                            {
                                // tuple care reprezinta pozitia unde se afla patul si indexul din vectorul de paturi din clasa de Hospital
                                // folosesc si indexul pentru a putea "elibera" patul mai incolo, daca nu as retine si indexul ar fi mai greu
                                // clasa Hospital retine o lista cu toate spitalele din mapa
                                // de asemenea o lista de in care indentifica fiecare pozitie in spatiu(vector3) cu un index al patului
                                var tuple = currentHospital.BedPosition();
                                if (tuple.Item2 != -1)
                                {
                                    CurrentDestination = tuple.Item1;
                                    _bedIndex = tuple.Item2;
                                    _hospital = currentHospital;
                                    _bedPosition = CurrentDestination;
                                    _agent.SetDestination(CurrentDestination);
                                    currentState = State.Hospital;
                                    return;
                                }
                            }

                    // Tranzitia 3 in stareaFind Action
                    Debug.Log("tranzitia 3");
                    TryFindNewAction();

                    break;
                case State.Meet:
                    if (Vector3.Distance(transform.position, _meetingPoint) < 1f)
                        _agent.enabled = false;
                    if (!_agent.enabled && !meetingBotMonolithic._agent.enabled)
                    {
                        Debug.Log("Enabling obstacle for : " + name);
                        talking = true;
                        _obstacle.enabled = true;
                        _animator.SetBool("talking", true);
                        Talk();
                    }

                    break;
                case State.Hospital:
                    if (Vector3.Distance(transform.position, CurrentDestination) < 5f)
                    {
                        infectionLevel = 0;
                        _hospital.LeaveBed(_bedIndex);
                        _startPatroling = false;
                        cured = true;
                        currentState = State.Patrol;
                    }

                    break;
                case State.AnyAction:
                    if (_currentAction != null)
                        actionName = _currentAction.name;
                    if (_startAction == false && _actionPathPending == false) // if there is no action right now
                        if (TryFindNewAction() == false) // if we fail to find any action then go to patrol
                        {
                            currentState = State.Patrol;
                            _startPatroling = true;
                        }

                    if (_actionPathPending)
                        // if the bot is going towards the action destination
                        if (_startAction == false && Vector3.Distance(transform.position, CurrentDestination) <
                            _currentAction.stopDistance + 0.1f)
                        {
                            _startAction = true; // now the action starts!
                            // snap to the disired position and rotation
                            transform.position = CurrentDestination;
                            transform.rotation = _currentAction.rotation;
                            _agent.isStopped = true;
                            Action start = null;
                            Action end = null;
                            int waitTime;

                            Debug.Log("<color=green> Start Action " + _currentAction.name + "</color>");
                            if (_currentAction.name == typingAction.name)
                            {
                                Debug.Log("<color=green>entered typing</color>");
                                start = () => _animator.SetBool("typing", true);
                                end = () => _animator.SetBool("typing", false);
                                waitTime = UnityEngine.Random.Range(10, 15);
                            }
                            else if (_currentAction.name == lookAtSphereAction.name)
                            {
                                Debug.Log("<color=green>entered looking</color>");
                                start = () => { transform.LookAt(_currentAction.targetTransform); };
                                waitTime = 5;
                            }
                            else
                            {
                                Debug.Assert(_currentAction.name != "typingAction" &&
                                             _currentAction.name != "lookAtSphereAction");
                                Debug.Log("<color=green>entered other</color>", this);
                                waitTime = 8;
                            }

                            StartCoroutine(AbstractCoroutine(start, end, waitTime));
                        }

                    break;
            }
        }

        private void DrawLineOfSight()
        {
            Gizmos.color = Color.green;
            var circle = new Vector3[viewAngle + 4];
            var index = 0;
            for (var angle = -viewAngle / 2; angle <= viewAngle / 2; angle++)
            {
                var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
                if (realSightView)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, direction, out hit, viewDistance))
                        circle[++index] = hit.point;
                    else
                        circle[++index] = transform.position + direction.normalized * viewDistance;
                }
                else
                {
                    circle[++index] = transform.position + direction.normalized * viewDistance;
                }
            }

            Gizmos.DrawLine(transform.position, circle[1]);
            Gizmos.DrawLine(transform.position, circle[index]);
            for (var i = 1; i <= index - 1; i++)
                Gizmos.DrawLine(circle[i], circle[i + 1]);
        }

        private void OnDrawGizmos()
        {
            if (_agent != null)
            {
                if (inMeeting)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_agent.destination, 0.2f);
                    Gizmos.color = Color.magenta;
                }
                else
                {
                    Gizmos.DrawLine(transform.position, _agent.destination);
                    Gizmos.DrawSphere(_agent.destination, 0.2f);
                }
            }

            //Debug.Log("event has in gizmos for " + name + " : " + GetListenerNumber(loadEvent) + "listeners");
            if (drawLines)
                DrawLineOfSight();
        }
    }
}
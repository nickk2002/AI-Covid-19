using UnityEngine;
using UnityEngine.UI;
using Covid19.AIBehaviour.Actions;
using Covid19.AIBehaviour;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float gameDuration = 60f;
    public float timeScale = 1;


    public AnimationCurve infectionCurve; // click on this in the inspector while the game in running
    public AnimationCurve coughCurve;

    // draws the grapth of infected people over time
    int currentlyInfected = 0;
    private Text textPercentage;
    private bool _istextPercentageNotNull;


    void Awake()
    {
        Debug.Log("called awake");
        if (instance == null)
            instance = this;
        infectionCurve = new AnimationCurve();
        coughCurve = new AnimationCurve();
        ActionPlace.ClearDict();
        Bot.ClearBots();
        MeetPoint.ClearList();

    }



    void Start()
    {
        _istextPercentageNotNull = textPercentage != null;
        textPercentage = UIManager.Instance.textInfectPercentage;
        DrawFunc();
    }



    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        if (Time.time > gameDuration)
            Debug.Break();
        int infected = Bot.CountNumberInfected();
        if (currentlyInfected != infected)
        {
            float x2 = Time.time;
            int y2 = infected;
            Keyframe newKeyFrame = new Keyframe(x2, y2);
            // just for UI, draw a function in the animation curve
            if (currentlyInfected == 0)
            {
                infectionCurve.AddKey(newKeyFrame);
            }
            else
            {
                Keyframe lastKeyFrame = infectionCurve.keys[infectionCurve.keys.Length - 1];
                float x1 = lastKeyFrame.time;
                int y1 = (int)lastKeyFrame.value;

                newKeyFrame.inTangent = Mathf.Atan((y2 - y1) / (x2 - x1));
                lastKeyFrame.outTangent = Mathf.Atan((y2 - y1) / (x2 - x1));
                infectionCurve.AddKey(newKeyFrame);
                infectionCurve.RemoveKey(infectionCurve.keys.Length - 2);
                infectionCurve.AddKey(lastKeyFrame);
            }

            currentlyInfected = infected;
            float percentage =
                currentlyInfected * 1.0f / Bot.ListBots.Count *
                100; // 100% means all infected, 50% means half of them are infected
            if (_istextPercentageNotNull)
            {
                textPercentage.text = percentage + "%"; // puts the value in a text
                if (percentage == 100)
                    Debug.Break(); // if reached 100% than stop Play Mode(this line of code just pauses the game, like pressing pause in unity)
            }
        }
    }

    float CoughFunction(float x)
    {
        return 20 * (Mathf.Pow((8f / 10), x));
    }

    void DrawFunc()
    {
        int cnt = 0;
        bool firstTime = coughCurve.keys.Length == 0;
        for (float i = 0; i < 10; i += 0.1f)
        {
            float x2 = i;
            float y2 = CoughFunction(i);
            Keyframe keyframe = new Keyframe(x2, y2);
            if (firstTime)
                coughCurve.AddKey(keyframe);
            else
            {
                coughCurve.MoveKey(cnt, keyframe);
                cnt++;
            }
        }
    }
}
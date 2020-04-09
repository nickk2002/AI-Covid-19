using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance;
    public Text textPercentage;
    public List<Bot> listBots = new List<Bot>();
    public AnimationCurve infectionCurve; // click on this in the inspector while the game in running
    // draws the grapth of infected people over time
    int currentlyInfected = 0;

    public void AddBot(Bot bot)
    {
        if(listBots.Find(x => x == bot) == null)// if bot is not in the list, then I add it into the list
        {
            listBots.Add(bot);
        }
    }
    int CountNumberInfected()
    {
        int cnt = 0;
        foreach(Bot bot in listBots)
        {
            if (bot.infectionLevel > 0)
                cnt++;
        }
        return cnt;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    void Start() { 
        infectionCurve = new AnimationCurve();
    }

    // Update is called once per frame
    void Update()
    {

        if (currentlyInfected != CountNumberInfected())
        {
            float x2 = Time.time;
            int y2 = CountNumberInfected();
            Keyframe newKeyFrame = new Keyframe(x2, y2);
            // just for UI, draw a function in the animation curve
            // I can explain better if we talk
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
            currentlyInfected = CountNumberInfected();
            float percentage = currentlyInfected * 1.0f / listBots.Count * 100;// 100% means all infected, 50% means half of them are infected
            if (textPercentage != null)
            {
                textPercentage.text = percentage.ToString() + "%"; // puts the value in a text
                if (percentage == 100)
                    Debug.Break();// if reached 100% than stop Play Mode(this line of code just pauses the game, like pressing pause in unity)
            }
       }
    }
}

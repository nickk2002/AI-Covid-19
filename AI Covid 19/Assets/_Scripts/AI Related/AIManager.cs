using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;
    
    public float maxMeetingCooldown = 60f; /// cat timp dureaza pt un bot sa mai incerce sa se intalneasca cu cineva dupa ce a avut o intalnire
    public float maxViewDistance = 64f;/// cat de mult poate sa vada maxim un bot in functie de cat de sociabil este
    public float minViewDistance = 10f;// cat de putin poate sa vada un bot
    public float minTalkDuration = 20f;// TODO : sa introduc conditiile astea in Bot.cs in TryMeetBot
    public float maxTalkDuration = 60f;// un minut

    public AnimationCurve coughCurve;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        coughCurve = new AnimationCurve();
    }

    private void Start()
    {
        DrawFunc();
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

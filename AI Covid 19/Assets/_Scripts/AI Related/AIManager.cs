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
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

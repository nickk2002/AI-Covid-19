using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance;
    public List<Bot> listaBoti = new List<Bot>();

    public void AddBot(Bot bot)
    {
        if(listaBoti.Find(x => x == bot) == null)
        {
            listaBoti.Add(bot);
        }
    }
    public void Meet()
    {
        Bot bot1, bot2;
        bot1 = bot2 = null;
        foreach(Bot bot in listaBoti)
        {
            if (bot.currentState != Bot.State.Meet && bot1 == null)
                bot1 = bot;
            if(bot.currentState != Bot.State.Meet && bot1)

        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance;
    public Text textPercentage;
    public List<Bot> listaBoti = new List<Bot>();
    int infectatiCurent = 0;
    
    public void AddBot(Bot bot)
    {
        if(listaBoti.Find(x => x == bot) == null)
        {
            listaBoti.Add(bot);
        }
    }
    int Numara()
    {
        int cnt = 0;
        foreach(Bot bot in listaBoti)
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

    // Update is called once per frame
    void Update()
    {
       if(infectatiCurent != Numara())
       {
            Debug.Log(Time.time + " " + "sunt : " + Numara() + " boti infectati");
            infectatiCurent = Numara();
            float procent = infectatiCurent * 1.0f / listaBoti.Count * 100;
           
            textPercentage.text = procent.ToString() + "%";
            if (procent == 100)
                Debug.Break();
       }
    }
}

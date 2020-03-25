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

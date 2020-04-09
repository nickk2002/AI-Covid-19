using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public GameObject enemy;
    public float randomPosRange = 500;
    public int howMany;


    void Pune(int index)
    {
        Vector3 pos = new Vector3(Random.Range(-randomPosRange, randomPosRange), 0, Random.Range(-randomPosRange, randomPosRange));
        GameObject bot = Instantiate(enemy,pos,Quaternion.identity);
        bot.transform.SetParent(transform);
        if (index == 1 || index == 2 || index == 3)// start with 3 already infected Bots for the simulation
            bot.GetComponent<Bot>().alreadyInfected = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(1);
        for (int i = 1; i <= howMany; i++)
        {
            Pune(i);
        }
        
    }

}

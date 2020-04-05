using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public GameObject enemy;
    public int numar;


    void Pune(int index)
    {
        Vector3 pos = new Vector3(Random.Range(-500f, 500f), 0, Random.Range(-500f, 500f));
        GameObject bot = Instantiate(enemy,pos,Quaternion.identity);
        bot.transform.SetParent(transform);
        if (index == 3 || index == 7)
            bot.GetComponent<Bot>().infected = true;
        //Debug.Log("I am here bot spwa");
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= numar; i++)
        {
            Pune(i);
        }
    }

}

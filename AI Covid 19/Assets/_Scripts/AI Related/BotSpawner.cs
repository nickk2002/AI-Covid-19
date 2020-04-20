using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotSpawner : MonoBehaviour
{
    public GameObject enemy;
    public float randomRange = 500;
    public int howMany;

    Vector3 RandomPos()
    {
        Collider[] colliders;
        Vector3 position;
        int cnt = 0;
        do
        {
            position = new Vector3(Random.Range(-randomRange, randomRange), 0, Random.Range(-randomRange, randomRange));
            colliders = Physics.OverlapSphere(position, 5f);
            cnt++;
            if (cnt > 100)
                Debug.Break();
        } while (colliders.Length > 1);

        return position;
    }
    void Pune(int index)
    {
        Vector3 pos = RandomPos();
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

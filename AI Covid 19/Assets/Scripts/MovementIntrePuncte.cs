using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MovementIntrePuncte : MonoBehaviour
{
    public GameObject[] pozitii = new GameObject[3];
    public int current = 0;
    float radius = 2f;
    public bool moving = false;

    NavMeshAgent agent;
    Vector3 destinatie;
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();

        for (int i=0; i <= 2; i++)
        {

            pozitii[i] = GameObject.Find("Loc" + i);


            

        }

        destinatie = agent.destination;
        
    }

    // Update is called once per frame
    void Update()
    {

        if(moving == false)
        {
            destinatie = pozitii[current].transform.position;
            agent.destination = destinatie;
            moving = true;
        }

            if (Vector3.Distance(pozitii[current].transform.position, transform.position) < radius)
            {

            moving = false;

                current++;
                if (current >= pozitii.Length)
                {
                    current = 0;
                    
                }


            }

            

        
        
    }
}

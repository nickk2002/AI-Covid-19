using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MovementIntrePuncte : MonoBehaviour
{
    public GameObject[] pozitii;/// trebuie sa putem sa punem mai mult de 3 pozitii
    public int current = 0;
    float radius = 2f;
    public bool moving = false;
    public GameObject posHolder;

    NavMeshAgent agent;
    Vector3 destinatie;
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        pozitii = new GameObject[posHolder.transform.childCount];
        int i = 0;
        foreach(Transform child in posHolder.transform)
        {
            pozitii[i] = child.gameObject;
            i++;
        }
        /// nu aveai nevoie de find acolo ca aveai in inspector
        
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
        /// agent.remainingDistance < radius  face acelasi lucru ca idee
        
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

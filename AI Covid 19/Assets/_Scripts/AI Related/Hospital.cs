using System.Collections.Generic;
using UnityEngine;
using System;

public class Hospital : MonoBehaviour
{
    public static List<Hospital> listHospitals = new List<Hospital>();
    public List<Transform> beds = new List<Transform>();
    public bool[] occupiedBeds;

    public bool Free()
    {
        for(int i = 0; i < occupiedBeds.Length; i++)
        {
            if (occupiedBeds[i] == false)
                return true;
        }
        return false;
    }
    public Tuple<Vector3,int> BedPosition()
    {
        int index = -1;
        for (int i = 0; i < occupiedBeds.Length; i++)
        {
            if (occupiedBeds[i] == false)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            occupiedBeds[index] = true;
            return Tuple.Create(beds[index].position,index);
        }
        return Tuple.Create(Vector3.negativeInfinity,-1);
    } 
    public void LeaveBed(int index)
    {
        Debug.Assert(0 <= index && index < beds.Count,"Trying to free a bed with invalid index");
        occupiedBeds[index] = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        if (beds.Count == 0)
            Debug.LogError("Beds not set for hospital");
        listHospitals.Add(this);
        occupiedBeds = new bool[beds.Count];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

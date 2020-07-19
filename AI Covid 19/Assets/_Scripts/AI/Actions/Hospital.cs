using System;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AI.Actions
{
    public class Hospital : MonoBehaviour
    {
        public static List<Hospital> ListHospitals = new List<Hospital>();
        public List<Transform> beds = new List<Transform>();
        public bool[] occupiedBeds;

        public bool Free()
        {
            for (var i = 0; i < occupiedBeds.Length; i++)
                if (occupiedBeds[i] == false)
                    return true;
            return false;
        }

        public Tuple<Vector3, int> BedPosition()
        {
            var index = -1;
            for (var i = 0; i < occupiedBeds.Length; i++)
                if (occupiedBeds[i] == false)
                {
                    index = i;
                    break;
                }

            if (index != -1)
            {
                occupiedBeds[index] = true;
                return Tuple.Create(beds[index].position, index);
            }

            return Tuple.Create(Vector3.negativeInfinity, -1);
        }

        public void LeaveBed(int index)
        {
            Debug.Assert(0 <= index && index < beds.Count, "Trying to free a bed with invalid index");
            occupiedBeds[index] = false;
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (beds.Count == 0)
                Debug.LogError("Beds not set for hospital");
            ListHospitals.Add(this);
            occupiedBeds = new bool[beds.Count];
        }
    }
}
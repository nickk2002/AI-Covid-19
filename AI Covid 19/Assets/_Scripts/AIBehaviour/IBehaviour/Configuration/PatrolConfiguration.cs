using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    [CreateAssetMenu(menuName = "AI Behaviour/Patrol")]
    public class PatrolConfiguration : ScriptableObject
    {
        public bool randomLocations = false; // if he uses randomLocations
        public float randomRange = 10f; // the range of the random
        public float stoppingDistance = 0.2f; // the distance at which the agent stops and moves to the next position

    }
}

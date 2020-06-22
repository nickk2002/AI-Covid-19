using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.States
{
    [CreateAssetMenu(menuName = "AI Behaviour/Meet")]
    public class MeetConfiguration : ScriptableObject
    {
        [Range(1,10)] public int sociabalLevel;
        [Range(1,5)] public int offsetMeeting;
        public float viewDist;
        public float viewAngle;
        public float cooldownMeeting;
    }
}
﻿using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour.Configuration
{
    [CreateAssetMenu(menuName = "AI Behaviour/NPC Data")]
    public class AgentConfiguration : ScriptableObject
    {
        public string npcName = "Bob";
        public string occupation = "Bussines Man";
        public int age = 35;

        [Range(1,10)] public int sociableLevel = 5;
        [Range(1, 10)] public int cautiousLevel = 5;
        [Range(1, 10)] public int immunityLevel = 5;
        
        public float cooldownMeeting = 5;
        public float stoppingDistance = 0.1f;
        
    }
}
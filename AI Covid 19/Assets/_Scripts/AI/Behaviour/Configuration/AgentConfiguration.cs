using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour.Configuration
{
    [CreateAssetMenu(menuName = "Covid19/AI/Data for Agent")]
    public class AgentConfiguration : ScriptableObject
    {
        public int age = 35;

        public AgentType agentType;

        [Range(1, 10)] public int cautiousLevel = 5;
        [Header("Debug")] public bool consoleDebug;

        public float cooldownMeeting = 5;
        public bool drawRealSight;
        [Range(1, 10)] public int immunityLevel = 5;
        public string npcName = "Bob";
        public string occupation = "Bussines Man";
        public bool sightDebug;
        [Range(1, 10)] public int sociableLevel = 5;
        [Range(0.1f, 4f)] public float stoppingDistance = 0.1f;
    }
}
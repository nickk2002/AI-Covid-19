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
        public float cooldownMeeting = 5;
        public bool drawRealSight;
        [Range(1, 10)] public int immunityLevel = 5;
        public string npcName = "Bob";
        public string occupation = "Bussines Man";
        [Range(1, 10)] public int sociableLevel = 5;
        public float stoppingDistance = 0.1f;
    }
}
using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour.Configuration
{
    [CreateAssetMenu(menuName = "Covid19/AI/Data for Agent")]
    public class AgentConfiguration : ScriptableObject
    {
        public bool debug;
        public AgentType agentType;
        public string npcName = "Bob";
        public string occupation = "Bussines Man";
        public int age = 35;
        
        [Range(1, 10)] public int cautiousLevel = 5;
        [Range(1, 10)] public int immunityLevel = 5;
        [Range(1, 10)] public int sociableLevel = 5;
        
        public float cooldownMeeting = 5;
        public bool drawRealSight;
        
        [Range(0.1f,4f)] public float stoppingDistance = 0.1f;
    }
}
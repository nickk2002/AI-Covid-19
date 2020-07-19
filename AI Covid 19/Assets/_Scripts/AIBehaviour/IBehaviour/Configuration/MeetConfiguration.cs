using UnityEngine;

namespace Covid19.AIBehaviour.IBehaviour.Configuration
{
    [CreateAssetMenu(menuName = "Covid19/AI/Meet")]
    public class MeetConfiguration : ScriptableObject
    {
        public float cooldownMeeting;
        [Range(1, 5)] public int offsetMeeting;
        [Range(1, 10)] public int sociableLevel;
        public float viewAngle;
        public float viewDist;
    }
}
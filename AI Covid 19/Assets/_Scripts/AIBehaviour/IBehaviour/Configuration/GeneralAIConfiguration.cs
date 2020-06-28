using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/General")]
public class GeneralAIConfiguration : ScriptableObject
{
    [Range(0.1f, 5f)] [Tooltip("The time that passes between to 'growths' of infection")]
    public float growthInterval = 3f;

    [Header("Infection")] [Range(0.1f, 1)] [Tooltip("The amound of infection added after each growth interval")]
    public float infectionSpeed = 0.1f;

    [Header("Meeting")] [Range(1, 5)] public float meetingDistance = 1;

    public float viewAngle = 60;

    [Range(10, 30)] public float viewDistance = 30;
}
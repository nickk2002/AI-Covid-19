using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/General")]
public class GeneralAIConfiguration : ScriptableObject
{
    [Range(0.1f, 5f)] [Tooltip("The time that passes between to 'growths' of infection")]
    public float growthInterval = 0.1f;

    [Header("Infection")] [Range(5, 10)] public float infectionDistance = 5;

    [Tooltip("The amound of infection added after each growth interval")] [Range(0.1f, 1)]
    public float infectionSpeed = 0.1f;

    [Header("Meeting")] [Range(1, 5)] public float meetingDistance = 1;

    public float viewAngle = 60;
    [Range(10, 30)] public float viewDistance = 30;
}
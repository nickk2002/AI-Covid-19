using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/General")]
public class GeneralAIConfiguration : ScriptableObject
{
    [Range(1,5)]
    public float meetingDistance;
    public float viewAngle;
    [Range(10,30)]
    public float viewDistance;
    
}

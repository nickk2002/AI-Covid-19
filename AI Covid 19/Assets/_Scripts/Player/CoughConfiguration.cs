using UnityEngine;

namespace Covid19.Player
{
    [CreateAssetMenu(menuName = "General Settings/Cough Configuration", order = 0)]
    public class CoughConfiguration : ScriptableObject
    {
        public float infectDistance = 5f;
        public float maxNumberCoughs = 100f;
        public AudioClip[] soundArray;

    }
}
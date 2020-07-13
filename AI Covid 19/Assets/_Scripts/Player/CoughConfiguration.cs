using UnityEngine;

namespace Covid19.Player
{
    [CreateAssetMenu(menuName = "General Settings/Cough Configuration", order = 0)]
    public class CoughConfiguration : ScriptableObject
    {
        public AudioClip[] soundArray;

    }
}
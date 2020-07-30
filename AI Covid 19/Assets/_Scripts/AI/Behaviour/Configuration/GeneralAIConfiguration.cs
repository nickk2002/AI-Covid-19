using System;
using UnityEngine;

namespace Covid19.AI.Behaviour.Configuration
{
    [CreateAssetMenu(menuName = "Covid19/AI/General")]
    public class GeneralAIConfiguration : ScriptableObject
    {
        [Tooltip("List of agents")] public MonoBehaviourList agentList;

        public AnimationCurve coughCurve = new AnimationCurve();


        [Header("Infection")] [Range(0.1f, 5f)] [Tooltip("The time that passes between to 'growths' of infection")]
        public float growthInterval = 2f;

        [Range(5, 10)] public float infectionDistance = 5;

        [Tooltip("The amound of infection added after each growth interval")] [Range(0.1f, 1)]
        public float infectionSpeed = 0.1f;

        [Tooltip("List of infirmerys")] public MonoBehaviourList infirmeryList;

        [Range(1, 10f)] public float maxInfectionValue = 10;

        [Header("Meeting")] [Range(1, 5)] public float meetingDistance = 1;

        [Range(1f, 10f)] public float timeScale = 1;

        [Range(60f, 90f)] public int viewAngle = 60;

        [Range(10, 30)] public float viewDistance = 30;

        private void OnEnable()
        {
            DrawCoughCurve();
        }

        private float CoughFunction(float x)
        {
            return 20 * Mathf.Pow(8f / 10, x);
        }

        private void DrawCoughCurve()
        {
            Array.Clear(coughCurve.keys, 0, coughCurve.keys.Length);
            for (float i = 0; i < 10; i += 0.1f)
            {
                var x2 = i;
                var y2 = CoughFunction(i);
                var keyframe = new Keyframe(x2, y2);
                coughCurve.AddKey(keyframe);
            }
        }
    }
}
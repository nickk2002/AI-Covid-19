using UnityEngine;

namespace Covid19.AIBehaviour
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance;

        public float maxMeetingCooldown = 60f;

        /// cat timp dureaza pt un bot sa mai incerce sa se intalneasca cu cineva dupa ce a avut o intalnire
        public float maxViewDistance = 64f;

        /// cat de mult poate sa vada maxim un bot in functie de cat de sociabil este
        public float minViewDistance = 10f; // cat de putin poate sa vada un bot


        public AnimationCurve coughCurve;


        // Start is called before the first frame update
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            coughCurve = new AnimationCurve();
        }

        private void Start()
        {
            DrawFunc();
        }

        private float CoughFunction(float x)
        {
            return 20 * Mathf.Pow(8f / 10, x);
        }

        private void DrawFunc()
        {
            var cnt = 0;
            var firstTime = coughCurve.keys.Length == 0;
            for (float i = 0; i < 10; i += 0.1f)
            {
                var x2 = i;
                var y2 = CoughFunction(i);
                var keyframe = new Keyframe(x2, y2);
                if (firstTime)
                {
                    coughCurve.AddKey(keyframe);
                }
                else
                {
                    coughCurve.MoveKey(cnt, keyframe);
                    cnt++;
                }
            }
        }
    }
}
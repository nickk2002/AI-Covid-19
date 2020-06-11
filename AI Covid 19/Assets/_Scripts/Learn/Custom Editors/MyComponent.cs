using UnityEngine;

namespace Covid19.Learn.Custom_Editors
{
    public class MyComponent : MonoBehaviour
    {
        public int numar;
        public float speed;
        [ContextMenuItem("Randomise position", "Randomise", order = 1)]
        [ContextMenuItem("Randomise Y position", "RandomiseY", order = 2)]
        public Vector3 position;

        void Randomise()
        {
            position = Random.insideUnitSphere * 20;
        }
        void RandomiseY()
        {
            position.y = Random.value * 20;
        }
        public void InstantiateGo()
        {
            GameObject gameObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObj.transform.position = position;


        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

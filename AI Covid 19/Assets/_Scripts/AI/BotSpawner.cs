using UnityEngine;

namespace Covid19.AI
{
    public class BotSpawner : MonoBehaviour
    {
        public GameObject enemy;
        public float randomRange = 500;
        public int howMany;

        private Vector3 RandomPos()
        {
            Collider[] colliders;
            Vector3 position;
            var cnt = 0;
            do
            {
                position = new Vector3(Random.Range(-randomRange, randomRange), 0,
                    Random.Range(-randomRange, randomRange));
                colliders = Physics.OverlapSphere(position, 5f);
                cnt++;
                if (cnt > 100)
                    Debug.Break();
            } while (colliders.Length > 1);

            return position;
        }

        private void Pune(int index)
        {
            var pos = RandomPos();
            var bot = Instantiate(enemy, pos, Quaternion.identity);
            bot.transform.SetParent(transform);
            if (index == 1 || index == 2 || index == 3) // start with 3 already infected Bots for the simulation
                bot.GetComponent<Bot>().alreadyInfected = true;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Random.InitState(1);
            for (var i = 1; i <= howMany; i++) Pune(i);
        }
    }
}
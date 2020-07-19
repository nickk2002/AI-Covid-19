using UnityEngine;

namespace Covid19.AI.Elevator
{
    public class Elevator : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
        }

        public void MoveUp()
        {
            GetComponent<Animator>().SetBool("move", true);
        }

        public void MoveDown()
        {
            GetComponent<Animator>().SetBool("move", true);
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}
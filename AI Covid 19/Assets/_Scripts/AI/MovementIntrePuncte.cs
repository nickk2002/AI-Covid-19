using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI
{
    public class MovementIntrePuncte : MonoBehaviour
    {
        public GameObject[] pozitii;

        /// trebuie sa putem sa punem mai mult de 3 pozitii
        public int current = 0;

        private float _radius = 2f;
        public bool moving = false;
        public GameObject posHolder;

        private NavMeshAgent _agent;
        private Vector3 _destinatie;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            pozitii = new GameObject[posHolder.transform.childCount];
            var i = 0;
            foreach (Transform child in posHolder.transform)
            {
                pozitii[i] = child.gameObject;
                i++;
            }

            /// nu aveai nevoie de find acolo ca aveai in inspector
        }

        // Update is called once per frame
        private void Update()
        {
            if (moving == false)
            {
                _destinatie = pozitii[current].transform.position;
                _agent.destination = _destinatie;
                moving = true;
            }
            /// agent.remainingDistance < radius  face acelasi lucru ca idee

            if (Vector3.Distance(pozitii[current].transform.position, transform.position) < _radius)
            {
                moving = false;
                current++;
                if (current >= pozitii.Length) current = 0;
            }
        }
    }
}
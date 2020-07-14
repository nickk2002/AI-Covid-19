using System.Collections;
using UnityEngine;

namespace Covid19.Learn
{
    public class TestRiderSyntax : MonoBehaviour
    {
        public GameObject cub;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<Camera>().fieldOfView = 150;
                Instantiate(cub);
                cub.transform.position = Random.insideUnitSphere * 100; // [0,100]
            }
        }
    }
}
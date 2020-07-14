using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.Utils
{
    public class DrawCircleDistance : MonoBehaviour
    {
        [SerializeField] private float circleRadius;
        private void OnDrawGizmos()
        {
            Vector3 [] circle = new Vector3[365];
            int index = 0;
            for (int angle = -180; angle <= 180; angle++)
            {
                Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward * circleRadius;
                circle[index++] = transform.position + direction;
            }

            for (int i = 0; i + 1 < index; i++)
                Gizmos.DrawLine(circle[i], circle[i + 1]);
            Gizmos.DrawLine(circle[index - 1],circle[0]);
        }
    }
}

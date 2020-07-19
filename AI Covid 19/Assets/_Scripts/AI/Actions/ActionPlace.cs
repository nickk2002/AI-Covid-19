using System.Collections.Generic;
using Covid19.Utils;
using UnityEngine;

namespace Covid19.AI.Actions
{
    [System.Serializable]
    public enum Place
    {
        Desk,
        Cook,
        Hospital,
        Bathroom,
        Lookat
    }

    public class ActionPlace : MonoBehaviour
    {
        public static Dictionary<Place, List<ActionPlace>> Dictionary = new Dictionary<Place, List<ActionPlace>>();
        [ShowOnly] public Place type;
        [ShowOnly] public Bot bot;
        [ShowOnly] public bool occupied = false;
        [ShowOnly] public float stopDistance;
    
        public static void ClearDict()
        {
            Debug.Log("<color=red>dictionary cleared</color>");
            Dictionary.Clear();
        }

        private void Awake()
        {
            if (Dictionary.ContainsKey(type))
            {
                Dictionary[type].Add(this);
            }
            else
            {
                var lista = new List<ActionPlace> {this};
                Dictionary.Add(type, lista);
            }

            var all = "";
            foreach (var key in Dictionary)
                all += key.Key + " ";
            // Debug.Log("<color=blue>" + all + "</color>");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            var circle = new Vector3[368];
            RaycastHit hit;
            var planePosition = transform.position;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
                planePosition = hit.point + new Vector3(0, 0.1f, 0); // un pic mai sus

            var index = 0;
            for (var angle = -180; angle <= 180; angle++)
            {
                var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
                circle[++index] = planePosition + direction.normalized * stopDistance;
            }

            for (var i = 1; i <= index - 1; i++)
                Gizmos.DrawLine(circle[i], circle[i + 1]);
        }
    }
}
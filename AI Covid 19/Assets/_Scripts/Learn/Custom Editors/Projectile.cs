using UnityEngine;

namespace Covid19.Learn.Custom_Editors
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [HideInInspector] public Rigidbody rb;
        public float damageRadius = 1f;
        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
        }
    }
}

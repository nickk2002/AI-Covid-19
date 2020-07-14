using UnityEngine;

namespace Covid19.Learn.Custom_Editors
{
    public class Launcher : MonoBehaviour
    {
        public Rigidbody projectile;
        public Vector3 offset;
        [Range(1, 15)] public float velocity = 15;

        [ContextMenu("Fire", false, -100)]
        public void Fire()
        {
            Debug.Log("fired");
            var body = Instantiate(projectile, transform.TransformPoint(offset), transform.rotation);
            body.velocity = Vector3.forward * velocity;
        }
    }
}

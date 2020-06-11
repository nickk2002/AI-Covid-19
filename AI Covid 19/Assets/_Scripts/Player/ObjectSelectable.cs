using UnityEngine;

namespace Covid19.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ObjectSelectable : MonoBehaviour
    {
        public Transform cameraTransform;
        public float distanceThrow = 30f;
        public float distanceSelect = 10f;
        private bool _isSelected = false;
        private Rigidbody _rb;
        Collider _gameobjectCollider;

        Player _player;
        Vector3 _difference;
        private float _holdTime = 0;
        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _player = Player.Instance;
            _gameobjectCollider = gameObject.GetComponent<Collider>();
        }


        void DisableSelect()
        {
            _gameobjectCollider.isTrigger = false;
            _rb.WakeUp();
            _rb.isKinematic = false;
            _isSelected = false;
            transform.parent = null;
            _player.hasObject = false;
            _holdTime = 0;

        }
        bool HasPlayer()
        {
            if (_player == null)
                return false;
            float dist = Vector3.Distance(_player.transform.position, this.transform.position);
            if (dist <= distanceSelect)
            {
                Vector3 directionCamera = cameraTransform.forward;
                Vector3 difference = transform.position - cameraTransform.transform.position;

                if (Vector3.Dot(difference.normalized, directionCamera.normalized) > 0.98f)
                {
                    return true;
                }
            }
            return false;
        }
        // Update is called once per frame
        void Update()
        {
            bool playerInRange = HasPlayer();

            if (playerInRange && _player.hasObject == false && Input.GetKeyDown(KeyCode.Mouse0))
            {
                _player.hasObject = true;
                _isSelected = true;
                _rb.isKinematic = true;
                _gameobjectCollider.isTrigger = true;
                transform.parent = cameraTransform;
            }
            if (_isSelected)
                _holdTime += Time.deltaTime;

            if (_isSelected)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    DisableSelect();
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    DisableSelect();
                    _rb.AddForce(cameraTransform.forward * distanceThrow, ForceMode.Impulse);
                }
            }
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (_player.hasObject && _holdTime >= 0.05f)
                DisableSelect();
        }
    }
}

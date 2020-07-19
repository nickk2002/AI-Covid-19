using UnityEngine;

namespace Covid19.Core
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ObjectSelectable : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float distanceThrow = 30f;
        [SerializeField] private float distanceSelect = 10f;
        
        private bool _isSelected = false;
        private Rigidbody _rb;
        private Collider _gameobjectCollider;
        
        private float _holdTime = 0;
        
        // Start is called before the first frame update
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _gameobjectCollider = gameObject.GetComponent<Collider>();
        }
        
        private void DisableSelect()
        {
            _gameobjectCollider.isTrigger = false;
            _rb.WakeUp();
            _rb.isKinematic = false;
            _isSelected = false;
            transform.parent = null;
            player.hasObject = false;
            _holdTime = 0;
        }

        private void Select()
        {
            player.hasObject = true;
            _isSelected = true;
            _rb.isKinematic = true;
            _gameobjectCollider.isTrigger = true;
            transform.parent = cameraTransform;
        }
        private bool PlayerInRange()
        {
            if (player == null)
                return false;
            float dist = Vector3.Distance(player.transform.position, transform.position);
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
        private void Update()
        {
            if (PlayerInRange() && player.hasObject == false && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Select();
            }
            if (_isSelected)
            {
                _holdTime += Time.deltaTime;
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
        private void OnTriggerEnter(Collider other)
        {
            if (player.hasObject && _holdTime >= 0.05f)
                DisableSelect();
        }
    }
}

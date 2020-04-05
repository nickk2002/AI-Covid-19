using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Selectable : MonoBehaviour
{
    public Transform cameraTransform;
    public float distanceThrow = 30f;
    public float distanceSelect = 10f;
    private bool isSelected = false;
    private bool collision = false;
    private Rigidbody rb;
    Collider collider;

    Player player;
    Vector3 difference;
    private float holdTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = Player.Instance;
        collider = gameObject.GetComponent<Collider>();
    }
  

    void DisableSelect()
    {
        collider.isTrigger = false;
        rb.WakeUp();
        rb.isKinematic = false;
        isSelected = false;
        transform.parent = null;
        player.hasObject = false;
        holdTime = 0;
        
    }
    bool HasPlayer()
    {
        if (player == null)
            return false;
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
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
        
        if(playerInRange && player.hasObject == false && Input.GetKeyDown(KeyCode.Mouse0))
        {
            player.hasObject = true;
            isSelected = true;
            rb.isKinematic = true;
            collider.isTrigger = true;
            transform.parent = cameraTransform;
        }
        if (isSelected)
            holdTime += Time.deltaTime;
        
        if(isSelected)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                DisableSelect();
            }else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                DisableSelect();
                rb.AddForce(cameraTransform.forward * distanceThrow,ForceMode.Impulse);
            }
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (player.hasObject && holdTime >= 0.05f) 
           DisableSelect();
    }
}

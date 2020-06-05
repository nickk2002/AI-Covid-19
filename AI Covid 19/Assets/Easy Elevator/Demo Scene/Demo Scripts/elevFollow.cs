using UnityEngine;

public class elevFollow : MonoBehaviour
{
    public Transform elevator;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, elevator.position.y + 2, transform.position.z);
    }
}

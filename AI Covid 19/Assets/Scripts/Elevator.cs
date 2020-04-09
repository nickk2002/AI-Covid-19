using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MoveUp()
    {
        GetComponent<Animator>().SetBool("move", true);
    }
    public void MoveDown()
    {
        GetComponent<Animator>().SetBool("move", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

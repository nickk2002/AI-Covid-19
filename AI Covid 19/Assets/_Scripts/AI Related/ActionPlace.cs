using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Place
{
    Desk,
}
public class ActionPlace : MonoBehaviour
{
    public Place type;
    public bool occupied = false;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.desks.Add(this);
    }
}

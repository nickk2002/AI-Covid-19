using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTeller : MonoBehaviour
{

    Text textComp;
    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int number = (int)Time.time;
        textComp.text = number.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InfectedUI : MonoBehaviour
{
    public Canvas canvas;
    public GameObject circleImagePrefab;
    public Image imageDisplay;
    private static int cnt = 0;
    

    private void Awake()
    {
        if(circleImagePrefab != null)
        {
            GameObject imageObject = Instantiate(circleImagePrefab);
            imageObject.transform.SetParent(canvas.transform);
            
            imageDisplay = imageObject.GetComponent<Image>();
            cnt++;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (imageDisplay != null)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
            imageDisplay.transform.position = position;
        }
    }
}

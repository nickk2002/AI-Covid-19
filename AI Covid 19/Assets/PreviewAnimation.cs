using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PreviewAnimation : MonoBehaviour
{
    public static int numar = 0;
    private void Start()
    {
        numar += 5;
        Debug.Log("numarul este " + numar);
    }
}

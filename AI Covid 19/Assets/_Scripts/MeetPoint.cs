using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeetPoint : MonoBehaviour
{
    static public List<MeetPoint> list = new List<MeetPoint>();
    public bool occupied;

    public static void ClearList()
    {
        list.Clear();
    }
    private void Start()
    {
        list.Add(this);
    }


}

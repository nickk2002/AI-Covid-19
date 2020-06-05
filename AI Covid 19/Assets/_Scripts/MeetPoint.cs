using System.Collections.Generic;
using UnityEngine;

public class MeetPoint : MonoBehaviour
{
    static public List<MeetPoint> List = new List<MeetPoint>();
    private int _weirdName;
    public bool occupied;

    public static void ClearList()
    {
        List.Clear();
    }
    private void Start()
    {
        List.Add(this);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Place
{
    Desk,
    Cook,
    Hospital,
    Bathroom,
    Lookat,
}

public class ActionPlace : MonoBehaviour
{
    public static List<ActionPlace> desks = new List<ActionPlace>();
    public static Dictionary<Place, List<ActionPlace>> dictionary = new Dictionary<Place, List<ActionPlace>>();
    public Place type;
    public Bot bot;
    public bool occupied = false;

    public static void ClearDict()
    {
        Debug.Log("<color=red>dictionary cleared</color>");
        dictionary.Clear();
    }
    void Awake()
    {
        if (dictionary.ContainsKey(type) == true)
        {
            dictionary[type].Add(this);
        }
        else
        {
            List<ActionPlace> lista = new List<ActionPlace>
            {
                this
            };
            dictionary.Add(type, lista);
        }
        string all = "";
        foreach (var key in dictionary)
            all += key.Key + " ";
        Debug.Log("<color=blue>" + all + "</color>");
    }
}

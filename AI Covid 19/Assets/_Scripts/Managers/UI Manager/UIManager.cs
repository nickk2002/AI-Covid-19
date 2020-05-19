using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Canvas canvas;
    public Text textInfectPercentage;
    public TextMeshProUGUI coughText;
    public Image coughLoadingBarUI;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        if (canvas == null)


        {
            Debug.LogError("Canvas not set in UI Manager");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

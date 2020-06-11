using Covid19.UI;
using Covid19.UI.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Covid19.GameManagers.UI_Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public Canvas canvas;
        public Text textInfectPercentage;
        public TextMeshProUGUI coughText;
        public Image coughLoadingBarUI;
        public ActionsManagerUI actionsManagerUI;
        
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

    }
}

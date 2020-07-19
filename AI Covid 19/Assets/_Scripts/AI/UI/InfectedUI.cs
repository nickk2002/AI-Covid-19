using Covid19.GameManagers.UI_Manager;
using UnityEngine;

namespace Covid19.AI.UI
{
    public class InfectedUI : MonoBehaviour
    {
        public GameObject botPanel;
        public GameObject circleImagePrefab;

        [HideInInspector] public GameObject imageDisplay;

        [HideInInspector] public GameObject panel;

        private void Start()
        {
            if (circleImagePrefab != null)
            {
                imageDisplay = Instantiate(circleImagePrefab, UIManager.Instance.canvas.transform, true);
                panel = Instantiate(botPanel, UIManager.Instance.canvas.transform, true);
                panel.SetActive(false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (imageDisplay != null)
            {
                var position = Camera.main.WorldToScreenPoint(transform.position);
                if (position.z < 0)
                {
                    imageDisplay.SetActive(false);
                }
                else
                {
                    imageDisplay.SetActive(true);
                    imageDisplay.transform.position = position;
                }
            }
        }
    }
}
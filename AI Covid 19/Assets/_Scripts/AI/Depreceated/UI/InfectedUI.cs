using UnityEngine;

namespace Covid19.AI.Depreceated.UI
{
    public class InfectedUI : MonoBehaviour
    {
        public GameObject botPanel;
        public GameObject circleImagePrefab;

        public GameObject imageDisplay;
        public GameObject panel;

        private void Start()
        {
            if (circleImagePrefab != null)
            { 
                Canvas canvas = FindObjectOfType<Canvas>();
                imageDisplay = Instantiate(circleImagePrefab, canvas.transform , true);
                panel = Instantiate(botPanel, canvas.transform, true);
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
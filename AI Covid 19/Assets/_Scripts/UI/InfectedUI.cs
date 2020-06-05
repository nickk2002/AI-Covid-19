using UnityEngine;


public class InfectedUI : MonoBehaviour
{
    public GameObject circleImagePrefab;
    public GameObject botPanel;

    [HideInInspector]
    public GameObject imageDisplay;
    [HideInInspector]
    public GameObject panel;

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
    void Update()
    {
        if (imageDisplay != null)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
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

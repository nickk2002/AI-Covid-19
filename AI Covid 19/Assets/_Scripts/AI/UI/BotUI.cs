using UnityEngine;

namespace Covid19.AI.UI
{
    public class BotUI : MonoBehaviour
    {
        [SerializeField] private GameObject updatedGameObject;
        [SerializeField] private GameObject notUpdatedGameObject;
        [SerializeField] private InfectedUI infectedUI;
        [SerializeField] private GameObject nameGizmos;


        public void LoadUpdated()
        {
            if (updatedGameObject == null || notUpdatedGameObject == null)
            {
                Debug.LogWarning("In the the ui the gameobjects are not set", this);
            }
            else
            {
                updatedGameObject.SetActive(true);
                notUpdatedGameObject.SetActive(false);
            }
        }

        public void LoadNotUpdated()
        {
            if (updatedGameObject == null || notUpdatedGameObject == null)
            {
                Debug.LogWarning("In the the ui the gameobjects are not set", this);
            }
            else
            {
                updatedGameObject.SetActive(false);
                notUpdatedGameObject.SetActive(true);
            }
        }

        public void SetGizmosName(string botName)
        {
            nameGizmos.name = botName;
        }
    }
}
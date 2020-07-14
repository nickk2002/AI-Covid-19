using UnityEngine;
using UnityEngine.UI;

namespace Covid19.UI.Quests
{
    [RequireComponent(typeof(Image))]
    public class SelecterUI : MonoBehaviour, ISelecterUI
    {
        public void Select()
        {
            gameObject.SetActive(true);
        }

        public void Deselect()
        {
            gameObject.SetActive(false);
        }
    }
}
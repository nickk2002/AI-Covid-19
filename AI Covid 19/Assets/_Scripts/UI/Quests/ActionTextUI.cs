using Covid19.Player.Quests;
using TMPro;
using UnityEngine;

namespace Covid19.UI.Quests
{
    public class ActionTextUI : MonoBehaviour
    {
        private ISelecterUI _selecter;
        private TextMeshProUGUI _textMeshPro;
        public Quest QuestOwner { set; get; }    
    
        public string Text
        {
            get => _textMeshPro.text;
            set => _textMeshPro.text = value;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            _selecter = GetComponentInChildren<ISelecterUI>(true);
            Debug.Log(_selecter);
        }

        public void Select()
        {
            _selecter.Select();
        }

        public void Deselect()
        {
            _selecter.Deselect();
        }
    }
}
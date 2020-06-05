using UnityEngine;
using UnityEngine.UI;

namespace Covid19.UI
{
    public class TimeTeller : MonoBehaviour
    {
        private Text _textComp;

        // Start is called before the first frame update
        private void Start()
        {
            _textComp = GetComponent<Text>();
        }

        // Update is called once per frame
        private void Update()
        {
            var number = (int) Time.time;
            _textComp.text = number.ToString();
        }
    }
}
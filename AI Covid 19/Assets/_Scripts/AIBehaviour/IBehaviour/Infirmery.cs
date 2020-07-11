using UnityEditor.UIElements;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class Infirmery : MonoBehaviour
    {
        public GameObject target;

        void Start()
        {
            AgentManager.Instance.SetInfirmery(this);
        }
    }
}
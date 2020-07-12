using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class DoctorBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;

        public void Enable()
        {
            _npc = GetComponent<AgentNPC>();
            Infirmery.Instance.AddDoctor(_npc);
        }

        public void Disable()
        {
        }

        public IEnumerator OnUpdate()
        {
            while (true) yield return null;
        }

        public override string ToString()
        {
            return "Stays in place";
        }
    }
}
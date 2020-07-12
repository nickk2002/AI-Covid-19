using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class Infirmery : MonoBehaviour
    {
        public static Infirmery Instance;
        private readonly List<AgentNPC> _doctorList = new List<AgentNPC>();
        public GameObject target;

        private void Awake()
        {
            Instance = this;
        }

        public void AddDoctor(AgentNPC doctor)
        {
            if (_doctorList.Contains(doctor) == false)
                _doctorList.Add(doctor);
        }

        public void CallDoctor(AgentNPC pacient)
        {
            AgentNPC doctor = _doctorList[0];
            var healBehaviour = doctor.gameObject.AddComponent<HealAgentBehaviour>();
            healBehaviour.pacient = pacient;
            doctor.SetBehaviour(healBehaviour);
        }
    }
}
﻿using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class Infirmery : MonoBehaviour
    {
        public List<Transform> beds;
        public static Infirmery Instance;
        private readonly List<AgentNPC> _doctorList = new List<AgentNPC>();
        private readonly List<bool> _ocuppiedBeds = new List<bool>();
        private readonly Dictionary<AgentNPC,int> _npcBedIndex = new Dictionary<AgentNPC,int>();
        public GameObject target;

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < beds.Count; i++)
                _ocuppiedBeds.Add(false);
        }

        public bool HasAvailableSpace()
        {
            foreach (var value in _ocuppiedBeds)
                if (value == false)
                    return true;
            return false;
        }

        public Transform GetBedPosition(AgentNPC pacientNpc)
        {
            for(int i = 0; i < _ocuppiedBeds.Count; i++)
                if (_ocuppiedBeds[i] == false)
                {
                    _npcBedIndex.Add(pacientNpc, i);
                    _ocuppiedBeds[i] = true;
                    return beds[i];
                }
            return null;
        }

        public void FreeBed(AgentNPC pacient)
        {
            Debug.Assert(_npcBedIndex.ContainsKey(pacient),"The pacient want to leave a bed and had not registred in the hospital");
            _ocuppiedBeds[_npcBedIndex[pacient]] = false;
            _npcBedIndex.Remove(pacient);
        }

        public void AddDoctor(AgentNPC doctor)
        {
            if (_doctorList.Contains(doctor) == false)
                _doctorList.Add(doctor);
        }

        public void CallDoctor(AgentNPC pacient)
        {
            AgentNPC doctor = _doctorList[0];
            if (doctor.GetComponent<HealAgentBehaviour>() == null)
            {
                var healBehaviour = doctor.gameObject.AddComponent<HealAgentBehaviour>();
                healBehaviour.AddPacient(pacient);
                doctor.SetBehaviour(healBehaviour);
            }
            else
            {
                var healBehaviour = doctor.gameObject.GetComponent<HealAgentBehaviour>();
                healBehaviour.AddPacient(pacient);
            }
        }
    }
}
﻿using System.Collections.Generic;
using Covid19.AI.Behaviour.States;
using Covid19.AI.Behaviour.Systems;
using UnityEngine;

namespace Covid19.AI.Behaviour
{
    public class Infirmery : MonoBehaviour
    {

        [SerializeField] private GameObject bedHolder;
        [SerializeField] private List<Transform> beds;
        private readonly List<AgentNPC> _doctorList = new List<AgentNPC>();
        private readonly List<bool> _ocuppiedBeds = new List<bool>();
        private readonly Dictionary<AgentNPC,int> _npcBedIndex = new Dictionary<AgentNPC,int>();

        private void Awake()
        {
            if (bedHolder != null)
            {
                foreach (Transform t in bedHolder.transform)
                {
                    beds.Add(t);
                }
            }
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
            if (_doctorList.Count == 0)
            {
                Debug.LogError("There is no doctor in the scene!");
                return;
            }

            AgentNPC doctor = ClosestDoctorFromPacient(pacient);
            if (doctor.GetComponent<HealAgentBehaviour>() == null)
            {
                var healBehaviour = doctor.gameObject.AddComponent<HealAgentBehaviour>();
                healBehaviour.AddPacient(pacient);
                doctor.BehaviourSystem.SetBehaviour(healBehaviour,TransitionType.StackTransition);
            }
            else
            {
                var healBehaviour = doctor.gameObject.GetComponent<HealAgentBehaviour>();
                healBehaviour.AddPacient(pacient);
            }
        }

        private AgentNPC ClosestDoctorFromPacient(AgentNPC pacient)
        {
            AgentNPC bestDoctor = null;
            float minDist = 2e9f;
            foreach (AgentNPC doctor in _doctorList)
            {
                float sqrtDist = Vector3.SqrMagnitude(pacient.transform.position - doctor.transform.position);
                if (sqrtDist < minDist)
                {
                    minDist = sqrtDist;
                    bestDoctor = doctor;
                }
            }
            return bestDoctor;
        }
    }
}
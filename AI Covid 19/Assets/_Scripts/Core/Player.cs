using System;
using System.Collections;
using System.Collections.Generic;
using Covid19.AI.Behaviour;
using Covid19.AI.Behaviour.Configuration;
using Covid19.AI.Behaviour.Systems;
using Covid19.Core.Quests;
using UnityEngine;

namespace Covid19.Core
{
    public class Player : MonoBehaviour
    {
        public MonoBehaviourList agentNPCList;
        public CoughConfiguration coughConfiguration;
        [HideInInspector] public bool hasObject = false;
        [HideInInspector] public Camera mainCamera;

        public static event Action OnFirstCough;
        
        private readonly List<QuestRequirement> _questRequirementList = new List<QuestRequirement>();

        private float _coughCount = 0;
        private AudioSystem _audioSystem;
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            mainCamera = GetComponentInChildren<Camera>();
            _audioSystem = new AudioSystem(coughConfiguration.soundArray,GetComponent<AudioSource>());
        }

        private void Start()
        {
            StartCoroutine(CoughMecanicCoroutine());
        }

        public bool HasRequirement(QuestRequirement questRequirement)
        {
            return _questRequirementList.Contains(questRequirement);
        }

        public void TakeQuestRequirement(QuestRequirement questRequirement)
        {
            _questRequirementList.Add(questRequirement);
        }
        
        private void TryInfectSomeone()
        {
            foreach (AgentNPC agentNPC in agentNPCList.items)
                if (Vector3.Distance(transform.position, agentNPC.transform.position) <=
                    coughConfiguration.infectDistance)
                {
                    agentNPC.StartInfection();
                    break;
                }
        }

        private void ShowLines()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (AgentNPC agentNPC in agentNPCList.items)
                if (Vector3.Distance(transform.position, agentNPC.transform.position) <=
                    coughConfiguration.infectDistance)
                {
                    positions.Add(transform.position);
                    positions.Add(agentNPC.transform.position);
                }

            _lineRenderer.positionCount = positions.Count;
            _lineRenderer.SetPositions(positions.ToArray());
        }

        private IEnumerator CoughMecanicCoroutine()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.C) && _coughCount < coughConfiguration.maxNumberCoughs)
                {
                    if (_coughCount == 0)
                        OnFirstCough?.Invoke();
                    _coughCount++;
                    float length = _audioSystem.PlayRandomCough();
                    TryInfectSomeone();
                    yield return new WaitForSeconds(length);
                }
                yield return null;
            }
        }
    }
}
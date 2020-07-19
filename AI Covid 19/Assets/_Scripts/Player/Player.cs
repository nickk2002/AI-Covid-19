using System;
using System.Collections;
using System.Collections.Generic;
using Covid19.AIBehaviour;
using Covid19.AIBehaviour.Behaviour;
using Covid19.GameManagers.UI_Manager;
using Covid19.Player.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Covid19.Player
{
    public class Player : MonoBehaviour
    {
        public CoughConfiguration coughConfiguration;
        [HideInInspector] public bool hasObject = false;
        [HideInInspector] public Camera mainCamera;
        
        public event Action OnFirstCough;
        
        public static Player Instance;
        private readonly List<QuestRequirement> _questRequirementList = new List<QuestRequirement>();
        
        private float _coughCount = 0;
        
        private AudioClip _lastAudioClip;
        private AudioSource _source;
        private LineRenderer _lineRenderer;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            _source = GetComponent<AudioSource>();
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            mainCamera = GetComponentInChildren<Camera>();
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
        private AudioClip RandomAudio()
        {
            AudioClip clip = coughConfiguration.soundArray[Random.Range(0, coughConfiguration.soundArray.Length - 1)];
            var tries = 0;
            while (clip == _lastAudioClip && tries <= 3)
            {
                clip = coughConfiguration.soundArray[Random.Range(0, coughConfiguration.soundArray.Length - 1)];
                tries++;
            }

            _lastAudioClip = clip;
            return clip;
        }
        
        private void TryInfectSomeone()
        {
            foreach (AgentNPC agentNPC in AgentManager.Instance.agentNPCList)
                if (Vector3.Distance(transform.position, agentNPC.transform.position) <= coughConfiguration.infectDistance)
                {
                    agentNPC.StartInfection();
                    break;
                }
        }

        private void ShowLines()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (AgentNPC agentNPC in AgentManager.Instance.agentNPCList)
                if (Vector3.Distance(transform.position, agentNPC.transform.position) <= coughConfiguration.infectDistance)
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
                    AudioClip coughClip = RandomAudio();
                    _source.clip = coughClip;
                    _source.Play();
                    yield return new WaitForSeconds(coughClip.length);
                }
                yield return null;
            }
        }
        // Update is called once per frame
        private void Update()
        {
            //ShowLines();
        }
    }
}
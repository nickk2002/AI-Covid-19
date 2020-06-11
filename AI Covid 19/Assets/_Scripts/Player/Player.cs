using System.Collections;
using System.Collections.Generic;
using Covid19.AIBehaviour;
using Covid19.GameManagers.UI_Manager;
using Covid19.Player.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Covid19.Player
{
    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        public static Player Instance;

        public AudioClip[] coughSoundArray;
        [Header("Coughing")]

        public float coughInfectDistance = 5f;
        public float maxNumberCoughs = 3f;

        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public bool hasObject = false;

        float _currentTimer;
        AudioClip _coughClip;
        AudioClip _lastAudioClip;
        AudioSource _source;

        private float _coughCount = 0;
        private Image _coughLoadingBarUI;
        private TextMeshProUGUI _coughTextUI;
        private List<QuestRequirement> _questRequirementList = new List<QuestRequirement>();
        private FirstPersonController _firstPersonController;


        void Awake()
        {
            if (Instance == null)
                Instance = this;
            _source = GetComponent<AudioSource>();
            _firstPersonController = GetComponent<FirstPersonController>();
            mainCamera = GetComponentInChildren<Camera>();
        }
        private void Start()
        {
            _coughLoadingBarUI = UIManager.Instance.coughLoadingBarUI;
            _coughTextUI = UIManager.Instance.coughText;
        }

        public bool HasRequirement(QuestRequirement questRequirement)
        {
            return _questRequirementList.Contains(questRequirement);
        }
        public void TakeQuestRequirement(QuestRequirement questRequirement)
        {
            _questRequirementList.Add(questRequirement);
        }

        public IEnumerator JumpAt(Vector3 position)
        {
            _firstPersonController.enabled = false;
            transform.position = position;
            yield return new WaitForFixedUpdate();
        }
        private AudioClip RandomAudio()
        {
            AudioClip clip = coughSoundArray[Random.Range(0, coughSoundArray.Length - 1)];
            int tries = 0;
            while (clip == _lastAudioClip && tries <= 3)
            {
                clip = coughSoundArray[Random.Range(0, coughSoundArray.Length - 1)];
                tries++;
            }

            _lastAudioClip = clip;
            return clip;
        }
        bool CanInfect(Bot bot)
        {
            float dist = Vector3.Distance(transform.position, bot.transform.position);
            if (dist <= coughInfectDistance && bot.cured == false)
                return true;
            return false;
        }
        void TryInfectSomeone()
        {
            foreach (Bot bot in Bot.ListBots)
            {
                /// daca nu este infectat si este in distanta potrivita atunci il infecteaza
                if (bot.infectionLevel == 0 && CanInfect(bot))
                {
                    //Debug.Log("Player has Infected bot : " + bot.name);
                    bot.StartInfection();
                    return;
                }
            }
        }
        void CoughMecanic()
        {
            if (Input.GetKeyDown(KeyCode.C) && _coughCount < maxNumberCoughs && (_coughClip == null || _currentTimer > _coughClip.length))
            {
                _coughCount++;
                _coughClip = RandomAudio();
                //Debug.Log("Playing clip : " + coughClip.name);
                _source.PlayOneShot(_coughClip);
                TryInfectSomeone();
                _currentTimer = 0;
            }
            else if (_coughClip != null)
            {
                if (_currentTimer < _coughClip.length)
                {
                    if (_coughTextUI != null)
                    {
                        int value = (int)(_currentTimer / _coughClip.length * 100);
                        _coughTextUI.text = value.ToString() + "%";
                    }
                    if (_coughLoadingBarUI)
                        _coughLoadingBarUI.fillAmount = _currentTimer / _coughClip.length;
                    _currentTimer += Time.deltaTime;
                }
                else
                {
                    if (_coughTextUI != null)
                        _coughTextUI.text = "100%";
                    if (_coughLoadingBarUI)
                        _coughLoadingBarUI.fillAmount = 1;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            CoughMecanic();
        }
    }
}
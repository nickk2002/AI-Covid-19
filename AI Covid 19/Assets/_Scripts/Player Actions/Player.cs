using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    static public Player Instance;

    public AudioClip [] coughSoundArray;
    [Header("Coughing")]
    
    public float coughInfectDistance = 5f;
    public float maxNumberCoughs = 3f;

    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public bool hasObject = false;
    float currentTimer;
    AudioClip coughClip;
    AudioClip lastAudioClip;
    AudioSource source;
    
    private float coughCount = 0;
    private Image coughLoadingBarUI;
    private TextMeshProUGUI coughTextUI;



    void Awake()
    {
        if (Instance == null)
            Instance = this;
        source = GetComponent<AudioSource>();
        mainCamera = GetComponentInChildren<Camera>();
    }
    private void Start()
    {
        coughLoadingBarUI = UIManager.Instance.coughLoadingBarUI;
        coughTextUI = UIManager.Instance.coughText;
    }

    AudioClip RandomAudio()
    {
        AudioClip clip = coughSoundArray[Random.Range(0, coughSoundArray.Length - 1)];
        int tries = 0;
        while(clip == lastAudioClip && tries <= 3)
        {
            clip = coughSoundArray[Random.Range(0, coughSoundArray.Length - 1)];
            tries++;
        }

        lastAudioClip = clip;
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
        foreach(Bot bot in GameManager.instance.listBots)
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
        if (Input.GetKeyDown(KeyCode.C) && coughCount < maxNumberCoughs && (coughClip == null || currentTimer > coughClip.length))
        {
            coughCount++;
            coughClip = RandomAudio();
            //Debug.Log("Playing clip : " + coughClip.name);
            source.PlayOneShot(coughClip);
            TryInfectSomeone();
            currentTimer = 0;
        }
        else if (coughClip != null)
        {
            if (currentTimer < coughClip.length)
            {
                if (coughTextUI != null)
                {
                    int value = (int)(currentTimer / coughClip.length * 100);
                    coughTextUI.text = value.ToString() + "%";
                }
                if (coughLoadingBarUI)
                    coughLoadingBarUI.fillAmount = currentTimer / coughClip.length;
                currentTimer += Time.deltaTime;
            }
            else
            {
                if (coughTextUI != null)
                    coughTextUI.text = "100%";
                if (coughLoadingBarUI)
                    coughLoadingBarUI.fillAmount = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CoughMecanic();
    }
    private void OnDrawGizmos()
    {
        
    }
}

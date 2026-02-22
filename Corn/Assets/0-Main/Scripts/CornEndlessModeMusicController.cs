using DG.Tweening;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CornEndlessModeMusicController : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioSource musicNotePlayerAS;
    [SerializeField] private List<AudioClip> musicNoteClips;
    [SerializeField] private AudioSource[] musicTrackAudioSources;
    private bool IsMusicNotePlaying = false;
    private event Action OnNoteFinishedPlaying;

    [SerializeField] private int TriggerFoodInPotMusicThreshold = 4;
    private CornItemManager _itemManager;
    //int validFoodNumberInPot = 0;
    bool foodInPotTrackIsPlaying = false;
    bool inactivityCountdownTimeActive = false;
    private event Action OnInactivityCountDownTimerFinished;
    private float countDownTimer = 0;
    [SerializeField] private float InactivityCountDownTime = 10;




    // Start is called before the first frame update
    //TO-DO 
    //corn buoyancy script tracks total number of food in pot
    //if total number in pot reaches threshold, trigger music event 
    //need one game event to trigger music event, to which endless music controller listens 
    //add bool to check if music event is already triggered. 
    //timer to reset bool and music 

    void Start()
    {
        _itemManager = FindObjectOfType<CornItemManager>();
        CornGameEvents.instance.OnMusicNoteTriggered += TriggerRandomMusicNoteFromList;
       // CornGameEvents.instance.OnTotalFoodInPotCountValueChanged += UpdateFoodInPotCount;
        CornGameEvents.instance.OnEndlessModeBegin += EnableMusic;
        DisableMusic();
        CornGameEvents.instance.OnMusicInactivityTimerReset += ResetCountDownTimer;
        CornGameEvents.instance.OnBeginMusicInactivityTimer += StartInactivityTimer;
        


    }

    void Update()
    {
        if (foodInPotTrackIsPlaying || inactivityCountdownTimeActive)
        {
            if (countDownTimer > 0f)
            {
                countDownTimer -= Time.deltaTime;
                print(countDownTimer);
            }
            else
            { 
                OnInactivityCountDownTimerFinished?.Invoke();
            
            }

            
        }


    }


    public void ResetVolumeofTracks(string[] volumeParameterNames, float fadeDuration)
    {
        for (int i = 0; i < volumeParameterNames.Length; i++)
        {
            FadeTrackVolume(volumeParameterNames[i], 0f, fadeDuration);

        }


    }
    public void FadeTrackVolume(string parameterName, float ToVolumeNormalized, float fadeDuration)
    {
        var ToVolume = 0f;
        bool tryGetExposedParameter = musicMixer.GetFloat(parameterName, out ToVolume);
        if (!tryGetExposedParameter)
        {
            Debug.LogError("Audio Mixer Parameter not found! Value cannot be set");
            return;
        }
        ToVolume = Mathf.Log10(Mathf.Clamp(ToVolumeNormalized, 0.0001f, 1f)) * 20f; //map 0-1 value to 20 - -80
        print(ToVolume);

        if(DOTween.TweensById(parameterName,true) != null)
        DOTween.Kill(parameterName); //interrupt all in progress fades and go with new fade

        Tween volumeTween = musicMixer.DOSetFloat(parameterName, ToVolume, fadeDuration).SetId(parameterName);

        
    }


    private void TriggerRandomMusicNoteFromList()
    {
        if (musicNoteClips.Count > 0 && !IsMusicNotePlaying)
        {
            IsMusicNotePlaying = true;
            OnNoteFinishedPlaying += HandleNoteFinishedPlaying;
            AudioClip noteTriggered = AudioManager.instance.PlayRandomSoundAtPosition(musicNoteClips, musicNotePlayerAS, transform.position, 1);
            StartCoroutine(WaitForClipEnd(noteTriggered));
        }
            


    }
    

    private IEnumerator WaitForClipEnd(AudioClip targetClip)
    {
        yield return new WaitForSeconds(targetClip.length);
        OnNoteFinishedPlaying?.Invoke();
        OnNoteFinishedPlaying -= HandleNoteFinishedPlaying;


    }

    private void HandleNoteFinishedPlaying()
    {
        IsMusicNotePlaying = false;

    }



    //food in pot track
    //after triggering track, if the player doesn't add more food or eat food, track slowly fades out
    public void UpdateFoodInPotCount(int addedValue)
    {
        _itemManager.NumOfFoodInPot += addedValue;
        print("num of Food in pot = " + _itemManager.NumOfFoodInPot);

        
         if (_itemManager.NumOfFoodInPot >= TriggerFoodInPotMusicThreshold)
        {

            ResetCountDownTimer(); // if food in track is already playing, only refresh countdown timer. if not, reset timer And trigger track

            if(!foodInPotTrackIsPlaying)
            TriggerFoodInPotTrack();

        }

    }

    private void StartInactivityTimer()
    {
        CornGameEvents.instance.ResetInactivityTimer();        
        OnInactivityCountDownTimerFinished += HandleOnCountDownFinished;
        inactivityCountdownTimeActive = true;
        

    }

    private void TriggerFoodInPotTrack()
    {
        print("food in track");
        
        foodInPotTrackIsPlaying = true;
        FadeTrackVolume("Volume_Track4", 0.8f, 3f);
        OnInactivityCountDownTimerFinished += HandleOnCountDownFinished;

    }

    void ResetCountDownTimer()
    {
        countDownTimer = InactivityCountDownTime;
    }


    private void HandleOnCountDownFinished()
    {
        OnInactivityCountDownTimerFinished -= HandleOnCountDownFinished;
        foodInPotTrackIsPlaying = false;

        //FadeTrackVolume("Volume_Track4", 0f, 3f);
        inactivityCountdownTimeActive = false;
        CornGameEvents.instance.InactivityTimerComplete();
       
    }

    private void DisableMusic()
    {
        FadeTrackVolume("Volume_GlobalMusic", 0f, 0.2f);
        foreach (var AS in musicTrackAudioSources)

        {
            AS.Stop();

        }
    }

    private void EnableMusic()
    {
        FadeTrackVolume("Volume_GlobalMusic", 0.5f, 0.2f);

        foreach (var AS in musicTrackAudioSources)

        {
            if(!AS.isPlaying)
            AS.Play();

        }

    }


   

    
}

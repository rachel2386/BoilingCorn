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
    private bool IsMusicNotePlaying = false;
    private event Action OnNoteFinishedPlaying;

    [SerializeField] private int TriggerFoodInPotMusicThreshold = 4;
    private CornItemManager _itemManager;
    //int validFoodNumberInPot = 0;
    bool foodInPotTrackIsPlaying = false;
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
        CornGameEvents.instance.OnTotalFoodInPotCountValueChanged += UpdateFoodInPotCount;


    }

    void Update()
    {
        if (foodInPotTrackIsPlaying)
        {
            if (countDownTimer > 0f)
            {
                countDownTimer -= Time.deltaTime;
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
        Tween volumeTween = musicMixer.DOSetFloat(parameterName, ToVolume, fadeDuration);

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
        FadeTrackVolume("Volume_Track4", 0f, 3f);
        



    }

    

   

    
}

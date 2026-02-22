using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornEndlessModeMusicEvents : MonoBehaviour
{
    [SerializeField]string[] AudioMixerTrackParameterNames;
    private CornEndlessModeMusicController mController;
    bool firstFoodAdded = false;
    bool firstFoodEaten = false;
    bool inactivityTimerActive = false;
    bool inIdleState = false;
    bool potBoiling = false;
    private int currentMusicState = -1;
    void Start()
    {
        mController = FindObjectOfType<CornEndlessModeMusicController>();
        FadeAllTracks(0,0.1f);

        
        CornGameEvents.instance.OnStoveOn += OnStoveTurnedOn;
        CornGameEvents.instance.OnPotBoiling += OnPotBoiling;
        CornGameEvents.instance.OnStoveOff += OnStoveTurnedOff;
        CornGameEvents.instance.OnFirstFoodAdded += OnFirstFoodAddedTrack;
        CornGameEvents.instance.OnFirstFoodEaten += OnFirstFoodEaten;
        CornGameEvents.instance.OnTotalFoodInPotCountValueChanged += OnFoodAdded;
        CornGameEvents.instance.OnFoodEaten += OnFoodEaten;
        CornGameEvents.instance.OnBeginMusicInactivityTimer += OnInactivityTimerStart;
        CornGameEvents.instance.OnMusicInactivityTimerComplete += OnInactivityTimerComplete;
        CornGameEvents.instance.OnReorder += OnReorder;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    void FadeAllTracks(float normalizedVolume, float fadeDuration)
    {
        if(mController != null)
        foreach (var name in AudioMixerTrackParameterNames)
        {  
          mController.FadeTrackVolume(name,normalizedVolume,fadeDuration);  
        
        }    
    
    }

    void OnStoveTurnedOn()
    {
        currentMusicState = 0;

        mController.FadeTrackVolume(AudioMixerTrackParameterNames[0], 0.5f, 10f); //fade in track intro

        mController.FadeTrackVolume(AudioMixerTrackParameterNames[1], 0f, 2f); //fade out track 1
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[2], 0f, 2f); //fade out track 2
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[3], 0f, 2f); //fade out track 3
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[4], 0f, 2f); //fade out track 4
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[5], 0f, 2f); //fade out track 5
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[6], 0f, 2f); //fade out no eating track

    }

    void OnPotBoiling() //track 1
    {
        if (mController == null) return;
        currentMusicState = 1;
        potBoiling = true;
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[1], 0.5f, 2f); //fade in track 1
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[0], 0f, 4f); //fade out track intro
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[6], 0f, 4f); //fade out no eating track
        

        mController.FadeTrackVolume(AudioMixerTrackParameterNames[2], 0f, 2f); //fade out track 2
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[3], 0f, 2f); //fade out track 3
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[4], 0f, 2f); //fade out track 4
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[5], 0f, 2f); //fade out track 5
    }

    void OnFoodAdded(int foodCount)
    {

        if (!potBoiling) return;
        if (!firstFoodAdded && inIdleState)
        {
            OnFirstFoodAddedTrack();
            inIdleState = false;

        }
            

        if (!inactivityTimerActive) return;
        CornGameEvents.instance.ResetInactivityTimer();
       
       
    
    }

    void OnFoodEaten()
    {
        if (!potBoiling) return;
        if (!firstFoodEaten)// happens if inactivity timer completes and variable resets to false
        {
            OnFirstFoodEaten();
            inIdleState = false;
        }
           

        if (!inactivityTimerActive) return;
        CornGameEvents.instance.ResetInactivityTimer();
       
       
    }

    

    void OnFirstFoodAddedTrack() //track 1& 2 3 playing
    {
        if (mController == null) return;
        if (!firstFoodAdded)
        {
            print("first food");
            firstFoodAdded = true;

            if (currentMusicState == 3) return; //do not fade track if we are already in state 3


            mController.FadeTrackVolume(AudioMixerTrackParameterNames[2], 0.5f, 2f); //fade in track 2
            mController.FadeTrackVolume(AudioMixerTrackParameterNames[3], 0.5f, 8f); //fade in track 3 duration equals time for food to cook
            currentMusicState = 2;

            //fade other tracks
            mController.FadeTrackVolume(AudioMixerTrackParameterNames[1], 0.5f, 4f); //keep track 1
            mController.FadeTrackVolume(AudioMixerTrackParameterNames[4], 0f, 7f); //fade out track 4
            mController.FadeTrackVolume(AudioMixerTrackParameterNames[5], 0f, 7f); //fade out track 5
            mController.FadeTrackVolume(AudioMixerTrackParameterNames[6], 0f, 7f); //fade out no eating track
        }
        

    }

    void OnFirstFoodEaten() //track 2 3 4
    {
        if (mController == null) return;
        if (!potBoiling) return;
        firstFoodEaten = true;
        if (!inactivityTimerActive)
        {
            CornGameEvents.instance.StartInactivityTimer();
        }
        currentMusicState = 3;
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[4], 0.5f, 4f); //fade in track 4        
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[5], 0.5f, 4f); //fade in track 5
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[1], 0f, 6f); //fade out track 1


        //fade other tracks
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[2], 0.5f, 4f); //keep track 2
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[3], 0.5f, 4f); //keep track 3
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[6], 0f, 6f); //fade out no eating track

    }

    void OnEnterPlayerIdleState()
    {
        if (!potBoiling) return;
        currentMusicState = 0;
        inIdleState = true;
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[6], 1f, 4f); //fade in no eating track
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[1], 0f, 9f); //fade out track 1
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[2], 0f, 9f); //fade out track 2
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[3], 0f, 9f); //fade out track 3
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[4], 0f, 9f); //fade out track 4
        mController.FadeTrackVolume(AudioMixerTrackParameterNames[5], 0f, 9f); //fade out track 5

    }

    void OnReorder()
    {
        if (!inactivityTimerActive) return;
        CornGameEvents.instance.ResetInactivityTimer();

    }

    void OnStoveTurnedOff() // no tracks
    {
        FadeAllTracks(0, 2f);
       // mController.FadeTrackVolume(AudioMixerTrackParameterNames[6], 0.5f, 3f); //fade in not eating track
        firstFoodAdded = false;
        firstFoodEaten = false;
        inactivityTimerActive = false;
        currentMusicState = -1;
        potBoiling = false;

    }

    void OnInactivityTimerComplete()
    {
        inactivityTimerActive = false;

        if (currentMusicState > 0)
        {
            currentMusicState--;   

            switch (currentMusicState)
            {
                case 0:
                    OnEnterPlayerIdleState(); 
                    break;

                case 1:
                    OnPotBoiling();
                    firstFoodAdded = false; //reset var to retrigger track
                    break;
                case 2:
                    firstFoodAdded = false; //reset var to retrigger track
                    OnFirstFoodAddedTrack();
                    firstFoodEaten = false; //reset var to retrigger track
                    break;
                default: 
                    break;           
            
            }
            
            CornGameEvents.instance.StartInactivityTimer(); //if current state >0, restart timer

        }
        
        

        //To-do  switch on current music state
        //graually decrease state and fade music, till current state reaches 0, and we play no eating only

        //if state  = 0 and pot is boiling, play idle music. if pot isn't boiling, play intro. 
    }

    void OnInactivityTimerStart()
    { 
        
        inactivityTimerActive = true;
    
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornGameEvents : MonoBehaviour
{
    public static CornGameEvents instance;

    void Awake()
    {
        instance = this;
    }

    public event Action<int> OnEnterGameStateTransition;

    public void EnterGameStateTransition(int stateIndex)
    {
        OnEnterGameStateTransition?.Invoke(stateIndex);
    }

    public event Action<int> OnExitGameStateTransition;

    public void ExitGameStateTransition(int stateIndex)
    {
        OnExitGameStateTransition?.Invoke(stateIndex);
    }

    public event Action OnStartDumpingFood;

    public void StartDumpingFood()
    {
        if (OnStartDumpingFood != null)
            OnStartDumpingFood.Invoke();
    }

    public event Action OnMusicNoteTriggered;
        
    public void TriggerMusicNote()
    { 
        if(OnMusicNoteTriggered != null)
            OnMusicNoteTriggered.Invoke();
    }

    public event Action<int> OnTotalFoodInPotCountValueChanged;

    public void UpdateFoodInPotCount(int AmountToUpdate)
    {
        if (OnTotalFoodInPotCountValueChanged != null)
        { 
            OnTotalFoodInPotCountValueChanged.Invoke(AmountToUpdate);
        }
        
    }

    public event Action OnEndlessModeBegin;
    public void BeginEndlessMode()
    {
        if (OnEndlessModeBegin != null)
            OnEndlessModeBegin.Invoke();

    }

    public event Action OnStoveOn;
    public void StoveOn()
    {
        if (OnStoveOn != null)
            OnStoveOn.Invoke();

    }

    public event Action OnStoveOff;
    public void StoveOff()
    {
        if (OnStoveOff != null)
            OnStoveOff.Invoke();

    }


    public event Action OnPotBoiling;
    public void PotBoiling()
    { 
        if(OnPotBoiling!=null)
            OnPotBoiling.Invoke();
    
    }

    public event Action OnFirstFoodAdded;
    public void FirstFoodAdded()
    {
        if (OnFirstFoodAdded != null)
            OnFirstFoodAdded.Invoke();

    }

    public event Action OnFirstFoodEaten;
    public void FirstFoodEaten()
    {
        if (OnFirstFoodEaten != null)
            OnFirstFoodEaten.Invoke();

    }

    public event Action OnFoodEaten;
    public void EatFood()
    {
        if (OnFoodEaten != null)
            OnFoodEaten.Invoke();

    }

    public event Action OnReorder;
    public void ReorderFood()
    {
        if (OnReorder != null)
            OnReorder.Invoke();

    }

    public event Action OnBeginMusicInactivityTimer;
    public void StartInactivityTimer()
    {
        if (OnBeginMusicInactivityTimer != null)
            OnBeginMusicInactivityTimer.Invoke();

    }
    public event Action OnMusicInactivityTimerReset;
    public void ResetInactivityTimer()
    {
        if (OnMusicInactivityTimerReset != null)
            OnMusicInactivityTimerReset.Invoke();

    }

    public event Action OnMusicInactivityTimerComplete;
    public void InactivityTimerComplete()
    {
        if (OnMusicInactivityTimerComplete != null)
            OnMusicInactivityTimerComplete.Invoke();

    }




}

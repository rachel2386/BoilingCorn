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


}

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

    public event Action<int> OnGameStateSwitchEnter;

    public void EnterGameStateSwitch(int stateIndex)
    {
        OnGameStateSwitchEnter?.Invoke(stateIndex);
    }
    
    public event Action<int> OnGameStateSwitchExit;

    public void ExitGameStateSwitch(int stateIndex)
    {
        OnGameStateSwitchExit?.Invoke(stateIndex);
    }

    public event Action OnStartDumpingFood;

    public void StartDumpingFood()
    {
        if(OnStartDumpingFood != null)
        OnStartDumpingFood.Invoke();
    }



}

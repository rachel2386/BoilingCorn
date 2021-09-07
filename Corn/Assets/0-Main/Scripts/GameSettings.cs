using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.SimpleLUT;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    
    private static GameSettings instance;
    [SerializeField] Slider globalVolumeAdjuster;
    [SerializeField] Slider globalBrightnessAdjuster;
    [SerializeField] private SimpleLUT cameraEffectScript;
    [SerializeField] private Dropdown resolutionOptions;
    [SerializeField] private Letterboxer.Letterboxer _letterboxer;
    [SerializeField] private Toggle invertInputToggle;
    [SerializeField] private Toggle fullscreenToggle;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        globalVolumeAdjuster.onValueChanged.AddListener(AdjustGlobalVolume);
        globalVolumeAdjuster.value = 1;
        
        globalBrightnessAdjuster.onValueChanged.AddListener(AdjustGlobalBrightness);
        

       resolutionOptions.onValueChanged.AddListener(ChangeScreenResolution);
       invertInputToggle.onValueChanged.AddListener(InvertMouseY);
       fullscreenToggle.onValueChanged.AddListener(ToggleFullScreen);
       
       var currentResInList = false;
       
       for (int i = 0; i < resolutionOptions.options.Count; i++)
       {
           var values = resolutionOptions.options[i].text.Split('x');
           var width = int.Parse(values[0]);
           if (width == Screen.currentResolution.width)
           {
               resolutionOptions.value = i;
               currentResInList = true;
               break;
           }
       }
        
       //if system resolution not in list, create new option
       if (!currentResInList)
       {
           Dropdown.OptionData option = new Dropdown.OptionData
           {
               text = Screen.currentResolution.width.ToString() + 'x' + Screen.currentResolution.height,
               image = null
           };

           resolutionOptions.options.Add(option);
           resolutionOptions.value = resolutionOptions.options.Count - 1;
       }

       
       

    }

    private void AdjustGlobalVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    
    private void AdjustGlobalBrightness(float value)
    {
        cameraEffectScript.Brightness = value;
    }

    private void ChangeScreenResolution(int option)
    {
        string[] res = resolutionOptions.options[option].text.Split('x');
          
        var newScreenRes = Vector2Int.right * int.Parse(res[0]) + Vector2Int.up * int.Parse(res[1]);

        Screen.SetResolution(newScreenRes.x, newScreenRes.y, Screen.fullScreen);
        _letterboxer.TargetWidth = newScreenRes.x;
        _letterboxer.TargetHeight = newScreenRes.y;
        
        
        
    }
    
    private void InvertMouseY(bool toggled)
    {
        FindObjectOfType<CornMouseLook>().YSensitivity *= -1;
    }
    
    private void ToggleFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
}

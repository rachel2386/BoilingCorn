using DG.Tweening;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CornEndlessModeMusicController : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer;
    // Start is called before the first frame update
    void Start()
    {
        
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

        
        var ToVolume = Mathf.Log10(Mathf.Clamp(ToVolumeNormalized, 0.0001f,1f))*20f; //map 0-1 value to 20 - -80
        print(ToVolume);
        Tween volumeTween = musicMixer.DOSetFloat(parameterName, ToVolume, fadeDuration);

    }

    //To-do: inactivity timer

    

}

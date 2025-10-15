using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource _audioSource;
    [HideInInspector]public bool playMusic = false;
    [SerializeField]private List<AudioClip> musicToPlay = new List<AudioClip>();
    private int currentSongIndex = 0;
    
    

    // Start is called before the first frame update
    void Start()
    {
        if (musicToPlay.Count <= 0)
        {
            print("no music to play");
        }

        _audioSource.loop = false;
        InvokeRepeating("playSong", 0.1f, 1f);
    }

    // Update is called once per frame

 

    void playSong()
    {
        if(musicToPlay.Count <= 0 || !playMusic) return;
        
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = musicToPlay[currentSongIndex];
            _audioSource.Play();
            currentSongIndex++;
            
            if (currentSongIndex >= musicToPlay.Count)
                currentSongIndex = 0;
           
        }


       
    }


    void GoToNextSong()
    {
        _audioSource.Stop();
       
        
    }

    void ResetSong()
    {
        currentSongIndex = 0;
        _audioSource.clip = musicToPlay[currentSongIndex];
        _audioSource.Play();
    }
}
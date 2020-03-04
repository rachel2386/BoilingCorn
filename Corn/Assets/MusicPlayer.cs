using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> musicToPlay = new List<AudioClip>();
    private int currentSongIndex = 0;

    private AudioSource myAS;

    // Start is called before the first frame update
    void Start()
    {
        myAS = GetComponent<AudioSource>();
        myAS.loop = true;
        if (musicToPlay.Count > 0)
        {
           GoToNextSong();
        }
        else
        {
            print("no music to play");
        }
    }

    // Update is called once per frame
    private void OnMouseUp()
    {
        if (currentSongIndex < musicToPlay.Count)
            GoToNextSong();
        else
        {
            myAS.Stop();
            currentSongIndex = 0;

        }
        
    }


    void GoToNextSong()
    {
        myAS.Stop();
        myAS.clip = musicToPlay[currentSongIndex];
        myAS.Play();
        currentSongIndex++;
    }

    void ResetSong()
    {
        currentSongIndex = 0;
        myAS.clip = musicToPlay[currentSongIndex];
        myAS.Play();
    }
}
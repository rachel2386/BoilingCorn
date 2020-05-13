using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour
{
    [Serializable]
    public struct NamedAudioClips
    {
        public string name;
        public AudioClip clip;
    }

    public NamedAudioClips[] audioClips;
    public AudioSource generatedSource;


    private void Awake()
    {
        generatedSource = GetComponent<AudioSource>();
        generatedSource.spatialBlend = 1;
    }

    public void  PlayAudioClipWithSource(AudioClip clipToPlay, AudioSource source, float volume = 1)
    {
        
        source.PlayOneShot(clipToPlay);
        source.volume = volume;


    }

    public void PlaySoundAtPostion(AudioClip clipToPlay, Vector3 position , float volume = 1)
    {

        generatedSource.volume = volume;
        generatedSource.transform.position = position;
        generatedSource.PlayOneShot(clipToPlay);
    }

    public List<AudioClip> SearchLibraryWithClipsOfSameType(string keyword)
    {
        List<AudioClip>clipsToReturn = new List<AudioClip>();
        
        foreach (var c in audioClips)
        {
            if (c.name.Contains(keyword))
            {
                clipsToReturn.Add(c.clip);
            }
        }

        return clipsToReturn;
    }

    public void PlayRandomSoundsAtPosition(List<AudioClip>clipsToPlay, Vector3 position)
    {

        PlaySoundAtPostion(clipsToPlay[Random.Range(0,clipsToPlay.Count)], position);
    }

    public AudioClip FindClipWithName(string name)
    {
        return Array.Find(audioClips, x => x.name == name).clip;
    }
}

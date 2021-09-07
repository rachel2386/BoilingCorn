using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

   
    
    [Serializable]
    public struct NamedAudioClips
    {
        public string name;
        public AudioClip clip;
    }
    
    public struct NamedAudioCollection
    {
        public string name;
        public AudioClip[] clips;
    }

   

    public NamedAudioClips[] audioClips;
    public List<NamedAudioCollection> AudioCollections = new List<NamedAudioCollection>();
    private AudioSource generatedSource;


    private void Awake()
    {
        instance = this;
        generatedSource = GetComponent<AudioSource>();
        generatedSource.spatialBlend = 1;
    }

   
    public void  PlayAudioClipWithSource(AudioClip clipToPlay, AudioSource source, float volume = 1)
    {

        if (source == null) source = generatedSource;
        source.PlayOneShot(clipToPlay);
        source.volume = volume;


    }

    public void PlaySoundAtPostion(AudioClip clipToPlay, AudioSource audioSource, Vector3 position, float volume = 1)
    {

        if (audioSource == null)
        {
            audioSource = generatedSource;
            audioSource.transform.position = position;
        }
        audioSource.volume = volume;
        audioSource.PlayOneShot(clipToPlay);
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

    public List<AudioClip> SearchCollectionsWithName(string keyword)
    {
        List<AudioClip>clipsToReturn = new List<AudioClip>();
        
        foreach (var c in AudioCollections)
        {
            if (c.name.Contains(keyword))
            {
                foreach (var clip in c.clips)
                {
                    clipsToReturn.Add(clip);
                }
                
            }
        }

        return clipsToReturn;
    }

    public void PlayRandomSoundsAtPosition(List<AudioClip>clipsToPlay, AudioSource source, Vector3 position, float volume = 1)
    {

        PlaySoundAtPostion(clipsToPlay[Random.Range(0,clipsToPlay.Count)],null, position);
    }

    public AudioClip FindClipWithName(string name)
    {
        return Array.Find(audioClips, x => x.name == name).clip;
    }
}

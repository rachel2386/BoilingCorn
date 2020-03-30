using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CornMonologueManager : MonoBehaviour
{
    public MonologueScriptableObj myMonologueList;
    public AudioSource MonologueAudio;
    public Queue<string> sentences = new Queue<string>();
   private bool textIsPlaying = false;
   public bool TextIsPlaying => textIsPlaying;

   private bool monologueIsComplete = true;

   public bool MonologueIsComplete => monologueIsComplete;

   private bool debug_ForceStop = false;

   public Text _monologueTextbox;

    public GameObject _TextPanel;

    public int textAnimSpeed = 20;
    // Start is called before the first frame update
    void Start()
    {
        _TextPanel.SetActive(false);
        _monologueTextbox.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("Force End Dialogue");
            debug_ForceStop = true;
            EndMonologue();
            if(MonologueAudio.isPlaying)
                MonologueAudio.Stop();
        }
    }

    public void StartMonologue(string nameOfMonologue)
    {

       //if(!MonologueIsComplete) return;
        _monologueTextbox.text = "";
        var monologueToPlay = myMonologueList.GetMonologueFromName(nameOfMonologue);
        if ( monologueToPlay== null //list doesnot contain monologue of name 
             ) // monologue is empty
        {
            print("no monologue with name " + nameOfMonologue + " found");
            return; 
        }

        if (monologueToPlay.WithVoiceOver && monologueToPlay.VoiceOverClip != null)
        {
            StartVoiceOver(monologueToPlay);
        }

        if (monologueToPlay.displayText == false || monologueToPlay.MonologueText.Length == 0)
        {
            print("no text to show for " + monologueToPlay.Name);
            return;
        }

        monologueIsComplete = false;
        char[]separators = new []{'\r', '\n'}; // separate strings by a new line
        string[] monologueLines = monologueToPlay.MonologueText.Split(separators,StringSplitOptions.RemoveEmptyEntries);
        
        sentences.Clear();
        
       foreach (string sentence in monologueLines)
       {
           sentences.Enqueue(sentence);
       }

       string currentSentence = sentences.Dequeue();
       
       
        _TextPanel.SetActive(true);
       Tween playMonologue = _monologueTextbox.DOText(currentSentence, textAnimSpeed, true);

      
       MonologuePlaying();
          
       playMonologue.OnComplete(NoMonologuePlaying);
       playMonologue.SetSpeedBased(true);


    }

    void StartVoiceOver(Monologue monologueToPlay)
    {
        
        if(MonologueAudio.isPlaying)
            MonologueAudio.Stop();
        MonologueAudio.PlayOneShot( monologueToPlay.VoiceOverClip);
        monologueIsComplete = false;
        StartCoroutine(MonologueAudioIsPlaying());
        
    }

    IEnumerator MonologueAudioIsPlaying()
    {
        

        while (MonologueAudio.isPlaying && !debug_ForceStop)
        {
            yield return true;

        }
        yield return false;
        EndMonologue();
        
    }

    public void DisplayNextSentence()
    {
        if(TextIsPlaying) return; // if text is playing, cannot go to next sentence
        if (sentences.Count == 0) // if all sentences are played, end monologue.
        {
            EndMonologue();
            return;
        }

        _monologueTextbox.text = ""; //clear text;
        string currentSentence = sentences.Dequeue();
       Tween playMonologue = _monologueTextbox.DOText(currentSentence, textAnimSpeed, true);
       playMonologue.SetSpeedBased(true);
       
       MonologuePlaying();
       playMonologue.OnComplete(NoMonologuePlaying);
    }

    public void EndMonologue()
    {
        Debug.Log("end of monologue");
        monologueIsComplete = true;
        _TextPanel.GetComponent<PlayMakerFSM>().SetState("EndOfDialogue");
        debug_ForceStop = false;
    }
    
    void MonologuePlaying()                                                                                                                                                                                                            
    {
        textIsPlaying = true;
    }

    void NoMonologuePlaying()
    {
        textIsPlaying = false; 
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CornMonologueManager : MonoBehaviour
{
    public MonologueScriptableObj myMonologueList;
    public Queue<string> sentences = new Queue<string>();
   private bool textIsPlaying = false;
   public bool TextIsPlaying => textIsPlaying;

   private bool monologueIsComplete = true;

   public bool MonologueIsComplete => monologueIsComplete;

   public Text _monologueTextbox;

    public GameObject _TextPanel;

    public int textAnimSpeed = 20;
    // Start is called before the first frame update
    void Start()
    {
        _TextPanel.SetActive(false);
        _monologueTextbox.text = "";
    }

    public void StartMonologue(string nameOfMonologue)
    {

       //if(!MonologueIsComplete) return;
        _monologueTextbox.text = "";
        
        if (myMonologueList.GetMonologueFromName(nameOfMonologue) == null //list doesnot contain monologue of name 
            || myMonologueList.GetMonologueFromName(nameOfMonologue).MonologueText.Length == 0) // monologue is empty
        {
            print("no monologue with name " + nameOfMonologue + " found");
            return; 
        }
        
        char[]separators = new []{'\r', '\n'}; // separate strings by a new line
        string[] monologueLines = myMonologueList.GetMonologueFromName(nameOfMonologue).MonologueText.Split(separators,StringSplitOptions.RemoveEmptyEntries);
        
        sentences.Clear();
        
       foreach (string sentence in monologueLines)
       {
           sentences.Enqueue(sentence);
       }

       string currentSentence = sentences.Dequeue();
       
       
        _TextPanel.SetActive(true);
       Tween playMonologue = _monologueTextbox.DOText(currentSentence, textAnimSpeed, true);

       monologueIsComplete = false;
       MonologuePlaying();
          
       playMonologue.OnComplete(NoMonologuePlaying);
       playMonologue.SetSpeedBased(true);


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

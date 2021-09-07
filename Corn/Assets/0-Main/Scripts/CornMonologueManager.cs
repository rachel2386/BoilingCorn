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
    public Queue<Sprite> spritesToPlay = new Queue<Sprite>();
    private bool textIsPlaying = false;
    public bool TextIsPlaying => textIsPlaying;

    private bool monologueIsComplete = true;

    public bool MonologueIsComplete => monologueIsComplete;

    private bool debug_ForceStop = false;

    public Text _monologueTextbox;

    public GameObject _TextPanel;

    public Image SpriteHolder;

    public int textAnimSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        _TextPanel.SetActive(false);
        _monologueTextbox.text = "";

        SpriteHolder.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("Force End Dialogue");
            debug_ForceStop = true;
            EndMonologue();
            if (MonologueAudio.isPlaying)
                MonologueAudio.Stop();
        }
    }

    public void StartMonologue(string nameOfMonologue)
    {
        //if(!MonologueIsComplete) return;
        _monologueTextbox.text = "";
        SpriteHolder.sprite = null;
        var monologueToPlay = myMonologueList.GetMonologueFromName(nameOfMonologue);


        if (monologueToPlay == null //list doesnot contain monologue of name 
        ) // monologue is empty
        {
            print("no monologue with name " + nameOfMonologue + " found");
            return;
        }

        StartVoiceOver(monologueToPlay);
        StartSprite(monologueToPlay);
        StartText(monologueToPlay);
    }


    void StartVoiceOver(Monologue monologueToPlay)
    {
        if (!monologueToPlay.WithVoiceOver || monologueToPlay.VoiceOverClip == null)
        {
            print("no clip to play for " + monologueToPlay.Name);
            return;
        }


        if (MonologueAudio.isPlaying)
            MonologueAudio.Stop();
        MonologueAudio.PlayOneShot(monologueToPlay.VoiceOverClip);
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


    void StartSprite(Monologue monologueToPlay)
    {
        if (!monologueToPlay.displaySprite || monologueToPlay.monologueSprite.Count == 0)
        {
            print("no sprite to play for " + monologueToPlay.Name);
            return;
        }


        monologueIsComplete = false;

        foreach (var sprite in monologueToPlay.monologueSprite)
        {
            
            spritesToPlay.Enqueue(sprite);
        }
       
        Sprite currentSprite = spritesToPlay.Dequeue();
        SpriteHolder.sprite = currentSprite;
        
        if(currentSprite == null)
            Debug.LogError("no sprite to display for " + monologueToPlay);
        else
        {
            SpriteHolder.gameObject.SetActive(true);
        
//            Tween playSprite = SpriteHolder.DOColor(SpriteHolder.color, 1);
//            MonologuePlaying();
//            playSprite.OnComplete(NoMonologuePlaying);
        }

       
    }


    void StartText(Monologue monologueToPlay)
    {
        if (!monologueToPlay.displayText || monologueToPlay.MonologueText.Length == 0)
        {
            print("no text to show for " + monologueToPlay.Name);
            return;
        }

        monologueIsComplete = false;

        char[] separators = new[] {'\r', '\n'}; // separate strings by a new line
        string[] monologueLines =
            monologueToPlay.MonologueText.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        sentences.Clear();

        foreach (string sentence in monologueLines)
        {
            sentences.Enqueue(sentence);
        }

        string currentSentence = sentences.Dequeue();


        _TextPanel.SetActive(true);
        _monologueTextbox.text = currentSentence;
        
        Tween playMonologue = _monologueTextbox.DOText(currentSentence, 2, true);
        MonologuePlaying();

        playMonologue.OnComplete(NoMonologuePlaying);
        
    }

    public void DisplayNextSprite()
    {
        if (TextIsPlaying) return; 
        if (spritesToPlay.Count == 0)
        {
            EndMonologue();
            return;
        }

        Sprite currentSprite = spritesToPlay.Dequeue();
        SpriteHolder.sprite = currentSprite;
//        Tween playSprite = SpriteHolder.DOColor(SpriteHolder.color, 1);
//        MonologuePlaying();
//        playSprite.OnComplete(NoMonologuePlaying);
    }

    public void DisplayNextSentence()
    {
        if (TextIsPlaying) return; // if text is playing, cannot go to next sentence
        if (sentences.Count == 0) // if all sentences are played, end monologue.
        {
            EndMonologue();
            return;
        }


        _monologueTextbox.text = ""; //clear text;
        string currentSentence = sentences.Dequeue();
        _monologueTextbox.text = currentSentence;
        Tween playMonologue = _monologueTextbox.DOText(currentSentence, 2, true);
        

        MonologuePlaying();
        playMonologue.OnComplete(NoMonologuePlaying);
    }

    private void EndMonologue()
    {
       monologueIsComplete = true;
        _TextPanel.GetComponent<PlayMakerFSM>().SetState("EndOfDialogue");
        SpriteHolder.GetComponent<PlayMakerFSM>().SetState("EndOfDialogue");
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
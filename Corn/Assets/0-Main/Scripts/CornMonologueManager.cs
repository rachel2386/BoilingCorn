using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;


public class CornMonologueManager : MonoBehaviour
{
    public MonologueScriptableObj myMonologueList;
    public AudioSource MonologueAudio;
    public Queue<string> sentences = new Queue<string>();
    public Queue<Sprite> spritesToPlay = new Queue<Sprite>();
    [SerializeField] private Queue<string> SpriteTranslationNames = new Queue<string>();
    private bool textIsPlaying = false;
    public bool TextIsPlaying => textIsPlaying;

    private bool monologueIsComplete = true;

    public bool MonologueIsComplete => monologueIsComplete;

    private bool debug_ForceStop = false;

    public Text _monologueTextbox;

    public GameObject _TextPanel;

    public Image SpriteHolder;
    [SerializeField] private LeanLocalizedImage MonologueLeanLocalizedImageComponent;
    

    public int textAnimSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        _TextPanel.SetActive(false);
        _monologueTextbox.text = "";

        SpriteHolder.gameObject.SetActive(false);
        
            //LocalizedTextComponent.TranslationName
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

        foreach (var name in monologueToPlay.SpriteLeanTranslationNames)
        {
            SpriteTranslationNames.Enqueue(name);

        }    
       
        //Sprite currentSprite = spritesToPlay.Dequeue();
       // SpriteHolder.sprite = currentSprite;

        string currentTranslationName = SpriteTranslationNames.Dequeue();
        MonologueLeanLocalizedImageComponent.TranslationName = currentTranslationName;

        //if (currentSprite == null)
        if(currentTranslationName == null)
            Debug.LogError("no sprite to display for " + monologueToPlay);
        else
        {
            SpriteHolder.gameObject.SetActive(true); //this activates playmaker FSM logic
        
//            Tween playSprite = SpriteHolder.DOColor(SpriteHolder.color, 1);
//            MonologuePlaying();
//            playSprite.OnComplete(NoMonologuePlaying);
        }

       
    }


    void StartText(Monologue monologueToPlay)
    {
        //Lean localization translation name must match name of monologue  
        var textString = LeanLocalization.GetTranslationText(monologueToPlay.Name);

        if (!monologueToPlay.displayText || monologueToPlay.MonologueText.Length == 0 || textString == null)
        {
            print("no text to show for " + monologueToPlay.Name);
            return;
        }


        monologueIsComplete = false;

        char[] separators = new[] {'\r', '\n'}; // separate strings by a new line
        string[] monologueLines =
            textString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            //monologueToPlay.MonologueText.Split(separators, StringSplitOptions.RemoveEmptyEntries);

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

    public void DisplayNextSprite() //called from playmaker FSM on spriteHolder
    {
        if (TextIsPlaying) return; 
       // if (spritesToPlay.Count == 0)
         if(SpriteTranslationNames.Count == 0)
        {
            EndMonologue();
            return;
        }
        string currentTranslationName = SpriteTranslationNames.Dequeue();
        MonologueLeanLocalizedImageComponent.TranslationName = currentTranslationName;

       // Sprite currentSprite = spritesToPlay.Dequeue();
        //SpriteHolder.sprite = currentSprite;
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
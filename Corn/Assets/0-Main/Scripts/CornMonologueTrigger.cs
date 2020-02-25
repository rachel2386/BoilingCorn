using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CornMonologueTrigger : MonoBehaviour
{
    public string[] MonologuesToPlay; //the monologue to trigger
    private int numberOfTimesClicked = 0;
    private CornMonologueManager _monologueManager;
    // Start is called before the first frame update
    void Start()
    {
        _monologueManager = FindObjectOfType<CornMonologueManager>();
    }

    // Update is called once per frame
    private void OnMouseDown()
    {
       if( _monologueManager.MonologueIsComplete &&
            !_monologueManager.TextIsPlaying && GameManager.gameState >0 && GameManager.gameState < 3)
        {
            numberOfTimesClicked++;
            if(numberOfTimesClicked <= MonologuesToPlay.Length)
                _monologueManager.StartMonologue(MonologuesToPlay[numberOfTimesClicked - 1]);
        }

       
    }
}

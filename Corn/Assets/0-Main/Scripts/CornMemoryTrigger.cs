using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CornMemoryTrigger : MonoBehaviour
{
    //public string[] MemoriesToPlay; //the monologue to trigger
    public string NameOfMemory;
    private Queue<Sprite> MemoriesToPlay = new Queue<Sprite>();
    private int numberOfTimesClicked = 0;
    private CornMonologueManager _monologueManager;
    public ItemMemoryScriptableObj itemMemoryProfile;
    private ItemMemory _myMemory;
    
    

    private MemoryDisplayControl _memoryDisplayControl;
    // Start is called before the first frame update
    void Start()
    {
        _monologueManager = FindObjectOfType<CornMonologueManager>();
        _memoryDisplayControl = FindObjectOfType<MemoryDisplayControl>();

        _myMemory = itemMemoryProfile.GetMemoryWithName(NameOfMemory);
        
        
       
        if ( _myMemory == null ||  _myMemory.MemoriesToDisplay.Count == 0)
        {
            Debug.LogError(gameObject.name + "with the memory name of " + NameOfMemory + " has script attached but has no memory to display");
            gameObject.tag = "Untagged";
            Destroy(this);
            return;
        }

        gameObject.tag = "Look";
        gameObject.layer = 13; // ignore collision
        foreach (var sprite in itemMemoryProfile.GetMemoryWithName(NameOfMemory).MemoriesToDisplay)
        {
            MemoriesToPlay.Enqueue(sprite);
        }
       

    }

    private void Update()
    {
        if (MemoriesToPlay.Count == 0)
        {
            gameObject.tag = "Untagged";
        }
    }

    // Update is called once per frame
    private void OnMouseDown()
    {
       if( _monologueManager.MonologueIsComplete
           && !_memoryDisplayControl.MemoryPlaying
           && GameManager.gameState >= 1
           && GameManager.gameState < 3
           && MemoriesToPlay.Count > 0)
        {
            
            _memoryDisplayControl.MemoryTrigger(MemoriesToPlay.Dequeue());
//            numberOfTimesClicked++;
//            if(numberOfTimesClicked <= MemoriesToPlay.Length)
//                _monologueManager.StartMonologue(MemoriesToPlay[numberOfTimesClicked - 1]);
        }

       
    }
}

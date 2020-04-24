using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MemoryDisplayControl : MonoBehaviour
{
    
    public Image MemoryImageHolder;
    [HideInInspector] public bool MemoryPlaying = false;
    private List<Image> memoryImageFrames = new List<Image>();
    
    void Start()
    {
        MemoryImageHolder.sprite = null;
        
        //add all frames into one list to control them together
        foreach (var child in transform.GetComponentsInChildren<Image>())
        {
            if (!child.GetComponent<Mask>())
            {
               memoryImageFrames.Add(child);
              // change sprite opacity to 0;
               var color = child.color;
               color.a = 0;
               child.color = color;
            }
        
        }
    }

    public void MemoryTrigger(Sprite memorySpriteToDisplay)
    {
        StartCoroutine(DisplayMemory(memorySpriteToDisplay));
    }

    private IEnumerator DisplayMemory(Sprite memorySpriteToDisplay)
    {
         
        MemoryPlaying = true;
      
            
        Tween memoryFadein = null; 
        for (int i = 0; i < memoryImageFrames.Count; i++)
        {
            memoryFadein =memoryImageFrames[i].DOFade(0.8f, 3);
        }
        MemoryImageHolder.sprite =memorySpriteToDisplay;
            
        //Tween memoryFadein = _foodMemoryHolder.DOFade(0.8f, 3);
        yield return memoryFadein.WaitForCompletion();

        yield return new WaitForSeconds(2);

        Tween memoryFadeOut = null;
        for (int i = 0; i <memoryImageFrames.Count; i++)
        {
            memoryFadeOut =memoryImageFrames[i].DOFade(0, 2);
                
        }
                
        yield return memoryFadeOut.WaitForCompletion();
        MemoryImageHolder.sprite = null;
        MemoryPlaying = false;
        
    }
}

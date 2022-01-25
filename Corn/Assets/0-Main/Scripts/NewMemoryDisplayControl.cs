using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NewMemoryDisplayControl : MonoBehaviour
{
    
    
    [SerializeField]private Image MemoryImageHolder;
    [SerializeField] private Animator memoryBubbleAnimator;
    [SerializeField] private float memoryDisplayTimeInSeconds = 2;
    [HideInInspector] public bool MemoryPlaying = false;
    
    void Start()
    {
        InitImages();
    }

    void InitImages()
    {
        MemoryImageHolder.sprite = null;
        GetComponent<Image>().color =  new Color(1, 1, 1, 1);;
        
        if(memoryBubbleAnimator !=null)
            memoryBubbleAnimator.SetBool("PlayMemory", false);
        else
            Debug.LogError("no animator attached to memory image holder");
        
        //set image holder alpha to 0;
        //Get animator 
        //Get animator bool
    }

    public void MemoryTrigger(Sprite memorySpriteToDisplay)
    {
        StartCoroutine(DisplayMemory(memorySpriteToDisplay));
    }

    private IEnumerator DisplayMemory(Sprite memorySpriteToDisplay)
    {
        
        MemoryPlaying = true;
      
        memoryBubbleAnimator.SetBool("PlayMemory", true);
        MemoryImageHolder.sprite =memorySpriteToDisplay;
           
       
        yield return new WaitForSeconds(memoryDisplayTimeInSeconds);
       
       
        memoryBubbleAnimator.SetBool("PlayMemory", false);

        while (!memoryBubbleAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            yield return null;
        }
        MemoryImageHolder.sprite = null;
        MemoryPlaying = false;
        
    }
}

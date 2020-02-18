﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CornUIManager : MonoBehaviour
{
    // Start is called before the first frame update
     private Image ImgSlot;
    public Sprite foodCursor;
    public Sprite defaultCursor;
    public Sprite interactableCursor;

    public Image InteractInstruction;
    public Image EatButtonInstruction;
    public Image LeanInstruction;
    public Image EndGameInstruction;
    
    
    private Camera MyCam; 

    void Start()
    {
        MyCam = Camera.main;
        ImgSlot = GameObject.Find("Reticle").GetComponent<Image>();
//        defaultCursor = transform.Find("DefaultCursor").GetComponent<Image>().mainTexture;
//        foodCursor = transform.Find("FoodCursor").GetComponent<Image>();
//        interactableCursor = transform.Find("InteractableCursor").GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo = new RaycastHit();

        
        
        if (!Physics.Raycast(MyCam.ScreenPointToRay(Input.mousePosition), out hitInfo) ||
            hitInfo.collider == null || Input.GetMouseButton(0)) return;
        
        if (hitInfo.collider.CompareTag("FoodItem") && GameManager.gameState == 1)
        {
            ImgSlot.sprite = foodCursor;
        }
        else if(hitInfo.collider.CompareTag("Interactable") || hitInfo.collider.CompareTag("Pickupable"))
        {
            ImgSlot.sprite = interactableCursor;
        }
        else
        {
            ImgSlot.sprite = defaultCursor;
        }




    }

    void UpdateUI()
    {
        //if(GameManager.gameState == 1)
            
    }

}

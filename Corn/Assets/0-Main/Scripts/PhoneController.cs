using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private Transform[] phonePositions;
    [SerializeField] private PlayMakerFSM PhoneMenuFSM;
    [SerializeField] private bool showPhone = false;


    private void Start()
    {
        showPhone = false;
       
    }

    private void OnMouseUp()
    {
        if(GameManager.gameState == 1 || GameManager.gameState == 2)
            
            if (!showPhone) 
            {
                PhoneMenuFSM.SendEvent("ShowPhone");
                showPhone = true;
            }

       
            
    }

    private void Update()
    {
        if(GameManager.gameState == 1 || GameManager.gameState == 2)
            if (Input.GetMouseButtonUp(1) && showPhone)
            {
                PhoneMenuFSM.SendEvent("HidePhone");
                showPhone = false;
            }

       
    }
}

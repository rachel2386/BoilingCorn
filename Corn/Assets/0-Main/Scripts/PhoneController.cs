using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private Transform[] phonePositions;
    [SerializeField] private PlayMakerFSM PhoneMenuFSM;
    [SerializeField] private Transform phoneMenuCollider;
    [SerializeField] private bool showPhone = false;


    private void Start()
    {
        showPhone = false;
       
    }



    private void Update()
    {
        if (GameManager.gameState == 1 || GameManager.gameState == 2)
        {

            RaycastHit hitInfo = new RaycastHit();
            
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,1000f, ~(1<<1 | 1<<2)))
            {
                if (hitInfo.transform == transform && !showPhone)
                {
                    PhoneMenuFSM.SendEvent("ShowPhone");
                    showPhone = true;
                }

                else if ( showPhone && hitInfo.transform != phoneMenuCollider)
                {
                    PhoneMenuFSM.SendEvent("HidePhone");
                    showPhone = false;
                }
            }

           
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                PhoneMenuFSM.SendEvent(!showPhone ? "ShowPhone" : "HidePhone" );
                showPhone = !showPhone;
            }

           
        }

        

       
    }
}

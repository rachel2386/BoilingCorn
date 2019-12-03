using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CornPickupObject : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera myCam;
    [HideInInspector] public bool IsholdingObject = false;
    
    private GameObject objectHolding;
    private Rigidbody objectRB;
    public Transform objectHolder;
    
    public Transform WaterObjectHolder;
    public Transform PlateObjectHolder;
    public Transform FridgeObjectHolder;
   
    void Start()
    {
        myCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        objectHolder = myCam.transform.Find("ObjectHolder");


    }

    // Update is called once per frame
    

    void FixedUpdate()
    {
        
        if (!IsholdingObject)
        {
            if (Input.GetMouseButtonUp(0))
            {
                
                RaycastHit hitInfo = new RaycastHit();

                {
                    if (Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.collider != null)
                        {
                           
                            if (hitInfo.collider.CompareTag("FoodItem"))
                            {
                                IsholdingObject = true;
                                //GrabFood(hitInfo);
                               InitPickup(hitInfo);
                            }
                            


                        }
                    }
                }
            }
        }

        else
        {
            //MoveObjectToCenter(objectRB);
            
            if (Input.GetMouseButtonUp(0))
            {
                PlaceObject();
              
//                RaycastHit hitInfo = new RaycastHit();
//
//                {
//                    if (Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo))
//                    {
//                        if (hitInfo.collider != null)
//                        {
//                            String tag = ""; 
//                            if(hitInfo.collider.CompareTag(tag))
//                            switch (tag)
//                            {
//                               case "Water": 
//                                PlaceObject(WaterObjectHolder);
//                                   break;
//                               case "Plate": 
//                                   PlaceObject(PlateObjectHolder);
//                                   break;
//                               case "Fridge": 
//                                   PlaceObject(FridgeObjectHolder);
//                                   break;
//                               default:
//                                   PlaceObject();
//                                   break;
//                            }
//                            if (hitInfo.collider.CompareTag("Water"))
//                            {
//                                PlaceObject(WaterObjectHolder);
//                            }
//                            else if (hitInfo.collider.CompareTag("Plate"))
//                            {
//                                PlaceObject(PlateObjectHolder);
//                            }
//                            else if (hitInfo.collider.CompareTag("Fridge"))
//                            {
//                                PlaceObject(FridgeObjectHolder);
//                            }
//                            else
//                            {
//                                PlaceObject();
//                            }
//                        }
//                    }
//                }
            }


        }
    }

    void GrabFood(RaycastHit objectClicked)
    {
        
        objectHolding = objectClicked.collider.gameObject;
        objectRB = objectHolding.GetComponent<Rigidbody>();
        objectRB.useGravity = false;
        objectRB.isKinematic = true;
        objectRB.transform.SetParent(objectHolder);
        //objectHoding.transform.localEulerAngles = Vector3.zero;
        Tween rbRotate = objectRB.transform.DOLocalRotate(Vector3.zero, 0.5f);
        rbRotate.SetEase(Ease.OutExpo);
        Tween rbMove = objectRB.transform.DOLocalMove(Vector3.zero, 0.5f,false);
        rbMove.SetEase(Ease.OutExpo);
        
    }
    
    void PlaceObject(Transform holder = null)
    {
       
        IsholdingObject = false;

        if (holder != null)
        {
            objectHolding.transform.parent = holder.transform;
            Tween rbMove = objectRB.transform.DOLocalMove(Vector3.zero, 0.5f,false);
            rbMove.SetEase(Ease.OutExpo);
        }
        else
            objectHolding.transform.parent = null;
        
        objectRB.useGravity = true;
        objectRB.isKinematic = false;
        objectRB.drag = 0;
        objectRB.angularDrag = 0;
        objectHolder.GetComponent<SpringJoint>().connectedBody = null;
        objectHolding.gameObject.GetComponent<ItemProperties>().HeldByPlayer = false;
        objectHolding = null;
        objectRB = null;
       


    }

    

    void InitPickup(RaycastHit objectClicked)
    {
        
            objectHolding = objectClicked.collider.gameObject;
            objectHolding.gameObject.GetComponent<ItemProperties>().HeldByPlayer = true;
            objectRB = objectHolding.GetComponent<Rigidbody>();
            objectHolder.GetComponent<SpringJoint>().connectedBody = objectRB;



    }

    void MoveObjectToCenter(Rigidbody rbHolding)
    {
        //KeepObjectToCenterOfScreen

        //reset rotation
        Tween rbRotate = objectRB.transform.DOLocalRotate(Vector3.zero, 0.5f);
        rbRotate.SetEase(Ease.OutExpo);

//        Vector3 mousePos = Input.mousePosition;
//        mousePos.z = myCam.WorldToScreenPoint(pickupOffset).z;
//        Vector3 worldMousePos = myCam.ScreenToWorldPoint(mousePos);
//
//        Vector3 MoveDir = objectHolder.position - rbHolding.position;
//        float forceMag = 0.3f;
        
       

    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CornFoodInteractions : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera myCam;
    [HideInInspector] public bool IsholdingObject = false;

    private GameObject objectHolding;
    private Transform foodParent;
    
    private Rigidbody objectRB;
    public Transform objectHolder;
//
//    public Transform WaterObjectHolder;
//    public Transform PlateObjectHolder;
//    public Transform FridgeObjectHolder;
    private Transform bowl;
    private PlayMakerFSM bowlFSM;
    Vector3 bowlRotateOffset = Vector3.zero;
    
    
    
    private AudioSource playerAS;
    [Header("Eating Sounds")]
    public AudioClip eatSound;

    void Start()
    {
        myCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        objectHolder = myCam.transform.Find("ObjectHolder");
        bowl = GameObject.Find("Bowl").transform;
        foodParent = GameObject.Find("Food").transform;
        bowlFSM = bowl.GetComponent<PlayMakerFSM>();
        playerAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame


    void FixedUpdate()
    {
        if (!IsholdingObject)
        {
            RaycastHit hitInfo = new RaycastHit();

            if (Input.GetMouseButtonDown(0)
                && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo)
                && hitInfo.collider != null
                && hitInfo.collider.CompareTag("FoodItem"))
            {
                var foodState = hitInfo.collider.GetComponent<FoodItemProperties>().foodState;
                if (foodState == 0) //if raw, pickup
                {
                    
                    IsholdingObject = true;
                    InitPickup(hitInfo);
                }
                else if(foodState == 1)//if cooked, put into bowl
                {
                    InitPickup(hitInfo);
                    PlaceObject(bowl);
                    hitInfo.collider.GetComponent<FoodItemProperties>().foodState = 2;
                    bowlFSM.Fsm.Variables.BoolVariables[0].Value = true; //new food in, trigger fsm animation
                }
                else //if in bowl, eaten
                {
                   EatFood(hitInfo.collider.gameObject);
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

    void EatFood(GameObject FoodToEat)
    {
        FoodToEat.SetActive(false);
        playerAS.PlayOneShot(eatSound);
    }

    void PlaceObject(Transform holder = null)
    {
        IsholdingObject = false;
        if (holder != null)
        {
            objectHolding.transform.parent = holder.transform;
            Tween rbMove = objectRB.transform.DOLocalMove(Vector3.zero, 0.5f, false);
            rbMove.SetEase(Ease.OutExpo);
           
            Tween rbRotate = objectRB.transform.DOLocalRotate(bowlRotateOffset, 0.5f);
            rbRotate.SetEase(Ease.OutExpo);
            bowlRotateOffset += Vector3.up * 15;
            objectRB.isKinematic = true;
            objectRB.useGravity = false;
        }
        else
        {
            objectRB.isKinematic = false;
            objectHolding.transform.parent = foodParent;
            objectRB.useGravity = true;
        }

        objectRB.drag = 0;
        objectRB.angularDrag = 0;
        //objectHolder.GetComponent<ConfigurableJoint>().connectedBody = null;
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
        //objectHolder.GetComponent<ConfigurableJoint>().connectedBody = objectRB;
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
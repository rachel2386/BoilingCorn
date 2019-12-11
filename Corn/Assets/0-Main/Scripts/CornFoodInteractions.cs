using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
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
        objectHolder = myCam.transform.Find("ObjectHolder");
        bowl = GameObject.Find("BowlPivot").transform;
        foodParent = GameObject.Find("Food").transform;
        bowlFSM = bowl.parent.GetComponent<PlayMakerFSM>();
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
                    StartCoroutine(InsertFrame());
                    
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
            objectHolding.gameObject.GetComponent<ItemProperties>().HeldByPlayer = false;
            holder.RotateAround(holder.parent.position, Vector3.up, 30);
            objectHolding.transform.parent = holder;
            Tween rbMove = objectRB.transform.DOLocalMove(Vector3.zero, 0.5f, false);
            rbMove.SetEase(Ease.OutExpo);
            rbMove.OnComplete(() => { RotatePlatePivot(holder); });
            
            Tween rbRotate = objectRB.transform.DOLocalRotate(Vector3.zero, 0.5f);
            rbRotate.SetEase(Ease.OutExpo);

            objectRB.isKinematic = true;
            objectRB.useGravity = false;
        }
        else
        {
           
            objectHolding.transform.parent = foodParent;
            objectHolding.gameObject.GetComponent<ItemProperties>().HeldByPlayer = false;
            objectHolding = null;
            objectRB = null;
        }

        
       
       
        //objectHolder.GetComponent<ConfigurableJoint>().connectedBody = null;
        objectHolder.GetComponent<SpringJoint>().connectedBody = null;
       
        
       
    }

    void InitPickup(RaycastHit objectClicked)
    {
        objectHolding = objectClicked.collider.gameObject;
        objectHolding.gameObject.GetComponent<ItemProperties>().HeldByPlayer = true;
        objectRB = objectHolding.GetComponent<Rigidbody>();
        objectHolder.GetComponent<SpringJoint>().connectedBody = objectRB;
        //objectHolder.GetComponent<ConfigurableJoint>().connectedBody = objectRB;
    }

    void RotatePlatePivot(Transform pivot)
    {

       objectHolding.transform.parent = pivot.parent;
       //objectRB.velocity = Vector3.zero; 
        
        objectHolding = null;
        objectRB = null;
        
    }

    IEnumerator InsertFrame()
    {
        yield return null;
    }
}
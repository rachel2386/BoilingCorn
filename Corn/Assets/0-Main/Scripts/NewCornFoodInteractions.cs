using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using HutongGames.PlayMaker;
using UnityEngine;
//update 2/22/20: 
//when food state is 0, can only left click 
//when food state is 1, can still left click to pick up and right click to eat 


public class NewCornFoodInteractions : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera myCam;
    public static bool IsholdingObject = false;

    private GameObject objectHolding;
    private Transform foodParent;

    private Rigidbody objectRB;
    public Transform objectHolder;

    private Transform bowl;
    private PlayMakerFSM bowlFSM;

    private AudioSource playerAS;
    [Header("Eating Sounds")] public AudioClip eatSound;
    
    //temp
    public PlayMakerFSM textAnimFSM;

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
                && hitInfo.collider != null)

                if (GameManager.gameState == 1) // in cooking state 
                {
                    if (hitInfo.collider.CompareTag("FoodItem"))
                    {
                        var foodState = hitInfo.collider.GetComponent<NewFoodItemProperties>().foodState;

                        if (foodState == 0) //if raw, pickup
                        {
                            IsholdingObject = true;
                            InitPickup(hitInfo);
                        }
                        else if (foodState == 1) //if cooked, put into bowl
                        {
                            InitPickup(hitInfo);
                            PlaceObject(bowl);
                            hitInfo.collider.GetComponent<NewFoodItemProperties>().foodState = 2;
                            //bowlFSM.Fsm.Variables.BoolVariables[0].Value = true; //new food in, trigger fsm animation
                            StartCoroutine(InsertFrame());
                        }
                        else if(foodState == 2  && clicked)//if in bowl, eaten
                        {
                            EatFood(hitInfo.collider.gameObject);
                        }
                    }
//                    else if (hitInfo.collider.CompareTag("Pickupable"))
//                    {
//                       print("pickup plate");
//                        IsholdingObject = true;
//                        InitPickupPlate(hitInfo);
//                    }
                }
                else if (GameManager.gameState == 2)
                {
                        if (hitInfo.collider.CompareTag("Pickupable"))
                    {
                        IsholdingObject = true;
                        ContainerPickUp(hitInfo);
                    
                    }
                }

            if (GameManager.gameState == 1 && Input.GetMouseButtonDown(1)
                                           && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo)
                                           && hitInfo.collider.CompareTag("FoodItem") && hitInfo.collider.GetComponent<NewFoodItemProperties>().foodCooked)
            {
                EatFood(hitInfo.collider.gameObject);
            }


        }
        else
        {
            if (GameManager.gameState == 1)
            {
                if (Input.GetMouseButtonUp(0))
                {

                    PlaceObject();
                }
            }
           else if (GameManager.gameState == 2)
            {
                RaycastHit hitInfo = new RaycastHit();
                if (Input.GetMouseButtonUp(0)
                    && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo)
                    && hitInfo.collider != null
                    && hitInfo.collider.CompareTag("Respawn")) 
                    ContainerDropOff(hitInfo.collider.transform);
             
            }

            
        }
    }

    void EatFood(GameObject FoodToEat)
    {
        FoodToEat.SetActive(false);
        if (!CornItemManager.FoodEaten.Contains(FoodToEat)) CornItemManager.FoodEaten.Add(FoodToEat);

        playerAS.PlayOneShot(eatSound);
    }

    void PlaceObject(Transform holder = null)
    {
        IsholdingObject = false;

        if (holder != null)
        {
            //place in bowl
            objectHolding.gameObject.GetComponent<NewFoodItemProperties>().HeldByPlayer = false;
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
            if (objectHolding.CompareTag("FoodItem"))
            {
                
                objectHolding.gameObject.GetComponent<NewFoodItemProperties>().OnDropOff();
                objectHolding.transform.parent = foodParent;

            }
            else if (objectHolding.CompareTag("Pickupable"))
            {
                objectHolding.gameObject.GetComponent<ItemProperties>().OnDropOff();
                objectHolding.transform.parent = null;
            }
            
            objectHolding = null;
            objectRB = null;
            
        }


        //objectHolder.GetComponent<ConfigurableJoint>().connectedBody = null;
        objectHolder.GetComponent<SpringJoint>().connectedBody = null;
    }

    void InitPickup(RaycastHit objectClicked)
    {
        objectHolding = objectClicked.collider.gameObject;
        objectRB =objectHolding.gameObject.GetComponent<NewFoodItemProperties>().OnPickUp();
        objectHolder.GetComponent<SpringJoint>().connectedBody = objectRB;
        //objectHolder.GetComponent<ConfigurableJoint>().connectedBody = objectRB;
    }

    

    void RotatePlatePivot(Transform pivot)
    {
        objectHolding.transform.parent = pivot.parent;
        

        objectHolding = null;
        objectRB = null;
    }

    void ContainerPickUp(RaycastHit hitInfo)
    {

        
            
        objectHolding = hitInfo.collider.gameObject;

        var ContainerTransform = hitInfo.collider.transform;
        
        ContainerTransform.SetParent(objectHolder);
        
        Tween MoveToHolder = ContainerTransform.DOLocalMove(Vector3.zero + Vector3.right * 0.3f - Vector3.up * 0.2f, 0.3f);
        MoveToHolder.SetEase(Ease.OutQuad);
    }

    void ContainerDropOff(Transform fridgeHolder)
    {
        //update text
        var firstPlatePickup = textAnimFSM.FsmVariables.FindFsmBool("firstPlateIn");
        if (!firstPlatePickup.Value)
            firstPlatePickup.Value = true;
        
           IsholdingObject = false;
           objectHolding.transform.parent = fridgeHolder;
           Tween moveToFridge = objectHolding.transform.DOLocalMove(Vector3.zero, 1f);
           moveToFridge.SetEase(Ease.OutSine);
           Tween ResetRotation = objectHolding.transform.DOLocalRotate(Vector3.zero, 1f);
           ResetRotation.SetEase(Ease.OutSine);
           
           fridgeHolder.GetComponent<FridgeHolderBehavior>().hasChild = true;
           objectHolding = null;

    }

    private bool clicked = false;
    IEnumerator InsertFrame()
    {
        clicked = false;
        yield return null;
        clicked = true;
    }
}
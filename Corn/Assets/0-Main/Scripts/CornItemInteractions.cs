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


//update 3/8 reorganize pickup script
//raycast on pickupable layer
//Get Item Component.Pickup();

public class CornItemInteractions : MonoBehaviour
{
    private CornMonologueManager _monologueManager;

    public bool playerIsFull = false;

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

    public void Initiate()
    {
        _monologueManager = FindObjectOfType<CornMonologueManager>();
        myCam = Camera.main;
        objectHolder = myCam.transform.Find("ObjectHolder");

        //bowl = GameObject.Find("BowlPivot").transform;
        foodParent = GameObject.Find("Food").transform;
        // bowlFSM = bowl.parent.GetComponent<PlayMakerFSM>();
        playerAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame

    private void Update()
    {
        if (GameManager.gameState == 1 && Input.GetKeyDown(KeyCode.Return) && !playerIsFull )
        {
            playerIsFull = true;
        }
    }

    void FixedUpdate()
    {
        
        RaycastHit hitInfo = new RaycastHit();
                
        if (Input.GetMouseButtonDown(1)
            && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo)
            && hitInfo.collider.CompareTag("FoodItem")
            && hitInfo.collider.GetComponent<NewFoodItemProperties>().foodState == 1)
        {
            MoveFoodToMouth(hitInfo.collider.gameObject);
        }
        
        if (!IsholdingObject)
            {
                
                
                if (GameManager.gameState < 3) // in cooking state 
                {
                   
                    if (Input.GetMouseButtonDown(0)
                        && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo, LayerMask.NameToLayer("Pickupable"))
                        && hitInfo.collider.GetComponent<ItemProperties>())
                    {

                       InitPickup(hitInfo);
                        
                    }

                    
                }
//                else if (GameManager.gameState == 2) // in cleanup state, food not pickupable
//                {
//                    if (Input.GetMouseButtonDown(0)
//                        && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo)
//                        && hitInfo.collider.CompareTag("Pickupable"))
//                    {
//                        IsholdingObject = true;
//                        ContainerPickUp(hitInfo);
//                    }
//                }
            }
            else
            {
//                if (GameManager.gameState ==1)
//                {
                RaycastHit hit = new RaycastHit(); //
//                    && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hit)
//                    && hit.collider != null)
                    if (Input.GetMouseButtonUp(0))
                    
                    {
                      
                        PlaceObject();
                    }
//                }
//                else if (GameManager.gameState == 2)
                {
//                    if (Input.GetMouseButtonUp(0)
//                        && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo)
//                        && hitInfo.collider != null
//                        && hitInfo.collider.CompareTag("Respawn"))
//                        ContainerDropOff(hitInfo.collider.transform);
                }
            }
    }

    void MoveFoodToMouth(GameObject FoodToEat)
    {
        FoodToEat.GetComponent<NewFoodItemProperties>().foodState = 2;
        var mouth = Camera.main.transform;
        FoodToEat.transform.parent = mouth;
        Tween moveToMouth = FoodToEat.transform.DOLocalMove(Vector3.up * -0.12f, 3);
        moveToMouth.SetEase(Ease.InOutSine);
        moveToMouth.OnComplete(() => FoodEaten(FoodToEat));
    }

    void FoodEaten(GameObject FoodToEat)
    {
        FoodToEat.SetActive(false);
        if (!CornItemManager.FoodEaten.Contains(FoodToEat))
            CornItemManager.FoodEaten.Add(FoodToEat); // add to list of eaten food

        playerAS.PlayOneShot(eatSound);

        
        var numOfFoodEaten = CornItemManager.FoodEaten.Count;
        
        switch (numOfFoodEaten)
        {
            case 1:
                _monologueManager.StartMonologue("eat first food");
                break;
            case 2:
                _monologueManager.StartMonologue("eat second food");
                break;
            case 5:
                _monologueManager.StartMonologue("eat fifth food");
                break;
           case 10:
                _monologueManager.StartMonologue("eat tenth food");
                break;
            case 8:
                _monologueManager.StartMonologue("noise from neighbors");
                break;
            case 11:
                _monologueManager.StartMonologue("full");
                playerIsFull = true;
                break;
            default:
                break;
        }
    }

    void PlaceObject()
    {
        IsholdingObject = false;
        objectHolding.gameObject.GetComponent<ItemProperties>().OnDropOff();
        
//            if (objectHolding.CompareTag("FoodItem"))
//            {
//                objectHolding.gameObject.GetComponent<ItemProperties>().OnDropOff(mousePosition);
//                
//            }
//            else if (objectHolding.CompareTag("Pickupable"))
//            {
//                objectHolding.gameObject.GetComponent<ItemProperties>().OnDropOff();
//                objectHolding.transform.parent = null;
//            }

            objectHolding = null;
            objectRB = null;
            objectHolder.GetComponent<SpringJoint>().connectedBody = null;
    }

    void InitPickup(RaycastHit objectClicked)
    {
        IsholdingObject = true; 
        objectHolding = objectClicked.collider.gameObject;
        objectRB = objectClicked.rigidbody;
        objectClicked.collider.GetComponent<ItemProperties>().OnPickUp(objectHolder.GetComponent<SpringJoint>());
//        objectHolder.GetComponent<SpringJoint>().connectedBody = objectRB;
//        //objectHolder.GetComponent<ConfigurableJoint>().connectedBody = objectRB;
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

        Tween MoveToHolder =
            ContainerTransform.DOLocalMove(Vector3.zero + Vector3.right * 0.3f - Vector3.up * 0.2f, 0.3f);
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
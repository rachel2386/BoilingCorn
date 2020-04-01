using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;

//update 2/22/20: 
//when food state is 0, can only left click 
//when food state is 1, can still left click to pick up and right click to eat 


//update 3/8 reorganize pickup script
//raycast on pickupable layer
//Get Item Component.Pickup();

public class CornItemInteractions : MonoBehaviour
{
    private CornMonologueManager _monologueManager;

    [SerializeField] private int fullAmount = 11;
    public bool playerIsFull = false;

    // Start is called before the first frame update
    private Camera myCam;
    public static bool IsholdingObject = false;

    private GameObject objectHolding;


    private Rigidbody objectRB;
    public Transform objectHolder;

    private Transform bowl;
    private PlayMakerFSM bowlFSM;

    private AudioSource playerAS;
    [Header("Eating Sounds")] public AudioClip eatSound;

    [HideInInspector] public Image FoodImage;

    [HideInInspector] public bool FoodMemoryPlaying = false;

    //temp
    public PlayMakerFSM textAnimFSM;

    private void Start()
    {
        playerAS = GetComponent<AudioSource>();

        FoodImage = GameObject.Find("FoodImage").GetComponent<Image>();
        FoodImage.sprite = null;
        _monologueManager = FindObjectOfType<CornMonologueManager>();
        myCam = Camera.main;
        objectHolder = myCam.transform.Find("ObjectHolder");
    }


    private void Update()
    {
        if (GameManager.gameState == 1 && Input.GetKeyDown(KeyCode.Alpha2) && !playerIsFull)
        {
            playerIsFull = true;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.gameState == 3 || GameManager.gameState == 0) return;
        RaycastHit hitInfo = new RaycastHit();

        if (Input.GetMouseButtonDown(1)
            && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, LayerMask.GetMask("Pickupable"))
            && hitInfo.collider.CompareTag("FoodItem")
            && CornItemManager.FoodEaten.Count < fullAmount 
            && hitInfo.collider.GetComponent<NewFoodItemProperties>().foodState == 1)
        {
            MoveFoodToMouth(hitInfo.collider.gameObject);
        }

        if (!IsholdingObject)
        {
            if (Input.GetMouseButtonDown(0)
                && Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo,1000, LayerMask.GetMask("Pickupable"))
                && hitInfo.collider.GetComponent<ItemProperties>())
            {
                InitPickup(hitInfo);
            }
        }
        else
        {
            RaycastHit hit = new RaycastHit(); //

            // object follow mesh 
            if(Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition),  out hit, 1000, LayerMask.GetMask("PUMesh")))
            {
                objectHolder.position = hit.point;
            }
        
               
            if (Input.GetMouseButtonUp(0))

            {
                PlaceObject();
            }
        }
    }

    void MoveFoodToMouth(GameObject FoodToEat)
    {
        FoodToEat.GetComponent<NewFoodItemProperties>().foodState = 2;
        var mouth = Camera.main.transform;
        Tween moveToMouth = FoodToEat.transform.DOMove(mouth.position + Vector3.up * -0.12f, 3);
        moveToMouth.SetEase(Ease.InOutSine);
        moveToMouth.OnComplete(() => FoodEaten(FoodToEat));
    }

    void FoodEaten(GameObject FoodToEat)
    {
        FoodToEat.GetComponent<NewFoodItemProperties>().StartCoroutine(nameof(NewFoodItemProperties.DisplayFoodMemory));
        playerAS.PlayOneShot(eatSound);
        if (!CornItemManager.FoodEaten.Contains(FoodToEat))
            CornItemManager.FoodEaten.Add(FoodToEat); // add to list of eaten food

        


        var numOfFoodEaten = CornItemManager.FoodEaten.Count;

        switch (numOfFoodEaten)
        {
//            case 1:
//                _monologueManager.StartMonologue("eat first food");
//                break;
//            case 2:
//                _monologueManager.StartMonologue("eat second food");
//                break;
            case 5:
                _monologueManager.StartMonologue("eat fifth food");
                break;
            case 10:
                _monologueManager.StartMonologue("eat tenth food");
                break;
//            case 8:
//                _monologueManager.StartMonologue("noise from neighbors");
//                break;
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
    }


    private bool clicked = false;

    IEnumerator InsertFrame()
    {
        clicked = false;
        yield return null;
        clicked = true;
    }
}
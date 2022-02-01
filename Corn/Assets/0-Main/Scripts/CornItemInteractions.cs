using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//update 2/22/20: 
//when food state is 0, can only left click 
//when food state is 1, can still left click to pick up and right click to eat 


//update 3/8 reorganize pickup script
//raycast on pickupable layer
//Get Item Component.Pickup();

public class CornItemInteractions : MonoBehaviour
{
    private CornMonologueManager _monologueManager;
    private CornItemManager _itemManager;

   public int fullAmount = 15;
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
    public Transform mouth;
    [Header("Eating Sounds")] public List<AudioClip> eatSounds = new List<AudioClip>();
    private AudioManager _audioManager;
    private NewMemoryDisplayControl memoryDisplay;

    [HideInInspector]public bool EatingFood = false;
    private List<AudioClip> foodPickUpSounds = new List<AudioClip>();
    
    //temp
    public PlayMakerFSM textAnimFSM;
    public PlayMakerFSM audioEventTrigger;
    

    private void Start()
    {
        playerAS = GetComponent<AudioSource>();
        _audioManager = FindObjectOfType<AudioManager>();
        _itemManager = FindObjectOfType<CornItemManager>();
        
        
        _monologueManager = FindObjectOfType<CornMonologueManager>();
        myCam = Camera.main;
        memoryDisplay = FindObjectOfType<NewMemoryDisplayControl>();
        
        foodPickUpSounds.AddRange(_audioManager.SearchLibraryWithClipsOfSameType("pickUpFood"));
    }


    private void Update()
    {
       

        if (_itemManager.FoodEaten.Count >= fullAmount && !EatingFood && _monologueManager.MonologueIsComplete)
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
            && _itemManager.FoodEaten.Count < fullAmount 
            && hitInfo.collider.GetComponent<NewFoodItemProperties>().foodState == 1
            && !EatingFood)
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
            if(Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition),  out hit, 1000, LayerMask.GetMask("Ignore Raycast")))
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
        EatingFood = true;
        FoodToEat.GetComponent<NewFoodItemProperties>().foodState = 2;
        //var mouth = Camera.main.transform;
        Tween moveToMouth = FoodToEat.transform.DOMove(mouth.position, 2);
        moveToMouth.SetEase(Ease.InOutSine);
        moveToMouth.OnComplete(() => FoodEaten(FoodToEat));
    }

    void FoodEaten(GameObject FoodToEat)
    {
        _audioManager.PlayRandomSoundsAtPosition(eatSounds,playerAS,playerAS.transform.position,1);
        FoodToEat.transform.SetParent(mouth);
        FoodToEat.GetComponent<NewFoodItemProperties>().StartCoroutine(nameof(NewFoodItemProperties.BiteFood));
       
        if (!_itemManager.FoodEaten.Contains(FoodToEat))
            _itemManager.FoodEaten.Add(FoodToEat); // add to list of eaten food
        
        var numOfFoodEaten =_itemManager.FoodEaten.Count;
        
//        if(numOfFoodEaten < fullAmount)
//        FoodToEat.GetComponent<NewFoodItemProperties>().StartCoroutine(nameof(NewFoodItemProperties.DisplayFoodMemory));

        if (numOfFoodEaten == Mathf.RoundToInt(fullAmount / 2))
        {
            audioEventTrigger.SendEvent("police");
        }
        else if (numOfFoodEaten == fullAmount - 1)
        {
            _monologueManager.StartMonologue("eat tenth food");
        }
        else if (numOfFoodEaten == fullAmount)
        {
            _monologueManager.StartMonologue("full");
            _audioManager.PlayAudioClipWithSource(_audioManager.FindClipWithName("feelFull"),playerAS,1f);
        }
        
       
    }

    public void FoodMemoryTrigger(GameObject FoodEaten, Sprite spriteToDisplay)
    {
        StartCoroutine(DisplayFoodMemory(FoodEaten, spriteToDisplay));
         _itemManager.memoriesCollected++;
    }

    private IEnumerator DisplayFoodMemory(GameObject FoodEaten, Sprite spriteToDisplay)
    {
        var randomNumber = Random.Range(0, 1);
        if (memoryDisplay.MemoryPlaying|| randomNumber == 1) //chances to play food memory is 1/2
        {
            FoodEaten.SetActive(false);
            yield return null;

        }
        else
        {
            memoryDisplay.MemoryTrigger(spriteToDisplay);
            while (memoryDisplay.MemoryPlaying)
            {
                yield return null;
            }
           
            FoodEaten.SetActive(false);
      
            
        }
    }
    public void PlaceObject()
    {
        IsholdingObject = false;
        objectHolding.gameObject.GetComponent<ItemProperties>().OnDropOff();
        objectHolding = null;
        objectRB = null;
        //objectHolder.GetComponent<SpringJoint>().connectedBody = null;
        objectHolder.GetComponent<Joint>().connectedBody = null;
    }

    void InitPickup(RaycastHit objectClicked)
    {
        IsholdingObject = true;
        objectHolding = objectClicked.collider.gameObject;
        objectRB = objectClicked.rigidbody;
        objectClicked.collider.GetComponent<ItemProperties>().OnPickUp(objectHolder.GetComponent<Joint>());
//        if(objectHolding.GetComponent<NewFoodItemProperties>().InWater)
//            _audioManager.PlayRandomSoundsAtPosition(foodPickUpSounds, objectRB.position);
        
    }

}
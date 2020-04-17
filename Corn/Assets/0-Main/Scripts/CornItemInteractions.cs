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

    [SerializeField] public int fullAmount = 11;
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
    [Header("Eating Sounds")] public AudioClip eatSound;

    [HideInInspector] public Image FoodImage;
    [HideInInspector]public GameObject _foodMemoryBG;
    [HideInInspector]public List<Image> _foodMemoryHolders = new List<Image>();

    [HideInInspector]public bool EatingFood = false;
    [HideInInspector] public bool FoodMemoryPlaying = false;
    
    //temp
    public PlayMakerFSM textAnimFSM;

    private void Start()
    {
        playerAS = GetComponent<AudioSource>();

        FoodImage = GameObject.Find("FoodImage").GetComponent<Image>();
        FoodImage.sprite = null;
        _foodMemoryBG = GameObject.Find("FoodImageBG");
        
        foreach (var child in _foodMemoryBG.GetComponentsInChildren<Image>())
        {
            if (!child.GetComponent<Mask>())
            {
                _foodMemoryHolders.Add(child);
                print("Img added");
            }

            
        }
        _foodMemoryBG.SetActive(false);
        
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

        if (CornItemManager.FoodEaten.Count >= fullAmount && !EatingFood && _monologueManager.MonologueIsComplete)
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
            if(!EatingFood)
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
        FoodToEat.transform.SetParent(mouth);
        FoodToEat.GetComponent<NewFoodItemProperties>().StartCoroutine(nameof(NewFoodItemProperties.BiteFood));
       
        if (!CornItemManager.FoodEaten.Contains(FoodToEat))
            CornItemManager.FoodEaten.Add(FoodToEat); // add to list of eaten food
        
        var numOfFoodEaten = CornItemManager.FoodEaten.Count;
        
//        if(numOfFoodEaten < fullAmount)
//        FoodToEat.GetComponent<NewFoodItemProperties>().StartCoroutine(nameof(NewFoodItemProperties.DisplayFoodMemory));
        
        switch (numOfFoodEaten)
        {
//            case 1:
//                _monologueManager.StartMonologue("eat first food");
//                break;
//            case 2:
//                _monologueManager.StartMonologue("eat second food");
//                break;
            case 5:
                _monologueManager.StartMonologue("police");
                break;
            case 10:
                _monologueManager.StartMonologue("eat tenth food");
                break;
//            case 8:
//                _monologueManager.StartMonologue("noise from neighbors");
//                break;
            
            default:
                break;
        }
    }

    public void FoodMemoryTrigger(GameObject FoodEaten, Sprite spriteToDisplay)
    {
        StartCoroutine(DisplayFoodMemory(FoodEaten, spriteToDisplay));
    }

    private IEnumerator DisplayFoodMemory(GameObject FoodEaten, Sprite spriteToDisplay)
    {
        var randomNumber = Random.Range(0, 2);
        if (FoodMemoryPlaying || randomNumber == 1) //chances to play food memory is 1/2
        {
            yield return null;

        }
        else
        {
           
            
            FoodMemoryPlaying = true;
            _foodMemoryBG.SetActive(true);
            
            Tween memoryFadein = null; 
            for (int i = 0; i < _foodMemoryHolders.Count; i++)
            {
                memoryFadein = _foodMemoryHolders[i].DOFade(0.8f, 3);
            }
            FoodImage.sprite = spriteToDisplay;
            
            //Tween memoryFadein = _foodMemoryHolder.DOFade(0.8f, 3);
            yield return memoryFadein.WaitForCompletion();

            yield return new WaitForSeconds(2);

            Tween memoryFadeOut = null;
            for (int i = 0; i < _foodMemoryHolders.Count; i++)
            {
                memoryFadeOut =_foodMemoryHolders[i].DOFade(0, 2);
                
            }
                
            yield return memoryFadeOut.WaitForCompletion();

            FoodImage.sprite = null;
            FoodMemoryPlaying = false;
            FoodEaten.SetActive(false);
            _foodMemoryBG.SetActive(false);
            
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
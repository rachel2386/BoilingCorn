using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NewFoodItemProperties : ItemProperties
{
    private FoodProfileManager _foodProfileManager;
    private CornItemInteractions _itemInteractions;
    private CornItemManager _itemManager;
    private FoodProperty myFoodProfile;
    public string FoodName;
    private bool hasFoodProfile = false;
    public int foodState = 0;
    private int raw = 0;
    private int cooked = 1;

    private float SecondsToCook;
    [HideInInspector]public bool inWaterLastFrame = false;

    private AudioSource PlayerAS;
    private AudioManager _audioManager;
    [SerializeField] private float timeCooked = 0;

    private float TimeCooked
    {
        get => timeCooked;
        set => timeCooked = value;
    }

    public float PercentageCooked = 0;
    public bool InWater = false;
    private CornBuoyancy _buoyancyScript;
    private Rigidbody foodRB;

    [HideInInspector] public bool foodCooked = false;


    private bool hasFoodMemory = true;

    public List<Renderer> foodMeshes = new List<Renderer>();


    private void Awake()
    {
        if (!GetComponent<Rigidbody>())
            gameObject.AddComponent<Rigidbody>();

        foodRB = GetComponent<Rigidbody>();


        foreach (var child in GetComponentsInChildren<Renderer>())
        {
            if (child.transform.parent == transform)
                foodMeshes.Add(child);
        }

        for (int i = 1; i < foodMeshes.Count; i++)
        {
            foodMeshes[i].gameObject.SetActive(false);
        }
    }

    public override void Start()
    {
       
        _itemManager = FindObjectOfType<CornItemManager>();
        _foodProfileManager = _itemManager.foodManager;
        _buoyancyScript = FindObjectOfType<CornBuoyancy>();
        _itemInteractions = FindObjectOfType<CornItemInteractions>();
        _audioManager = FindObjectOfType<AudioManager>();
        //_foodMemoryHolder = GameObject.Find("FoodImage").GetComponent<Image>();

        foodRB.WakeUp();
        PlayerAS = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
        ;

        myFoodProfile = _foodProfileManager.GetPropertyFromName(FoodName);


        if (myFoodProfile != null)
        {
            hasFoodProfile = true;
            InitFoodWithProfile();
        }
        else
        {
            hasFoodProfile = false;
            InitFoodGeneric();
        }

        if (SecondsToCook <= 0)
        {
            SecondsToCook = 5;
            print(gameObject.name + " cook time not assigned, using default cook time");
        }
    }

    void InitFoodGeneric()
    {
        // InitOutline();
        hasFoodMemory = false;
    }

    void InitFoodWithProfile()
    {
        SecondsToCook = myFoodProfile.SecondsToCook;

        if (myFoodProfile.foodMemories.Count == 0)
        {
            //print(myFoodProfile.Name + " does not have food memory");
            hasFoodMemory = false;
        }


        // InitOutlineWithProfile();
    }

    private void Update()
    {
        
        if (TimeCooked >= SecondsToCook)
        {
            PercentageCooked = 1;
            if (!foodCooked)
                FoodReady();
        }
        else
        {
            if (foodState == raw && InWater && _buoyancyScript.PotIsBoiling && TimeCooked < SecondsToCook)
                TimeCooked += Time.deltaTime;
            PercentageCooked = TimeCooked / SecondsToCook;
        }

        
    }

    private void LateUpdate()
    {
        inWaterLastFrame = InWater;
    }


    private void FixedUpdate()
    {
        if (HeldByPlayer)
        {
            ChangePhysicsProperties(4, 4, false);
        }
        else
        {
            if (foodState == 2 || foodState == 3)
            {
                foodRB.isKinematic = true;
            }

            if (InWater)
            {
                if (foodState == raw)
                    ChangePhysicsProperties(3, 3, false);
                else if (foodState == cooked)
                    ChangePhysicsProperties(5, 5, false);
            }
            else
            {
                ChangePhysicsProperties(1, 1, true);
            }
        }
    }

    void ChangePhysicsProperties(int drag, int angularDrag, bool useGravity)
    {
        foodRB.drag = drag;
        foodRB.angularDrag = angularDrag;
        foodRB.useGravity = useGravity;
    }


    void FoodReady()
    {
        foodCooked = true;
        foodState = 1;

        if (!CornBuoyancy.cookedFoodInWater.Contains(foodRB))
            CornBuoyancy.cookedFoodInWater.Add(foodRB);
    }

    public override void OnPickUp(SpringJoint objectHolder)
    {
        
        if (foodState != 2) //if food not eaten, pickupable
        {
            var randomNumber = Random.Range(0, 2);
            
            base.OnPickUp(objectHolder);
            if(InWater)
               if (randomNumber == 0)
                _audioManager.PlaySoundAtPostion(_audioManager.FindClipWithName("pickUpFoodWater"),null,transform.position, 0.5f);
               else
                   _audioManager.PlaySoundAtPostion(_audioManager.FindClipWithName("pickUpFoodWater2"),null,transform.position, 0.5f);   
            
            //StartCoroutine(InsertFrame());
        }
    }

    public override void OnDropOff()
    {
        base.OnDropOff();
        

        
    }

    private void DisplayFoodMemory()
    {
        if (!hasFoodMemory || myFoodProfile.foodMemoryQueue.Count <= 0 || _itemManager.FoodEaten.Count >= _itemInteractions.fullAmount)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _itemInteractions.FoodMemoryTrigger(gameObject, myFoodProfile.foodMemoryQueue.Dequeue());
            
        }
    }


    public IEnumerator BiteFood()
    {
        var foodSteps = foodMeshes.Count;
        int currentStep = 0;


        while (currentStep < foodSteps)
        {
            {
                if (currentStep + 1 < foodSteps)
                {
                    foodMeshes[currentStep + 1].gameObject.SetActive(true);
                }

                //PlayerAS.PlayOneShot(_itemInteractions.eatSound);
                foodMeshes[currentStep].gameObject.SetActive(false);
                Tween rotateFood = transform.DORotate(Vector3.one * Random.Range(10, 50), 1);
                yield return rotateFood.WaitForCompletion();

                currentStep++;
            }
        }

        _itemInteractions.EatingFood = false;
       DisplayFoodMemory();
//        while (_itemInteractions.FoodMemoryPlaying)
//        {
//            yield return null;
//        }
    }

//    private void OnMouseEnter()
//    {
//        if (!NewCornFoodInteractions.IsholdingObject)
//            if (hasFoodProfile)
//            {
//                foreach (var child in InitOutlineWithProfile())
//                {
//                    child.enabled = true;
//                }
//            }
//            else
//            {
//                InitOutline().enabled = true;
//            }
//    }
//
//    private void OnMouseExit()
//    {
//        if (!NewCornFoodInteractions.IsholdingObject)
//            if (hasFoodProfile)
//            {
//                foreach (var child in InitOutlineWithProfile())
//                {
//                    child.enabled = false;
//                }
//            }
//            else
//            {
//                InitOutline().enabled = false;
//            }
//    }
}
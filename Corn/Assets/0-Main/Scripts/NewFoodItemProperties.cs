using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewFoodItemProperties : ItemProperties
{
    private FoodProfileManager _itemManager;
    private FoodProperty myFoodProfile;
    public string FoodName;
    private bool hasFoodProfile = false;
    public int foodState = 0;
    private int raw = 0;
    private int cooked = 1;
    private GameObject foodToRender;
    private float SecondsToCook;
   [SerializeField] private float timeCooked = 0;
   
   private float TimeCooked
    {
        get => timeCooked;
        set => timeCooked = value;
    }

    public float PercentageCooked = 0;
    public bool InWater = false;
    private NewBuoyancy _buoyancyScript;
    private Rigidbody foodRB;


    [HideInInspector] public bool foodCooked = false;

    private void Awake()
    {
        if (!GetComponent<Rigidbody>())
            gameObject.AddComponent<Rigidbody>();

        foodRB = GetComponent<Rigidbody>();
    }

    public override void Start()
    {
        _itemManager = GameObject.Find("GameManager").GetComponent<CornItemManager>().foodManager;
        _buoyancyScript = FindObjectOfType<NewBuoyancy>();

        myFoodProfile = _itemManager.GetPropertyFromName(FoodName);

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
    }

    void InitFoodWithProfile()
    {


        
        SecondsToCook = myFoodProfile.SecondsToCook;


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

    private void FixedUpdate()
    {
        if (HeldByPlayer)
        {
            ChangePhysicsProperties(4, 4, false);
        }
        else
        {
            if (foodState == 3)
            {
                foodRB.isKinematic = true;
            }
            else
            {
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

        if (!NewBuoyancy.cookedFoodInWater.Contains(foodRB))
            NewBuoyancy.cookedFoodInWater.Add(foodRB);

        return;
        
        var foodRot = foodToRender.transform.localRotation;
        var foodPos = foodToRender.transform.localPosition;
        var foodScale = foodToRender.transform.localScale;

       // foodToRender = myFoodProfile.CookedPrefab;

        if (foodToRender == null)
        {
            print("cooked prefab not found for" + myFoodProfile.Name);
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }

        var cookedFood = Instantiate(foodToRender, transform, false);
        cookedFood.transform.localPosition = foodPos;
        cookedFood.transform.localRotation = foodRot;
        //cookedFood.transform.localScale = foodScale;


//        CornItemManager.ListOfFood.Add(cookedFood);
//
//        var foodInchildren = cookedFood.GetComponentsInChildren<NewFoodItemProperties>();
//        if (foodInchildren.Length <= 0) return;
//        foreach (var f in foodInchildren)
//        {
//            CornItemManager.ListOfFood.Add(f.gameObject);
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
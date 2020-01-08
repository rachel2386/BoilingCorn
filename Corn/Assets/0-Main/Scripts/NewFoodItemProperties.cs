using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewFoodItemProperties : ItemProperties
{
    private FoodProfileManager _foodManager;
    private FoodProperty myFoodProfile;
    public string FoodName;
    
    public int foodState = 0;
    private int raw = 0;
    private int cooked = 1;
    private float SecondsToCook;
    private GameObject foodToRender;
    
    [SerializeField] float timeCooked = 0;
    public float TimeCooked
    {
        get => timeCooked;
        set => timeCooked = value;
    }

    public bool InWater = false;
    private NewBuoyancy _buoyancyScript;
    private Rigidbody foodRB;

    
    [HideInInspector] public bool foodCooked = false;

    private void Awake()
    {
        if (GetComponent<MeshRenderer>())
            GetComponent<MeshRenderer>().enabled = false;
        
        if(transform.childCount > 0)
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
        
        
        
        if (!GetComponent<Rigidbody>())
           gameObject.AddComponent<Rigidbody>();

        foodRB = GetComponent<Rigidbody>();
        
    }

    void Start()
    {
        
        _foodManager = GameObject.Find("GameManager").GetComponent<CornItemManager>().foodManager;
        _buoyancyScript = GameObject.FindObjectOfType<NewBuoyancy>();
        
        InitFood();
    }

    void InitFood()
    {
        
       
        
       myFoodProfile = _foodManager.GetPropertyFromName(FoodName);

        if ( myFoodProfile.Name == FoodName)
        {
           SecondsToCook =  myFoodProfile.SecondsToCook;
           
           if (SecondsToCook <= 0)
           {
               SecondsToCook = 5;
               print(myFoodProfile.Name + " cook time not assigned, using default cook time");
           }
            
            foodToRender =  myFoodProfile.RawPrefab;
            
            GameObject foodmesh = Instantiate(foodToRender, transform,false);
            foodmesh.transform.localPosition = Vector3.zero;
           
        }
        else
        {
            print("no profile found");
        }

        

       

    }

    private void Update()
    {
       // if (transform.parent.name.Contains("Clone") || gameObject.name.Contains("Clone"))
         if(TimeCooked >= SecondsToCook)
        {
            if (!foodCooked)
                FoodReady();
        }
        else
        {
            if (foodState == raw && InWater && _buoyancyScript.PotIsBoiling && TimeCooked < SecondsToCook)
                TimeCooked += Time.deltaTime;

            if (!foodCooked && TimeCooked >= SecondsToCook)
            {
                FoodReady();
            }
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
                    if(foodState == raw)
                        ChangePhysicsProperties(3, 3, false);
                    else if(foodState == cooked)
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

        var foodRot = foodToRender.transform.localRotation;
        var foodPos = foodToRender.transform.localPosition;
        var foodScale = foodToRender.transform.localScale;
        
        foodToRender = myFoodProfile.CookedPrefab;

        if (foodToRender == null) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
            
        var cookedFood = Instantiate(foodToRender,transform,false);
        cookedFood.transform.localPosition = foodPos;
        cookedFood.transform.localRotation = foodRot;
        //cookedFood.transform.localScale = foodScale;
        

        CornItemManager.ListOfFood.Add(cookedFood);
            
        var foodInchildren = cookedFood.GetComponentsInChildren<FoodItemProperties>();
        if (foodInchildren.Length <= 0) return;
        foreach (var f in foodInchildren)
        {
            CornItemManager.ListOfFood.Add(f.gameObject);
        }
    }
}
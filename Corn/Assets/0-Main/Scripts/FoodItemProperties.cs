using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FoodItemProperties : ItemProperties
{
    public string FoodName;
    public int foodState = 0;

    private int raw = 0;

    private int cooked = 1;
    [SerializeField] float percentCooked = 0;
    private float SecondsToCook;
   private FoodProfileManager _foodManager;

    public float PercentCooked
    {
        get => percentCooked;
        set => percentCooked = value;
    }

    public bool InWater = false;


    private Rigidbody myRB;

    private List<GameObject> rawFood = new List<GameObject>();
    private GameObject foodAssetToLoad;
    [HideInInspector] public bool foodCooked = false;

    private void Awake()
    {
        if (!GetComponent<Rigidbody>())
            gameObject.AddComponent<Rigidbody>();

        myRB = GetComponent<Rigidbody>();
    }

    void Start()
    {

        _foodManager = GameObject.Find("GameManager").GetComponent<CornItemManager>().foodManager;
        
        //if (!gameObject.name.Contains("Clone") && !transform.parent.name.Contains("Clone"))
            if(foodState == 0)
            InitFood();
    }

    void InitFood()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            rawFood.Add(transform.GetChild(i).gameObject);
        }

        var foodProfile = _foodManager.GetPropertyFromName(FoodName);

        if (foodProfile != null)
        {
            
            SecondsToCook = foodProfile.SecondsToCook;
            foodAssetToLoad = foodProfile.CookedPrefab;
            print("foodProfile found");
        }
        else
        {
            string path = "Prefabs/Food/" + gameObject.name + "-C";
            foodAssetToLoad = Resources.Load<GameObject>(path);
            SecondsToCook = 10;
        }
        
        
        
    }

    private void Update()
    {
        if (transform.parent.name.Contains("Clone") || gameObject.name.Contains("Clone"))
        {
            if (!foodCooked)
                FoodReady();
        }
        else
        {
            if (foodState == raw && InWater && Buoyancy.PotIsBoiling && PercentCooked < SecondsToCook)
                PercentCooked += Time.deltaTime;

            if (!foodCooked && PercentCooked >= SecondsToCook)
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
                myRB.isKinematic = true;
            }
            else
            {
                if (InWater)
                {
                    if (foodState == cooked)
                    {
                        ChangePhysicsProperties(10, 10, false);
                    }
                    else
                    {
                        ChangePhysicsProperties(5, 5, true);
                    }
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
        myRB.drag = drag;
        myRB.angularDrag = angularDrag;
        myRB.useGravity = useGravity;
    }


    void FoodReady()
    {
        foodCooked = true;
        foodState = 1;

        if (!Buoyancy.cookedFoodInWater.Contains(myRB))
            Buoyancy.cookedFoodInWater.Add(myRB);

        if (foodAssetToLoad != null)
        {
            var cookedFood = Instantiate(foodAssetToLoad);
            cookedFood.transform.position = gameObject.transform.position;
            cookedFood.transform.parent = gameObject.transform.parent;
            cookedFood.transform.localScale = gameObject.transform.localScale;

            gameObject.SetActive(false);
            CornItemManager.ListOfFood.Add(cookedFood);
            
            var foodInchildren = cookedFood.GetComponentsInChildren<FoodItemProperties>();
            if (foodInchildren.Length <= 0) return;
            foreach (var f in foodInchildren)
            {
                CornItemManager.ListOfFood.Add(f.gameObject);
            }


        }
    }
}
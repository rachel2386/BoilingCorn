using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FoodItemProperties : ItemProperties
{
    public int foodState = 0;

    private int raw = 0;

    private int cooked = 1;
    [SerializeField] float percentCooked = 0;
    [SerializeField] float SecondsToCook = 10;

    public float PercentCooked
    {
        get => percentCooked;
        set => percentCooked = value;
    }

    public bool InWater = false;


    private Rigidbody myRB;
    private ItemProperties itemScript;

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
        itemScript = GetComponent<ItemProperties>();
        if (!gameObject.name.Contains("Clone") && !transform.parent.name.Contains("Clone"))
            InitFood();
    }

    void InitFood()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            rawFood.Add(transform.GetChild(i).gameObject);
        }

        string path = "Assets/0-Main/Resources/Prefabs/Food/" + gameObject.name + "-C.prefab";
        foodAssetToLoad = AssetDatabase.LoadAssetAtPath<GameObject>(path);
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
        if (itemScript.HeldByPlayer)
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
using System.Collections;
using System.Collections.Generic;
using NVIDIA.Flex;
using UnityEditor;
using UnityEngine;

public class FoodItemProperties : ItemProperties
{
    public int foodState = 0;

    private int raw = 0;

    private int cooked = 1;

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
        if (!gameObject.name.Contains("Clone") && !transform.parent.name.Contains("Clone") ) 
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
        if(transform.parent != null)
            if(transform.parent.name.Contains("Clone") ||  gameObject.name.Contains("Clone"))  return;
       
        if (foodCooked) return;
        if (foodState == cooked)
        {
            ChangeFoodState();
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

    void ChangePhysicsProperties(int drag, int angularDrag, bool useGravity)
    {
        myRB.drag = drag;
        myRB.angularDrag = angularDrag;
        myRB.useGravity = useGravity;
    }

   

    void ChangeFoodState()
    {
        foodCooked = true;
        
        if (foodAssetToLoad != null)
        {
            var cookedFood = Instantiate(foodAssetToLoad);
            cookedFood.transform.position = gameObject.transform.position;
            cookedFood.transform.localScale = gameObject.transform.localScale;
            
           gameObject.SetActive(false);
//            if(rawFood.Count > 0)
//                foreach (var child in rawFood)
//                {
//                    child.SetActive(false);
//                }
            
        }

       
    }
}
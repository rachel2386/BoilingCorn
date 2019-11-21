using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider myCol;
    [HideInInspector] public float surfaceLevel;
    public bool PotIsBoiling = false;
    private List<Rigidbody> cookedFoodInWater = new List<Rigidbody>();
    private List<Rigidbody> rawFoodInWater = new List<Rigidbody>();
    List<Rigidbody> standbyList = new List<Rigidbody>();


    void Start()
    {
        myCol = GetComponent<Collider>();
        surfaceLevel = myCol.bounds.max.y; //(transform.position.y);
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (var rb in cookedFoodInWater)
        {
            if(rb.GetComponent<ItemProperties>().HeldByPlayer) return;
            Vector3 surfaceAngle = rb.transform.eulerAngles;
            rb.transform.right = Vector3.Slerp(rb.transform.right, transform.up, Time.deltaTime);
            // surfaceAngle.z = Mathf.LerpAngle(surfaceAngle.y, transform.up.y, Time.deltaTime);
        }
 if(Input.GetKeyDown("i"))
 {
     print("rawFood" + rawFoodInWater.Count + "cookedFood" + cookedFoodInWater.Count);
     for (int i = 0; i < cookedFoodInWater.Count; i++)
     {
         print(cookedFoodInWater[i].name);
     }
     
 }

    }

    void FixedUpdate()
    {
        foreach (var rb in rawFoodInWater)
        {
            
 
        }

        foreach (var rb in cookedFoodInWater)
        {
            if(rb.GetComponent<ItemProperties>().HeldByPlayer) return;
           
            var col = rb.GetComponent<Collider>();
            if (col.bounds.center.y < surfaceLevel)
            {
               rb.AddForce(-Physics.gravity);
            }
            else
            {
              rb.AddForce(Physics.gravity * 0.1f);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        print("enter");
     
        if (!other.CompareTag("FoodItem")) return;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        var foodProp = other.GetComponent<FoodItemProperties>();
        foodProp.InWater = true;
        if (foodProp.foodState == 0)
        {
            rawFoodInWater.Add(rb);
            StartCoroutine(CookingTime(rb, 10));
        }
        else
        {
            cookedFoodInWater.Add(rb);
        }
            
            
        
    }
    private void OnTriggerExit(Collider other)
    {
        print("exit");
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rawFoodInWater.Contains(rb))
            rawFoodInWater.Remove(rb);
        else if (cookedFoodInWater.Contains(rb))
            cookedFoodInWater.Remove(rb);

        rb.GetComponent<FoodItemProperties>().InWater = false;
        
        

    }

    private IEnumerator CookingTime(Rigidbody foodToCook, int secToCook)
    {
        while (!PotIsBoiling)
        {
            yield return null;
        }

        while (rawFoodInWater.Contains(foodToCook))
        {
            if (!rawFoodInWater.Contains(foodToCook))
            {
                print("food not cooking");
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(secToCook);
                cookedFoodInWater.Add(foodToCook);
                rawFoodInWater.Remove(foodToCook);
                foodToCook.GetComponent<FoodItemProperties>().foodState = 1;
            }
        }
    }
}
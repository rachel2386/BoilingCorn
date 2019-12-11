using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider myCol;
    [HideInInspector] public float surfaceLevel;
    public static bool PotIsBoiling = false;
    public static List<Rigidbody> cookedFoodInWater = new List<Rigidbody>();
    //private List<Rigidbody> rawFoodInWater = new List<Rigidbody>();
   


    void Start()
    {
        myCol = GetComponent<Collider>();
        surfaceLevel = myCol.bounds.max.y-transform.localScale.y/4; //(transform.position.y);
    }

    

    void FixedUpdate()
    {
        

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
       
     
        if (!other.CompareTag("FoodItem")) return;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        var foodProp = other.GetComponent<FoodItemProperties>();
        foodProp.InWater = true;
        if (foodProp.foodState == 1)
        {
            if(!cookedFoodInWater.Contains(rb))
            cookedFoodInWater.Add(rb);

        }
       
            
            
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("FoodItem")) return;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        
        if (cookedFoodInWater.Contains(rb))
            cookedFoodInWater.Remove(rb);

        rb.GetComponent<FoodItemProperties>().InWater = false;
    
    }

}
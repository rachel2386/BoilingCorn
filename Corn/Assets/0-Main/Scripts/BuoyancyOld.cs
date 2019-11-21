using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyOld : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider myCol;
    public float surfaceLevel;
    private List<Rigidbody> cookedFoodInWater = new List<Rigidbody>();
    private List<Rigidbody> rawFoodInWater = new List<Rigidbody>();
    

    void Start()
    {
        myCol = GetComponent<Collider>();
        surfaceLevel = (transform.position.y);
    }

    // Update is called once per frame
    private void Update()
    {
        
            foreach (var rb in cookedFoodInWater)
            {
                Vector3 surfaceAngle = rb.transform.eulerAngles;
                rb.transform.right = Vector3.Slerp(rb.transform.right,transform.up,Time.deltaTime);   
                // surfaceAngle.z = Mathf.LerpAngle(surfaceAngle.y, transform.up.y, Time.deltaTime);
                
            }
           
        
    }

    void FixedUpdate()
    {
      
        foreach (var rb in rawFoodInWater)
        {
                
            if(!rb.useGravity)
                rb.useGravity = true;

            rb.drag = 3;

        }    
        
        foreach (var rb in cookedFoodInWater)
            {
                
                if(rb.useGravity)
                    rb.useGravity = false;
                
                var col = rb.GetComponent<Collider>();
                if (col.bounds.center.y < surfaceLevel)
                {
                    rb.drag = 10;
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
        Rigidbody rb = other.GetComponent<Rigidbody>();
        //
//        if (other.GetComponent<FoodCookState>().foodState == 0)
//        {
//            rawFoodInWater.Add(rb);
//            StartCoroutine(CookingTime(rb, 10));
//        }
//        else
//        {
          cookedFoodInWater.Add(rb);  
//        }

       
        //cookedFoodInWater.Add(rb);
        //rb.useGravity = false;

        //rb.useGravity = false;

    }

   

    private void OnTriggerExit(Collider other)
    {
        
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rawFoodInWater.Contains(rb))
        rawFoodInWater.Remove(rb);

        if (cookedFoodInWater.Contains(rb))
            cookedFoodInWater.Remove(rb);
        rb.drag = 1;
        rb.angularDrag = 1;
        rb.useGravity = true;
        
    }

   private IEnumerator CookingTime(Rigidbody foodToCook, int secToCook)
   {
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
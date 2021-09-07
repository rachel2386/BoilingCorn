using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornBuoyancy : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider myCol;
    [HideInInspector] public float surfaceLevel;
    public  bool PotIsBoiling = false;
    public float waterBoilTimeInseconds = 60;
    public static List<Rigidbody> cookedFoodInWater = new List<Rigidbody>();
    List<Rigidbody>PotBaseStuff = new List<Rigidbody>();

    private AudioManager _audioManager;
    //private List<Rigidbody> rawFoodInWater = new List<Rigidbody>();

    

    void Start()
    {
        myCol = GetComponent<Collider>();
        _audioManager = FindObjectOfType<AudioManager>();
        surfaceLevel = myCol.bounds.max.y-transform.localScale.y/4; //(transform.position.y);
        
        foreach (var potbase in GameObject.FindGameObjectsWithTag("PotBase"))
        {
            if (!potbase.GetComponent<Rigidbody>()) potbase.AddComponent<Rigidbody>();
            
            var rb = potbase.GetComponent<Rigidbody>();
            rb.useGravity = false; //to prevent huajiao from dropping huajiao has no collider
            rb.drag = 1;
            rb.angularDrag = 2;
            PotBaseStuff.Add(rb);
            rb.AddForce(Physics.gravity);
            
        }

       
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

        foreach (var rb in PotBaseStuff)
        {
            var col = rb.GetComponent<Collider>();
            if ( PotIsBoiling)
            {
                if (col.bounds.max.y < surfaceLevel)
                {
                    rb.drag = 5;
                    rb.AddForce(-Physics.gravity * 0.5f + rb.transform.forward * Random.Range(-0.1f,0.1f));
                }
                else
                {
                    rb.drag = 1;
                    rb.AddForce(Physics.gravity * 0.1f);
                
                }
            }
           

            
            
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        
        if (GameManager.gameState != 1) return;
       
        
        Rigidbody rb;
        if (other.CompareTag("FoodItem"))
        {
           rb =  other.GetComponent<Rigidbody>();
            var foodProp = other.GetComponent<NewFoodItemProperties>();
            foodProp.InWater = true;
            
            if(foodProp.foodState == 0)
                {
                    var randomNumber = Random.Range(0, 2);
                    if(randomNumber == 0)
                        _audioManager.PlayAudioClipWithSource(_audioManager.FindClipWithName("dropFoodWater"), GetComponent<AudioSource>(),0.3f);
                    else
                        _audioManager.PlayAudioClipWithSource(_audioManager.FindClipWithName("dropFoodWater2"), GetComponent<AudioSource>(),0.3f);    
                }
        
            else if (foodProp.foodState == 1)
            {
                if(!cookedFoodInWater.Contains(rb))
                    cookedFoodInWater.Add(rb);
                
                var randomNumber = Random.Range(0, 4);
                if(randomNumber == 0)
                    _audioManager.PlayAudioClipWithSource(_audioManager.FindClipWithName("dropFoodWater"), GetComponent<AudioSource>(),0.3f);
                else if(randomNumber == 1)
                    _audioManager.PlayAudioClipWithSource(_audioManager.FindClipWithName("dropFoodWater2"), GetComponent<AudioSource>(),0.3f);    

            }
        }
        else if(other.CompareTag("PotBase"))
        {
            rb =  other.GetComponent<Rigidbody>();
            if(!PotIsBoiling)
            rb.AddForce(Physics.gravity);
        }

        
       
            
            
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FoodItem"))
        {
            
            Rigidbody rb = other.GetComponent<Rigidbody>();
        
            if (cookedFoodInWater.Contains(rb))
                cookedFoodInWater.Remove(rb);

            rb.GetComponent<NewFoodItemProperties>().InWater = false;
        }
        
        
//        else if(other.CompareTag("PotBase"))
//        {
//            var rb = other.GetComponent<Rigidbody>();
//            if (PotBaseStuff.Contains(rb))
//                PotBaseStuff.Remove(rb);
//                //rb.useGravity = true;
//                //rb.drag = 1;
//        }

       
    
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider myCol;
    public float surfaceLevel;
    private List<Rigidbody> rbInWater = new List<Rigidbody>();
    

    void Start()
    {
        myCol = GetComponent<Collider>();
        surfaceLevel = (transform.position.y);
    }

    // Update is called once per frame
    private void Update()
    {
        
            foreach (var rb in rbInWater)
            {
                Vector3 surfaceAngle = rb.transform.eulerAngles;
                rb.transform.right = Vector3.Slerp(rb.transform.right,transform.up,Time.deltaTime);   
                // surfaceAngle.z = Mathf.LerpAngle(surfaceAngle.y, transform.up.y, Time.deltaTime);
                
            }
           
        
    }

    void FixedUpdate()
    {
      
            foreach (var rb in rbInWater)
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
        rbInWater.Add(rb);
        rb.useGravity = false;
        
        //rb.useGravity = false;
       
    }

   

    private void OnTriggerExit(Collider other)
    {
        
        Rigidbody rb = other.GetComponent<Rigidbody>();
        rbInWater.Remove(rb);
        rb.drag = 1;
        rb.angularDrag = 1;
        rb.useGravity = true;
        
    }
}
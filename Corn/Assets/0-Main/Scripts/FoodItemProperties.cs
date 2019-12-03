using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItemProperties : ItemProperties
{
    public int foodState = 0;

    private int raw = 0;

    private int cooked = 1;

    public bool InWater = false;
    private Rigidbody myRB;
    private ItemProperties itemScript;

    private void Awake()
    {
        if (!GetComponent<Rigidbody>())
            gameObject.AddComponent<Rigidbody>();

        myRB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        itemScript = GetComponent<ItemProperties>();
    }

    private void FixedUpdate()
    {
        if (itemScript.HeldByPlayer)
        {
            myRB.drag = 4;
            myRB.angularDrag = 4;
            myRB.useGravity = false;
        }
        else
        {
            if (InWater)
            {
                if (foodState == cooked)
                {
                    myRB.drag = 10;
                    myRB.angularDrag = 10;
                    myRB.useGravity = false;
                }
                else
                {
                    myRB.drag = 5;
                    myRB.angularDrag = 5;
                    myRB.useGravity = true;
                }
                
            }
            else
            {
                myRB.drag = 1;
                myRB.angularDrag = 1;
                myRB.useGravity = true;
            }
            
        }
        
    }
}
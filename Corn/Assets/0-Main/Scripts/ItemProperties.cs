using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemProperties : MonoBehaviour
{
    [HideInInspector] public bool HeldByPlayer = false;
    

//  public abstract bool OnPickup();
    public virtual void Start()
    {
        
        print("running base start");
        gameObject.layer = LayerMask.NameToLayer("Pickupable");
        //InitOutline();
    }

    public virtual void OnPickUp(Joint objectHolder)
    {
        HeldByPlayer = true;
        transform.SetParent(objectHolder.transform);
            var myRB = GetComponent<Rigidbody>();
            myRB.DOMove(transform.parent.position,0.2f);
        //GetComponent<Rigidbody>().isKinematic = true;
        objectHolder.connectedBody = myRB;



    }

    public virtual void OnDropOff()//Vector3 DropOffPosition)
    {
        HeldByPlayer = false;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.SetParent(null);
        //GetComponent<Rigidbody>().position = DropOffPosition;
        


    }






}
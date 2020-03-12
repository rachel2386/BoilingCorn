using System.Collections;
using System.Collections.Generic;
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

    public virtual void OnPickUp(SpringJoint objectHolder)
    {
        HeldByPlayer = true;
       objectHolder.connectedBody = GetComponent<Rigidbody>();
      
    }

    public virtual void OnDropOff()//Vector3 DropOffPosition)
    {
        HeldByPlayer = false;
        //GetComponent<Rigidbody>().position = DropOffPosition;
        


    }

    protected QuickOutline InitOutline()
    {
       
        print("haha");
            if (!gameObject.GetComponent<QuickOutline>())
                gameObject.AddComponent<QuickOutline>();
            
            var outline = gameObject.GetComponent<QuickOutline>();
            outline.OutlineMode = QuickOutline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 10f;
            outline.enabled = false;
            
            return outline;
    }

    protected List<QuickOutline> InitOutlineWithProfile()
    {
        print("xixi");
        List<QuickOutline> childToOutline = new List<QuickOutline>();

        foreach (var meshR in transform.GetComponentsInChildren<MeshRenderer>())
        {
            if (!meshR.gameObject.GetComponent<QuickOutline>())
            meshR.gameObject.AddComponent<QuickOutline>();
            
            var outline = meshR.gameObject.GetComponent<QuickOutline>();
            outline.OutlineMode = QuickOutline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 10f;
            outline.enabled = false;
            
            childToOutline.Add(outline);
        }

        return childToOutline;
    }

//    private void OnMouseEnter()
//    {
//        
//        gameObject.GetComponent<QuickOutline>().enabled = true;
//    }
//
//    private void OnMouseExit()
//    {
//        gameObject.GetComponent<QuickOutline>().enabled = false;
//    }
}
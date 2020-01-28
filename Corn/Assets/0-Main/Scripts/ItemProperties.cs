using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    [HideInInspector] public bool HeldByPlayer = false;

//  public abstract bool OnPickup();
    public virtual void Start()
    {
        print("running base start");
        InitOutline();
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
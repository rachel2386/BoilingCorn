using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupObjectWPhysics : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera myCam;
    private GameObject objectHoding;
    private Rigidbody objectRB;
    public Transform objectHolder;
    public Transform WaterObjectHolder;
    public Transform PlateObjectHolder;
    public Transform FridgeObjectHolder;
    private bool IsholdingObject = false;
    public Transform ZReference;
    void Start()
    {
        myCam = Camera.main;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!IsholdingObject)
        {
            if (Input.GetMouseButtonUp(0))
            {
                
                RaycastHit hitInfo = new RaycastHit();

                {
                    if (Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.collider != null)
                        {
                           
                            if (hitInfo.collider.CompareTag("FoodItem"))
                            {
                                IsholdingObject = true;
                                GrabFood(hitInfo);
                            }
                        }
                    }
                }
            }
        }

        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                
                RaycastHit hitInfo = new RaycastHit();

                {
                    if (Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.collider != null)
                        {
                           
                            if (hitInfo.collider.CompareTag("Water"))
                            {
                                PlaceObject(WaterObjectHolder);
                            }
                            else if (hitInfo.collider.CompareTag("Plate"))
                            {
                                PlaceObject(PlateObjectHolder);
                            }
                            else if (hitInfo.collider.CompareTag("Fridge"))
                            {
                                PlaceObject(FridgeObjectHolder);
                            }
                        }
                    }
                }
            }


        }
    }

    void GrabFood(RaycastHit objectClicked)
    {
        
        objectHoding = objectClicked.collider.gameObject;
        objectRB = objectHoding.GetComponent<Rigidbody>();
        objectRB.useGravity = false;
        objectRB.isKinematic = true;
        objectRB.transform.SetParent(objectHolder);
        objectHoding.transform.localEulerAngles = Vector3.zero;
        Tween rbMove = objectRB.transform.DOLocalMove(Vector3.zero, 0.5f,false);
        rbMove.SetEase(Ease.OutExpo);
        
    }
    
    void PlaceObject(Transform holder)
    {
        IsholdingObject = false;
        objectHoding.transform.parent = holder.transform;
        objectRB.useGravity = true;
        objectRB.isKinematic = false;
        Tween rbMove = objectRB.transform.DOLocalMove(Vector3.zero, 0.5f,false);
        rbMove.SetEase(Ease.OutExpo);
        objectHoding = null;
        objectRB = null;
        
    }

    
    
    
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera myCam;
    private GameObject objectHoding;
    private Rigidbody objectRB;
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
            if (Input.GetMouseButton(0))
            {
                
                RaycastHit hitInfo = new RaycastHit();

                {
                    if (Physics.Raycast(myCam.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.collider != null)
                        {
                           
                            if (hitInfo.collider.CompareTag("Pickupable"))
                            {
                                IsholdingObject = true;
                                objectHoding = hitInfo.collider.gameObject;
                                objectRB = objectHoding.GetComponent<Rigidbody>();
                            }
                        }
                    }
                }
            }
        }

        if (IsholdingObject)
        {
            if (objectHoding != null)
            {
                objectRB.useGravity = false;
                objectRB.isKinematic = true;
                Vector3 mousePos = Input.mousePosition;
                //var ObjectPosOnScreen = myCam.WorldToScreenPoint(objectHoding.transform.position);
//                ObjectPosOnScreen.z = mousePos.y;

                mousePos.z = Mathf.Abs(myCam.transform.position.z - ZReference.position.z);
                objectHoding.transform.position = myCam.ScreenToWorldPoint(mousePos);
                
            }

            if (Input.GetMouseButtonUp(0))
            {
                objectRB.useGravity = true;
                objectRB.isKinematic = false;
                IsholdingObject = false;
                objectHoding = null;
                objectRB = null;
            }
        }
    }
}
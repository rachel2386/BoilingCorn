using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class Leaning : MonoBehaviour
{
    private Transform child;
    private Transform camPivot;
    
    private Vector3 eulerOffset;

    private int leaning = 0;
    // Start is called before the first frame update
    void Start()
    {
       // childCam = GetComponentInChildren<CinemachineVirtualCamera>().transform;
       child = transform.GetChild(0);
       camPivot = transform.parent;
        eulerOffset =  camPivot.localEulerAngles-transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
         var euler = camPivot.transform.localEulerAngles;
           euler.y = transform.localEulerAngles.y;
           camPivot.transform.localEulerAngles = euler + eulerOffset;
       
        

        if (leaning == 1)
        {
            //camPivot.localEulerAngles = child.localEulerAngles - eulerOffset;
            if (Input.GetButtonUp("Up"))
            {
                
                //camPivot.localEulerAngles = transform.localEulerAngles - eulerOffset;
                leaning = 0;
            }
        }
        else
        {
            if (Input.GetButtonDown("Up"))
            {
                
                leaning = 1;
            }
        }


    }
}

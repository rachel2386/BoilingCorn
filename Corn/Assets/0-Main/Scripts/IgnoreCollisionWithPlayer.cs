using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionWithPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().detectCollisions = false;
    }

   
}

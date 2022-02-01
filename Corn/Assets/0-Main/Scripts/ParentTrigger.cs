using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemProperties>() && !other.GetComponent<ItemProperties>().HeldByPlayer)
            other.transform.parent = transform;
    }
    
    private void OnTriggerStay(Collider other)
    {
    if (other.transform.parent != transform && other.GetComponent<ItemProperties>() && !other.GetComponent<ItemProperties>().HeldByPlayer)
        other.transform.parent = transform;
    }
}

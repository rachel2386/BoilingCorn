using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeOnTriggerEnter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("FoodItem")&&  !CornItemManager.FoodToSave.Contains(other.gameObject))
           CornItemManager.FoodToSave.Add(other.gameObject);
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FoodItem") &&  CornItemManager.FoodToSave.Contains(other.gameObject))
            CornItemManager.FoodToSave.Remove(other.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeOnTriggerEnter : MonoBehaviour
{
    private CornItemManager _itemManager;
    // Start is called before the first frame update
    void Start()
    {
        _itemManager = FindObjectOfType<CornItemManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("FoodItem")&&  !_itemManager.FoodToSave.Contains(other.gameObject))
           _itemManager.FoodToSave.Add(other.gameObject);
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FoodItem") &&  _itemManager.FoodToSave.Contains(other.gameObject))
            _itemManager.FoodToSave.Remove(other.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornItemManager : MonoBehaviour
{
    // Start is called before the first frame update
   List<GameObject> ListOfFood = new List<GameObject>();
   List<GameObject> ListOfItems = new List<GameObject>();
    void Awake()
    {
        foreach (var child in GameObject.Find("Interactables").GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Pickupable"))
            {
                ListOfItems.Add(child.gameObject);
                if(!child.GetComponent<ItemProperties>())
                    child.gameObject.AddComponent<ItemProperties>();
            }
        }
       
        ListOfFood.AddRange(GameObject.FindGameObjectsWithTag("FoodItem"));
        foreach (var food in ListOfFood)
        {
          
            if(!food.GetComponent<FoodItemProperties>())
            food.AddComponent<FoodItemProperties>();
        }
    }

   
}

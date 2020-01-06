using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornItemManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static List<GameObject> ListOfFood = new List<GameObject>();
    public FoodPropertyManager foodManager;
    List<GameObject> ListOfItems = new List<GameObject>();

    public static List<GameObject> Containers = new List<GameObject>();
    public static List<GameObject> FoodToSave = new List<GameObject>();
    public static List<GameObject> WastedFood = new List<GameObject>();
    public static List<GameObject> FoodEaten = new List<GameObject>();
    [HideInInspector] public List<GameObject> FridgeHolders = new List<GameObject>(); 

    void Awake()
    {
//        foreach (var child in GameObject.Find("Interactables").GetComponentsInChildren<Transform>())
//        {
//            if (child.gameObject.layer == LayerMask.NameToLayer("Pickupable"))
//            {
//                ListOfItems.Add(child.gameObject);
//                if (!child.GetComponent<ItemProperties>())
//                    child.gameObject.AddComponent<ItemProperties>();
//            }
//        }

        ListOfFood.AddRange(GameObject.FindGameObjectsWithTag("FoodItem"));
        foreach (var food in ListOfFood)
        {
            if (!food.GetComponent<FoodItemProperties>())
                food.AddComponent<FoodItemProperties>();
        }

        Containers.AddRange(GameObject.FindGameObjectsWithTag("Container"));

        foreach (var c in Containers)
        {
            var containerParent = c.transform.parent;
            containerParent.tag = "Untagged";
            if(!c.GetComponent<ContainerTriggerEnter>())
            c.AddComponent<ContainerTriggerEnter>();
            
        }

       FridgeHolders.AddRange(GameObject.FindGameObjectsWithTag("Respawn"));
        foreach (var holder in FridgeHolders)
        {
            holder.AddComponent<FridgeHolderBehavior>();
        }
        
        var FridgeTrigger = GameObject.FindGameObjectsWithTag("Fridge");
        foreach (var holder in FridgeTrigger)
        {
            holder.AddComponent<FridgeOnTriggerEnter>();
        }
    }
}
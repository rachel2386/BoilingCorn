using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornItemManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static List<GameObject> ListOfFood = new List<GameObject>();
    public FoodProfileManager foodManager;
    List<GameObject> ListOfItems = new List<GameObject>();

    public static List<GameObject> Containers = new List<GameObject>();
    public static List<GameObject> FoodToSave = new List<GameObject>();
    public static List<GameObject> WastedFood = new List<GameObject>();
    public static List<GameObject> FoodEaten = new List<GameObject>();
    [HideInInspector] public List<GameObject> FridgeHolders = new List<GameObject>(); 

    public void InitLists()
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
            if(!holder.GetComponent<FridgeOnTriggerEnter>())
            holder.AddComponent<FridgeOnTriggerEnter>();
        }

        
        //load food memories into queue
        foreach (var foodProfile in foodManager.FoodProperties)
        {
            if (foodProfile.foodMemories.Count > 0)
            {
                //add list items into queue (because queues cannot be public)
                foreach (var sprite in foodProfile.foodMemories)
                {
                    foodProfile.foodMemoryQueue.Enqueue(sprite);
                }
            }
            else
            {
                print(foodProfile.Name + " does not have food memory");
            }
        }
    }
}
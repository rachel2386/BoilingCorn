using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerTriggerEnter : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform foodParent;
    private void Start()
    {
        foodParent = GameObject.Find("Food").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.gameState != 1) return;
        if(other.CompareTag("FoodItem"))
        other.transform.parent = transform;
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(GameManager.gameState != 1) return;
        if(other.CompareTag("FoodItem"))
        other.transform.parent = foodParent;
    }
}

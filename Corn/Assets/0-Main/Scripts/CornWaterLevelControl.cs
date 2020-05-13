using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornWaterLevelControl : MonoBehaviour
{
    public Transform waterLevelScaler;
    private Vector3 myScale;
    private float subtractAmount;

    private CornItemInteractions _itemInteractions;
    // Start is called before the first frame update
    void Start()
    {
        myScale = waterLevelScaler.localScale;
        _itemInteractions = FindObjectOfType<CornItemInteractions>();
        subtractAmount = 0.2f / _itemInteractions.fullAmount; //half of 1/fullamount
       
    }

    // Update is called once per frame
    void Update()
    {
        if (CornItemManager.FoodEaten.Count < _itemInteractions.fullAmount)
        {
            myScale.y = Mathf.Lerp(myScale.y,1 - (CornItemManager.FoodEaten.Count * subtractAmount), Time.deltaTime);
            waterLevelScaler.localScale = myScale;
        }
        else
        {
            myScale.y = Mathf.Lerp(myScale.y,0, Time.deltaTime);
        }

       
    }
}

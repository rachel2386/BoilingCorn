using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornWaterLevelControl : MonoBehaviour
{
    public Transform waterLevelScaler;
    private Vector3 myScale;
    private float subtractAmount;

    private CornItemInteractions _itemInteractions;

    private CornItemManager _itemManager;
    // Start is called before the first frame update
    void Start()
    {
        myScale = waterLevelScaler.localScale;
        _itemInteractions = FindObjectOfType<CornItemInteractions>();
        _itemManager = FindObjectOfType<CornItemManager>();
        subtractAmount = 0.2f / _itemInteractions.fullAmount; //half of 1/fullamount
       
    }

    // Update is called once per frame
    void Update()
    {
        if (_itemManager.FoodEaten.Count < _itemInteractions.fullAmount)
        {
            myScale.y = Mathf.Lerp(myScale.y,1 - (_itemManager.FoodEaten.Count * subtractAmount), Time.deltaTime);
            waterLevelScaler.localScale = myScale;
        }
        else
        {
            myScale.y = Mathf.Lerp(myScale.y,0, Time.deltaTime);
        }

       
    }
}

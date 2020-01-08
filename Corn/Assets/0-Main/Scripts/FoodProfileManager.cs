using System.Collections;
using System.Collections.Generic;
using Unity.QuickSearch;
using UnityEngine;

[CreateAssetMenu(order = 1, fileName = "FoodProperty")]
public class FoodProfileManager :ScriptableObject
{
    public List<FoodProperty> FoodProperties;

    public FoodProperty GetPropertyFromName(string name)
    {
        return FoodProperties.Find(x => x.Name == name);
    }
}

[System.Serializable]
public class FoodProperty
{
    public string Name;
    public GameObject RawPrefab;
    public GameObject CookedPrefab;
    public float SecondsToCook;
   
    
}

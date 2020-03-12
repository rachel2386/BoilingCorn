using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject FoodPrefab;
    //public GameObject CookedPrefab;
    public float SecondsToCook;
    public AudioClip EatSound; 
    public List<Sprite>foodMemories = new List<Sprite>();
    public Queue<Sprite>foodMemoryQueue = new Queue<Sprite>();
   


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public string FoodName;
    public int FoodMultiplier;
    public FoodProfileManager _foodProfileManager;
   
    public void Initiate()
    {
        
        if(_foodProfileManager.GetPropertyFromName(FoodName) == null) return;
        if (_foodProfileManager.GetPropertyFromName(FoodName).FoodPrefab == null)
        {
            print("can not spawn food. No " + FoodName + " found in profile. Add profile or food prefab.");
           //yield return null;
        }
        else
        {
            var foodToInstatiate = _foodProfileManager.GetPropertyFromName(FoodName).FoodPrefab;

            for (int i = 0; i < FoodMultiplier * 10; i++)
            {
                InstantiateFood(foodToInstatiate);
               // yield return new WaitForSeconds(0.1f);
            }
        }


    }

    // Update is called once per frame

    void InstantiateFood(GameObject foodToInstatiate)
    {
        GameObject newFood = Instantiate(foodToInstatiate, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}

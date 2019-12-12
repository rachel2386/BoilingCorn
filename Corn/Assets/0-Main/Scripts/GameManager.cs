using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int gameState = 1;
    private int mainState = 1;
    private int wrapUpState = 2;
    private int endState = 3;
    private CornItemManager _cornItemManager;

   private PlayMakerFSM textAnimFSM;
    

    private void Awake()
    {
       
        if(!GetComponent<CornItemManager>())
            gameObject.AddComponent<CornItemManager>();
        
        _cornItemManager = GetComponent<CornItemManager>();
        textAnimFSM = GetComponent<PlayMakerFSM>();


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyUp(KeyCode.Return) || 
            CornItemManager.FoodEaten.Count >= CornItemManager.ListOfFood.Count 
            || _cornItemManager.FridgeHolders.Count <= 0)  // if player eats all food or no fridge slots yet or player trigger, end game
        {
            ChangeGameState();
            
        }
       
    }

    private void ChangeGameState()
    {
        if (gameState == mainState)
        {
            gameState = wrapUpState;
            WrapUpGameState();
        }
        else if (gameState == wrapUpState)
        {
            gameState = endState;
            EndGameState();
            
        }
    }

    void WrapUpGameState()
    {
        textAnimFSM.FsmVariables.BoolVariables[0].Value = true;
        var allFood = FindObjectsOfType<FoodItemProperties>();
        print("allFood" + allFood.Length);

        foreach (var f in allFood)
        {
            f.foodState = 3;
        }

        foreach (var c in CornItemManager.Containers)
        {
            if(c.transform.childCount == 0) continue;
            
            var cParent = c.transform.parent;
            cParent.tag = "Pickupable";
        }
    }

    private void EndGameState()
    {
        
        textAnimFSM.FsmVariables.BoolVariables[1].Value = true;
          
        foreach (var f in CornItemManager.ListOfFood)
        {
            if (CornItemManager.FoodToSave.Contains(f.gameObject) ||
                CornItemManager.FoodEaten.Contains(f.gameObject)) continue;

            CornItemManager.WastedFood.Add(f.gameObject);
            
        }

        textAnimFSM.FsmVariables.StringVariables[0].Value =
            "You ate " + CornItemManager.FoodEaten.Count + " pieces of food today.\n" +
            "You saved " + CornItemManager.FoodToSave.Count + " for tomorrow.\n" +
            "You dumped away " + CornItemManager.WastedFood.Count + ".\n" +
            "...\n" +
            "..\n" +
            "                                              \n" +
            "It's been a great day! ";
        
        
//        print("Wasted Food" + CornItemManager.WastedFood.Count);
//        print("Eaten Food" + CornItemManager.FoodEaten.Count);
//        print("Saved Food" + CornItemManager.FoodToSave.Count);
    }

}
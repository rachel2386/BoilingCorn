using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using Toggle = UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int gameState = 1;
    private int mainState = 1;
    private int wrapUpState = 2;
    private int endState = 3;
    private CornItemManager _cornItemManager;

    public PlayMakerFSM textAnimFSM;
    private FSM<GameManager> gameFSM;

    public bool WithOrderSystem = true;


    private void Awake()
    {
        if (!GetComponent<CornItemManager>())
            gameObject.AddComponent<CornItemManager>();

        _cornItemManager = GetComponent<CornItemManager>();
        textAnimFSM = GetComponent<PlayMakerFSM>();


        gameFSM = new FSM<GameManager>(this);

        if (WithOrderSystem)
            gameFSM.TransitionTo<OrderState>(); //default state
        else
            gameFSM.TransitionTo<CookingState>();
    }

    void Update()
    {
        gameFSM.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown("i"))
            print("gamestate = " + gameState + "  current state = " + gameFSM.CurrentState);
    }


    private abstract class GameState : FSM<GameManager>.State
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
    }

    private class OrderState : GameState
    {
        private GameObject OrderMenu;
        private List<Toggle.Toggle> Toggles = new List<Toggle.Toggle>();
        private List<FoodSpawner> _foodSpawners = new List<FoodSpawner>();
        private Button confirmButton;
        private GameObject warningText;
        
       int NumberOfFoodSelected = 0;

        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 0;
            FindObjectOfType<CornMouseLook>().lockCursor = false;
            Cursor.lockState = CursorLockMode.None;


            OrderMenu = GameObject.Find("OrderMenu");
           //Toggles.AddRange(FindObjectsOfType<Toggle.Toggle>());
            _foodSpawners.AddRange(FindObjectsOfType<FoodSpawner>());
            warningText = GameObject.Find("WarningText");
            warningText.SetActive(false);
            confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
            confirmButton.onClick.AddListener(EnterCookingState);
        }

        public override void OnExit()
        {
            base.OnExit();

            OrderMenu.SetActive(false);
            for (int i = 0; i < Toggles.Count; i++)
            {
                _foodSpawners[i].FoodName = Toggles[i].transform.parent.name;
                _foodSpawners[i].StartCoroutine(_foodSpawners[i].Initiate());
            }


           
        }

        void EnterCookingState()
        {
            
            Toggles.Clear();
            foreach (var toggle in FindObjectsOfType<Toggle.Toggle>())
            {
                if (toggle.isOn)
                    Toggles.Add(toggle);
            }

            if (Toggles.Count != 6)
                warningText.SetActive(true);
            else
                TransitionTo<CookingState>();
        }
    }

    private class CookingState : GameState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 1;
            FindObjectOfType<CornMouseLook>().lockCursor = true;

           // Context.StartCoroutine(RemoveWalls());
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.Return))
//            || CornItemManager.FoodEaten.Count >= CornItemManager.ListOfFood.Count 
//            || _cornItemManager.FridgeHolders.Count <= 0)  // if player eats all food or no fridge slots yet or player trigger, end game
            {
                Context.gameFSM.TransitionTo<CleanUpState>();
            }
        }

//        IEnumerator RemoveWalls()
//        {
//            yield return new WaitForSeconds(2);
//            foreach (var wall in GameObject.FindGameObjectsWithTag("Walls"))
//            {
//                wall.gameObject.SetActive(false);
//            }
//
//            yield return null;
//        }
    }

    private class CleanUpState : GameState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 2;
            InitCleanUpState();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Context.gameFSM.TransitionTo<EndState>();
            }
        }

        void InitCleanUpState()
        {
            Context.textAnimFSM.FsmVariables.BoolVariables[0].Value = true;
            var allFood = FindObjectsOfType<NewFoodItemProperties>();
            print("allFood" + allFood.Length);

            foreach (var f in allFood)
            {
                f.foodState = 3;
            }

            foreach (var c in CornItemManager.Containers)
            {
                if (c.transform.childCount == 0) continue;

                var cParent = c.transform.parent;
                cParent.tag = "Pickupable";
            }
        }
    }

    private class EndState : GameState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 3;
            InitEndGameState();
        }

        private void InitEndGameState()
        {
            Context.textAnimFSM.FsmVariables.BoolVariables[1].Value = true;

            foreach (var f in CornItemManager.ListOfFood)
            {
                if (CornItemManager.FoodToSave.Contains(f.gameObject) ||
                    CornItemManager.FoodEaten.Contains(f.gameObject)) continue;

                CornItemManager.WastedFood.Add(f.gameObject);
            }

            Context.textAnimFSM.FsmVariables.StringVariables[0].Value =
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
}
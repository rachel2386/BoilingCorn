using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private NewCornFoodInteractions _FoodInteractionScript;
    private CornMonologueManager _monologueManager;

    public PlayMakerFSM textAnimFSM;
    private FSM<GameManager> gameFSM;
    private GameObject OrderMenu;
    public GameObject SceneToLoad;
    

    private Image fadeImage;

    //public bool WithOrderSystem = true;
    public int Debug_StartWithState = -1;


    private void Awake()
    {
        gameFSM = new FSM<GameManager>(this);

        if (!GetComponent<CornItemManager>())
            gameObject.AddComponent<CornItemManager>();

        _cornItemManager = GetComponent<CornItemManager>();
        _FoodInteractionScript = FindObjectOfType<NewCornFoodInteractions>();
        _monologueManager = GetComponent<CornMonologueManager>();
        OrderMenu = GameObject.Find("OrderMenu");
        OrderMenu.SetActive(false);

        textAnimFSM = GetComponent<PlayMakerFSM>();
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        fadeImage.gameObject.SetActive(false);


        if (Debug_StartWithState == 0)
        {
            gameFSM.TransitionTo<OrderState>();
        }
        else if (Debug_StartWithState == 1)
        {
            SceneToLoad.SetActive(true);
            foreach (var child in FindObjectsOfType<FoodSpawner>())
            {
                child.StartCoroutine(child.Initiate());
            }

            gameFSM.TransitionTo<CookingState>();
        }
        else
        {
            SceneToLoad.SetActive(false);
            gameFSM.TransitionTo<BeforeOrderState>(); //default state
        }
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

    private class BeforeOrderState : GameState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            gameState = -1;
            //FindObjectOfType<CornMouseLook>().lockCursor = true;
            Cursor.lockState = CursorLockMode.Locked;
            Context.StartCoroutine(playMonologue());
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private IEnumerator playMonologue()
        {
            yield return new WaitForSeconds(3);
            Context._monologueManager.StartMonologue("phone call john");
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2);
            Context._monologueManager.StartMonologue("phone call christine");
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }

            yield return new WaitForSeconds(3);
            Context._monologueManager.StartMonologue("what to eat");
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2);
            TransitionTo<OrderState>();
        }
    }

    private class OrderState : GameState
    {
        private List<Toggle.Toggle> Toggles = new List<Toggle.Toggle>();
        private List<FoodSpawner> _foodSpawners = new List<FoodSpawner>();
        private Button confirmButton;
        private GameObject warningText;


        int NumberOfFoodSelected = 0;

        public override void OnEnter()
        {
            base.OnEnter();


            gameState = 0;


           Context.OrderMenu.SetActive(true);
            Context.SceneToLoad.SetActive(true); //load scene with food;
            
            FindObjectOfType<CornMouseLook>().lockCursor = false;
            Cursor.lockState = CursorLockMode.None;
            
            _foodSpawners.AddRange(FindObjectsOfType<FoodSpawner>());
            warningText = GameObject.Find("WarningText");
            warningText.SetActive(false);
            confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
            confirmButton.onClick.AddListener(EnterCookingState);
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
                Context.StartCoroutine(LoadCookingState());
        }

        IEnumerator LoadCookingState()
        {
            Context.fadeImage.gameObject.SetActive(true);
            Tween showImg = Context.fadeImage.DOFade(1, 1);
            while (Context.fadeImage.color.a < 0.98f)
            {
                yield return null;
             }
            TransitionTo<CookingState>();
            
            yield return new WaitForSeconds(5);
            Tween hideImg = Context.fadeImage.DOFade(0, 3);
            while (Context.fadeImage.color.a > 0.1f)
            {
                yield return null;
            }
            Context.fadeImage.gameObject.SetActive(false);    
            Context._monologueManager.StartMonologue("table setup");
            
            
        }

        public override void OnExit()
        {
            base.OnExit();

            Context.OrderMenu.SetActive(false);
            for (int i = 0; i < Toggles.Count; i++)
            {
                _foodSpawners[i].FoodName = Toggles[i].transform.parent.name;
                _foodSpawners[i].StartCoroutine(_foodSpawners[i].Initiate());
            }
        }
    }

    private class CookingState : GameState
    {
        private PlayMakerFSM knobFSM;
        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 1;
            FindObjectOfType<CornMouseLook>().lockCursor = true;
            InitManagers();
               
           
        }

        public override void Update()
        {
            base.Update();

            if (Context._FoodInteractionScript.playerIsFull && Context._monologueManager.MonologueIsComplete)
            {
                Context.StartCoroutine(LoadCleanUpState());
            }
        }

        void InitManagers()
        {
            Context._FoodInteractionScript.Initiate();
            Context._cornItemManager.InitLists();
            knobFSM = GameObject.Find("knob").GetComponent<PlayMakerFSM>();
        }

        

        IEnumerator LoadCleanUpState()
        {
            knobFSM.SetState("OffActions"); //turn off knob
            TransitionTo<CleanUpState>();
            Context.fadeImage.gameObject.SetActive(true);
            Tween showImg = Context.fadeImage.DOFade(1, 1);
            while (Context.fadeImage.color.a < 0.98f)
            {
                yield return null;
            }
            yield return new WaitForSeconds(3);
            Tween hideImg = Context.fadeImage.DOFade(0, 3);
            while (Context.fadeImage.color.a > 0.1f)
            {
                yield return null;
            }
            Context.fadeImage.gameObject.SetActive(false);    
            Context._monologueManager.StartMonologue("clean up");
            
            
        }
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

            if (Input.GetKeyDown(KeyCode.Return) || GameObject.FindGameObjectsWithTag("Respawn").Length == 0 ||
                CornItemManager.FoodEaten.Count > CornItemManager.ListOfFood.Count)
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
            
            Context._monologueManager.StartMonologue("end1");
            Context.StartCoroutine(MonologueControl());
        }

        public override void Update()
        {
            base.Update();
            if (Context.textAnimFSM.ActiveStateName == "end2 monologue" && Context._monologueManager.MonologueIsComplete)
            {
              
                Context.textAnimFSM.SetState("end");
                Context.textAnimFSM.FsmVariables.StringVariables[0].Value =
                    "You ate " + CornItemManager.FoodEaten.Count + " pieces of food today.\n" +
                    "You saved " + CornItemManager.FoodToSave.Count + " for tomorrow.\n" +
                    "You dumped away " + CornItemManager.WastedFood.Count + ".\n" +
            
               "...\n"+
                "It's been a great day! ";
                
                
            }
        }

        IEnumerator MonologueControl()
        {
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }
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
                "You dumped away " + CornItemManager.WastedFood.Count + ".\n";
            
           
//                +
//                "...\n" +
//                "..\n" +
//                "                                              \n" +
//                "It's been a great day! ";


//        print("Wasted Food" + CornItemManager.WastedFood.Count);
//        print("Eaten Food" + CornItemManager.FoodEaten.Count);
//        print("Saved Food" + CornItemManager.FoodToSave.Count);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using Toggle = UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int gameState = -1;
   private CornItemManager _cornItemManager;
    private CornItemInteractions _FoodInteractionScript;
   private CornMonologueManager _monologueManager;
    private AudioManager _audioManager;
    private CornMouseLook _mouseLook; 

    public PlayMakerFSM textAnimFSM;
    private FSM<GameManager> gameFSM;
    private GameObject OrderMenu;
    public GameObject SceneToLoad;

    private GameObject cleanupBowl;

    private AudioSource backgroundMusic;
    
    public GameObject lightToTurnOff;


    private bool startGame = false;

    public GameObject TitleMenu;
    
    //public bool WithOrderSystem = true;
    public int Debug_StartWithState = -1;
    public int waterBoilSeconds = 30;


    private void Awake()
    {

        
        gameFSM = new FSM<GameManager>(this);

        if (!GetComponent<CornItemManager>())
            gameObject.AddComponent<CornItemManager>();

        _cornItemManager = GetComponent<CornItemManager>();
        _FoodInteractionScript = FindObjectOfType<CornItemInteractions>();
        _monologueManager = GetComponent<CornMonologueManager>();
       _audioManager = FindObjectOfType<AudioManager>();
        _mouseLook = FindObjectOfType<CornMouseLook>();
       TitleMenu.SetActive(false);
        OrderMenu = GameObject.Find("OrderMenu");
        OrderMenu.SetActive(false);

        FindObjectOfType<CornBuoyancy>().waterBoilTimeInseconds = waterBoilSeconds;
        textAnimFSM = GetComponent<PlayMakerFSM>();
        cleanupBowl = GameObject.Find("BowlForTmr");
        backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
        
        


//        if (Debug_StartWithState == 1 || Debug_StartWithState == 2)
//        {
//            foreach (var child in FindObjectsOfType<FoodSpawner>())
//            {
//                child.StartCoroutine(child.Initiate());
//            }
//
//            gameFSM.TransitionTo<CookingState>();
//        }
//        else if(Debug_StartWithState == 0)
//        {
//            gameFSM.TransitionTo<OrderState>(); //default state
//        }
      
    }

    private void Start()
    {
        gameState = -1;
        gameFSM.TransitionTo<InitialState>();
        
    }

    void Update()
    {
        gameFSM.Update();

    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
        
    }

    public void StartGame(bool enabled)
    {
        startGame = enabled;
         gameFSM.TransitionTo<OrderState>();
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }


    private abstract class GameState : FSM<GameManager>.State
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
    }

    private class InitialState : GameState
    {
        private FinalBowlAnimation bowlAnim;
        private bool debugMode = false;
        public override void OnEnter()
        {
            base.OnEnter();
            Context.StartCoroutine(EnterDebug());
            Context._mouseLook.enableMouseLook = false;
            CornGameEvents.instance.EnterGameStateSwitch(-1);
           
        }
        IEnumerator EnterDebug()
        {
            yield return new WaitForSeconds(2);
            if(!debugMode)
            Context.gameFSM.TransitionTo<MenuState>();
        }
        
        public override void Update()
        {

            if(Input.GetKeyDown(KeyCode.F1))
                debugMode = true;
                
            
            if(!debugMode) return;
            
            if(Input.GetKeyUp(KeyCode.Alpha0))
            {
                Context.gameFSM.TransitionTo<MenuState>();
                
            }
            else if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                transitionToCooking();
                Context.backgroundMusic.Play();
                
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                transitionToCooking();
                Context.Debug_StartWithState = 2;
                //Context._FoodInteractionScript.playerIsFull = true;
            }
            else if(Input.GetKeyUp(KeyCode.Alpha3))
            {
               
                transitionToCooking();
                Context.Debug_StartWithState = 3;

            }
        }

        void transitionToCooking()
        {
            foreach (var child in FindObjectsOfType<FoodSpawner>())
            {
                child.StartCoroutine(child.Initiate());
                Context.gameFSM.TransitionTo<CookingState>();
                
            }
            
        }

        public override void OnExit()
        {
            base.OnExit();
            if(debugMode)
                CornGameEvents.instance.ExitGameStateSwitch(-1);
           
        }
    }

    private class MenuState : GameState
    {
       // private PlayableDirector timelineControl; 
        public override void OnEnter()
        {
            base.OnEnter();
            CornGameEvents.instance.ExitGameStateSwitch(-1);
            gameState = -1;
            Context.SceneToLoad.SetActive(false);
            Context.TitleMenu.SetActive(true);
            //timelineControl = FindObjectOfType<PlayableDirector>();
            Context._mouseLook.enableMouseLook = false;
            Context._mouseLook.SetCursorLock(false);
        }

       
    }

    private class OrderState : GameState
    {
        private PlayMakerFSM PhoneFsm;

        private bool doneCalling = false;
        private bool doneOrdering = false;
        private bool doneWithOpening = false;

        private List<Toggle.Toggle> Toggles = new List<Toggle.Toggle>();
        private List<FoodSpawner> _foodSpawners = new List<FoodSpawner>();
        private Button confirmButton;
        private GameObject warningText;



        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 0;
            
            
            Context.SceneToLoad.SetActive(false);
            Context._mouseLook.SetCursorLock(true);

            Context._mouseLook.SetRotation(Vector3.zero, Vector3.zero);
            Context._mouseLook.enableMouseLook = true;
            InitPhone();
        }

        void InitPhone()
        {
            PhoneFsm = GameObject.Find("Phone").GetComponent<PlayMakerFSM>();
            Context.StartCoroutine(playMonologue());
        }

        void InitMenu()
        {
            Context.OrderMenu.SetActive(true);
            AudioSource.PlayClipAtPoint(Context._audioManager.FindClipWithName("openMenu"), Camera.main.transform.position);
            Context.SceneToLoad.SetActive(true);

            Context._mouseLook.SetCursorLock(false);
            Context._mouseLook.enableMouseLook = false;
            

            _foodSpawners.AddRange(FindObjectsOfType<FoodSpawner>());
            warningText = GameObject.Find("WarningText");
            warningText.SetActive(false);
            confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
            confirmButton.onClick.AddListener(ConfirmOrder);
        }

        public override void Update()
        {
            base.Update();
            
           if (PhoneFsm.ActiveStateName == "DoneCalling")
            {
                doneCalling = true;
            }

           
        }

        void ConfirmOrder()
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
            {
                Context.SceneToLoad.SetActive(false);
                Context.OrderMenu.SetActive(false);
                Context._mouseLook.SetCursorLock(true);
                Context._mouseLook.enableMouseLook = true;
                doneOrdering = true;
            }
        }

        private IEnumerator playMonologue()
        {
            yield return new WaitForSeconds(3);
            Context._monologueManager.StartMonologue("ready for hotpot");
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }

            doneWithOpening = true;
            PhoneFsm.SendEvent("triggered"); // player is now able to click on menu

            while ( !PhoneFsm.FsmVariables.GetFsmBool("menuOpened").Value || !doneWithOpening)
            {
                yield return null;
            }
            
            InitMenu();
            

            while (!doneOrdering)
            {
                yield return null;
            }

            Context._monologueManager.StartMonologue("where are they");

            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }

            PhoneFsm.FsmVariables.BoolVariables[1].Value = true; //start phone call fsm

            while (!doneCalling)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1);
            Context._monologueManager.StartMonologue("what to eat");
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }


            Context.StartCoroutine(LoadNextState());
        }


        IEnumerator LoadNextState()
        {
           CornGameEvents.instance.EnterGameStateSwitch(1);


         yield return  new WaitForSeconds(3);
            
            Context._mouseLook.enableMouseLook = false;
            Context._mouseLook.SetRotation(Vector3.zero, Vector3.right * 40);
            
            Context.OrderMenu.SetActive(true);
            Context.SceneToLoad.SetActive(true);

            for (int i = 0; i < Toggles.Count; i++)
            {
                _foodSpawners[i].FoodName = Toggles[i].transform.parent.name;
                _foodSpawners[i].StartCoroutine(_foodSpawners[i].Initiate());
            }
    
            Context.OrderMenu.SetActive(false);
            yield return new WaitForSeconds(1);
            Context._audioManager.PlayAudioClipWithSource(Context._audioManager.FindClipWithName("cry"), null, 1);
            yield return new WaitForSeconds(4);
            Context._monologueManager.StartMonologue("I am so angry");
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }
            
            
            TransitionTo<CookingState>();
            
           
            yield return new WaitForSeconds(3);
            
            CornGameEvents.instance.ExitGameStateSwitch(1);
//           
            yield return new WaitForSeconds(2);
           
            Context._monologueManager.StartMonologue("music on");
            while (!Context._monologueManager.MonologueIsComplete)
            {
                yield return null;
            }
            Context.backgroundMusic.Play();
            
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }

    private class CookingState : GameState
    {
        private PlayMakerFSM knobFSM;

        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 1;

           Context._mouseLook.SetCursorLock(true);
           Context._mouseLook.enableMouseLook = true;
           Context._mouseLook.SetRotation(Vector3.zero, Vector3.right * 40);
            

            InitManagers();
            if (Context.Debug_StartWithState >= 2)
            {
                Context._FoodInteractionScript.playerIsFull = true;
                
                return;
            }
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
           
            Context._cornItemManager.InitLists();
            knobFSM = GameObject.Find("knob").GetComponent<PlayMakerFSM>();
        }


        IEnumerator LoadCleanUpState()
        {
            knobFSM.SetState("OffActions"); //turn off knob
            
            CornGameEvents.instance.EnterGameStateSwitch(2);
            
            TransitionTo<CleanUpState>();
            
            yield return new WaitForSeconds(3);
            
            CornGameEvents.instance.ExitGameStateSwitch(2);
            
            yield return new WaitForSeconds(1.5f);
            
            Context._FoodInteractionScript.audioEventTrigger.SendEvent("fly");
            Context._monologueManager.StartMonologue("clean up");

           
        }
    }

    private class CleanUpState : GameState
    {
        private Transform holder;
        private Rigidbody Lid;
        private FinalBowlAnimation bowlAnimPlayer;

        public override void OnEnter()
        {
            base.OnEnter();
            
            gameState = 2;
            InitCleanUpState();
           
          
        }

        void InitCleanUpState()
        {
            Context.textAnimFSM.FsmVariables.BoolVariables[0].Value = true;
            holder = FindObjectOfType<FridgeHolderBehavior>().transform;
            bowlAnimPlayer = FindObjectOfType<FinalBowlAnimation>();
            Lid = Context.cleanupBowl.transform.Find("BoxLid").GetComponent<Rigidbody>();
            Lid.gameObject.SetActive(false);
            Context.lightToTurnOff.SetActive(false);
    
            Context.backgroundMusic.DOFade(0,1);


            if (Context.Debug_StartWithState == 3)
            {
                FindObjectOfType<FridgeHolderBehavior>().hasChild = true;
                bowlAnimPlayer.EndOfAnimation();
                Context._monologueManager.DisplayNextSprite();
            }
        }

        public override void Update()
        {
            base.Update();

            if (GameObject.FindGameObjectsWithTag("Respawn").Length == 0) // transition to next stage after putting plate in fridge
            {
                if (!bowlAnimPlayer.IsPlaying)
                {
                    bowlAnimPlayer.StartAnimation();
                }

                if (bowlAnimPlayer.AnimationComplete)
                    Context.gameFSM.TransitionTo<EndState>();
            }

            if (Input.GetKeyDown(KeyCode.Return) && !movedBowl)
            {
                Context.StartCoroutine(MoveBowlToFridge());
                print("moving bowl");
               
            }
        }


        private bool movedBowl = false;
        IEnumerator MoveBowlToFridge()
        {
            movedBowl = true;
            Lid.gameObject.SetActive(true);
            Lid.isKinematic = false;
            Sequence moveBowlSequence = DOTween.Sequence();

            var liftedPos = Context.cleanupBowl.transform.position + Vector3.up * 0.4f + Vector3.forward * -0.3f;
            moveBowlSequence.Append(Context.cleanupBowl.transform.DOMove(liftedPos, 3));
            moveBowlSequence.SetEase(Ease.OutSine);

            moveBowlSequence.AppendInterval(1);

            moveBowlSequence.Append(Context.cleanupBowl.transform.DOMove(holder.position, 3));
            moveBowlSequence.SetEase(Ease.OutCirc);

            Context._monologueManager.StartMonologue("end1");

            yield return moveBowlSequence.WaitForCompletion();

            var rbConnected = Context._FoodInteractionScript.objectHolder.GetComponent<SpringJoint>().connectedBody;
            
            if (rbConnected != null)
                Context._FoodInteractionScript.PlaceObject();
            

           
           holder.GetComponent<FridgeHolderBehavior>().hasChild = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            var allFood = FindObjectsOfType<NewFoodItemProperties>();
            foreach (var f in allFood)
            {
                f.foodState = 3;
            }
        }
    }


    private class EndState : GameState
    {
        
        public override void OnEnter()
        {
            base.OnEnter();
            gameState = 3;
            CornGameEvents.instance.EnterGameStateSwitch(3);
            Context.StartCoroutine(MonologueControl());
        }


        IEnumerator MonologueControl()
        {
            
           yield return new WaitForSeconds(2);
            
           InitEndGameState();
           
           yield return new WaitForSeconds(1);
           
           Context.textAnimFSM.FsmVariables.BoolVariables[1].Value = true;

            while (Context.textAnimFSM.ActiveStateName != "end2 monologue")
            {
                yield return null;
            }
            
            CornGameEvents.instance.ExitGameStateSwitch(3);
            
            while (Context.textAnimFSM.ActiveStateName !="end")
            {
                
                yield return null;
            }
            yield return new WaitForSeconds(1);
            //Context._monologueManager.StartMonologue("great day");
        }

        private void InitEndGameState()
        {
           

            foreach (var f in Context._cornItemManager.ListOfFood)
            {
                if (Context._cornItemManager.FoodToSave.Contains(f.gameObject) ||
                    Context._cornItemManager.FoodEaten.Contains(f.gameObject)) continue;

                Context._cornItemManager.WastedFood.Add(f.gameObject);
            }

            Context.textAnimFSM.FsmVariables.StringVariables[0].Value =
                "You collected " + Context._cornItemManager.memoriesCollected + "/" + Context._cornItemManager.TotalMemoriesToCollect +  " pieces of memories. \n" + 
                "You ate " + Context._cornItemManager.FoodEaten.Count + " pieces of food.\n" +
                "You saved " + Context._cornItemManager.FoodToSave.Count + " for tomorrow.\n" +
                "You dumped away " + Context._cornItemManager.WastedFood.Count + ".\n";
            
            


//        print("Wasted Food" + CornItemManager.WastedFood.Count);
//        print("Eaten Food" + CornItemManager.FoodEaten.Count);
//        print("Saved Food" + CornItemManager.FoodToSave.Count);
        }

        public override void Update()
        {
            base.Update();
            if (Context.textAnimFSM.ActiveStateName == "end2 monologue" &&
                Context._monologueManager.MonologueIsComplete)
            {
               Context.textAnimFSM.SetState("end");
                
            }
        }
    }
}
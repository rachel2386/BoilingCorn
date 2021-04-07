using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class CornUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Image ImgSlot;
    public Image fadeImage;
    public Sprite foodCursor;
    public Sprite defaultCursor;
    public Sprite interactableCursor;
    public Sprite lookCursor;
   
    private CornMouseLook _mouseLookScript;
    private CornItemManager _itemManager;
    private CornItemInteractions _itemInteractions;

    public GameObject InteractInstruction;
    public GameObject EatButtonInstruction;
    public GameObject ZoomInstruction;
    public GameObject EndGameInstruction;
    private FinalBowlAnimation finalAnimation;

    public bool DontShowUI = false;

    private Camera MyCam;

    void Start()
    {
        MyCam = Camera.main;
        CornGameEvents.instance.OnGameStateSwitchEnter += ScreenFadeOut;
        CornGameEvents.instance.OnGameStateSwitchExit += ScreenFadeIn;
        
        _itemManager = FindObjectOfType<CornItemManager>();
        _itemInteractions = FindObjectOfType<CornItemInteractions>();
        _mouseLookScript = FindObjectOfType<CornMouseLook>();
        
        ImgSlot = GameObject.Find("Reticle").GetComponent<Image>();
        //ImgSlot.gameObject.SetActive(false);
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.gameObject.SetActive(true);


        ZoomInstruction.SetActive(false);
        InteractInstruction.SetActive(false);
        EndGameInstruction.SetActive(false);
        EatButtonInstruction.SetActive(false);

        finalAnimation = FindObjectOfType<FinalBowlAnimation>();
    }

    
    void Update()
    {

        if (DontShowUI  || !_mouseLookScript.enableMouseLook)
        {
           return;
        }

       

        // ImgSlot.gameObject.SetActive(true);

        RaycastHit hitInfo = new RaycastHit();


        if (!finalAnimation.IsPlaying)
        {
            if (GameManager.gameState > 0 && GameManager.gameState < 3)
            {
                ZoomInstruction.SetActive(true);
                InteractInstruction.SetActive(true);

                EndGameInstruction.SetActive(GameManager.gameState == 2);
                if (!Input.GetMouseButton(0) &&
                    Physics.Raycast(MyCam.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                        ~(1 << 1 | 1 << 2)) && hitInfo.collider != null)
                {
                    if (hitInfo.collider.CompareTag("FoodItem"))
                    {
                        ImgSlot.sprite = foodCursor;
                        EatButtonInstruction.SetActive(GameManager.gameState < 2
                                                       &&_itemManager.FoodEaten.Count < _itemInteractions.fullAmount //player not full
                                                       && hitInfo.collider.GetComponent<NewFoodItemProperties>()
                                                           .foodState == 1); //if food cooked, enabled eat ui
                    }
                    else if (hitInfo.collider.CompareTag("Interactable"))
                    {
                        ImgSlot.sprite = interactableCursor;
                    }
                    else if (hitInfo.collider.CompareTag("Look"))
                    {
                        ImgSlot.sprite = lookCursor;
                    }
                    else
                    {
                        ImgSlot.sprite = defaultCursor;
                        EatButtonInstruction.SetActive(false);
                    }
                }
                else
                {
                    EatButtonInstruction.SetActive(false);
                }
            }
            else if(GameManager.gameState == 0)
            {
              
                    ZoomInstruction.SetActive(true);
                    InteractInstruction.SetActive(true);

                    if (!Input.GetMouseButton(0) && Physics.Raycast(MyCam.ScreenPointToRay(Input.mousePosition),
                                                     out hitInfo, 1000, ~(1 << 1 | 1 << 2))
                                                 && hitInfo.collider.CompareTag("Interactable"))
                    {
                        ImgSlot.sprite = interactableCursor;
                    }
                    else
                    {
                        ImgSlot.sprite = defaultCursor;
                    }
                
            }
        }
        else
        {
            InteractInstruction.SetActive(false);
            EatButtonInstruction.SetActive(false);
            EndGameInstruction.SetActive(false);
            ZoomInstruction.SetActive(false);
            ImgSlot.enabled = false;
        }
    }

    private void ScreenFadeIn(int sceneIndex)
    {
        var duration = 1;

        if (sceneIndex == 2)
            duration = 2;
        
        fadeImage.DOFade(0f,duration);
        
    }
    private void ScreenFadeOut(int sceneIndex)
    {
        var duration = 1;

        if (sceneIndex == 2)
            duration = 1;
            
        fadeImage.DOFade(1f,duration);
    }

}
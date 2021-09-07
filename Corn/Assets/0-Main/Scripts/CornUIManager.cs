using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class CornUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CornUIManager instance;
    
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

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        MyCam = Camera.main;
        CornGameEvents.instance.OnGameStateSwitchEnter += ScreenFadeOut;
        CornGameEvents.instance.OnGameStateSwitchExit += ScreenFadeIn;
        
        _itemManager = FindObjectOfType<CornItemManager>();
        _itemInteractions = FindObjectOfType<CornItemInteractions>();
        _mouseLookScript = FindObjectOfType<CornMouseLook>();
        
        ImgSlot = GameObject.Find("Reticle").GetComponent<Image>();
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

        if (DontShowUI)
        {
           return;
        }

        RaycastHit hitInfo = new RaycastHit();


        if (!finalAnimation.IsPlaying)
        {
            if (GameManager.gameState > 0 && GameManager.gameState < 3)
            {
                ZoomInstruction.SetActive(false);
                InteractInstruction.SetActive(false);

                
                if (!Input.GetMouseButton(0) &&
                    Physics.Raycast(MyCam.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                        ~(1 << 1 | 1 << 2)) && hitInfo.collider != null)
                {
                    
                    EndGameInstruction.SetActive(hitInfo.collider.CompareTag("cleanupBowl") && GameManager.gameState == 2);
                    
                    if (hitInfo.collider.CompareTag("FoodItem"))
                    {
                        SwapReticleSprite(foodCursor);
                        EatButtonInstruction.SetActive(GameManager.gameState < 2
                                                       &&_itemManager.FoodEaten.Count < _itemInteractions.fullAmount //player not full
                                                       && hitInfo.collider.GetComponent<NewFoodItemProperties>()
                                                           .foodState == 1); //if food cooked, enabled eat ui
                    }
                    else if (hitInfo.collider.CompareTag("Interactable") || hitInfo.collider.CompareTag("cleanupBowl"))
                    {
                        SwapReticleSprite(interactableCursor);
                    }
                    else if (hitInfo.collider.CompareTag("Look"))
                    {
                        SwapReticleSprite(lookCursor);
                    }
                    else
                    {
                        SwapReticleSprite(defaultCursor);
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
                        SwapReticleSprite(interactableCursor);
                    }
                    else
                    {
                        SwapReticleSprite(defaultCursor);
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

    public void CursorSelectionMode()
    {
        DontShowUI = true;
        ImgSlot.gameObject.SetActive(false);
        _mouseLookScript.SetCursorLock(false);
        _mouseLookScript.enableMouseLook = false;
        
        
    }
    
    public void CursorLookMode()
    {
        DontShowUI = false;
        ImgSlot.gameObject.SetActive(true);
        _mouseLookScript.lockCursor = true;
        _mouseLookScript.enableMouseLook = true;
        
        
    }

    void SwapReticleSprite(Sprite reticleSprite)
    {
        
        if (ImgSlot.sprite == reticleSprite) return;
        StartCoroutine(SwapSprite(reticleSprite));
    }

    IEnumerator SwapSprite(Sprite reticleSprite)
    {
            Tween scaleDown = ImgSlot.rectTransform.DOScale(Vector3.zero, 0.1f);
            yield return new WaitForSeconds(0.05f);
            ImgSlot.sprite = reticleSprite;
            ImgSlot.rectTransform.DOScale(Vector3.one, 0.1f);
            CalculateSpritePivot(reticleSprite);
        
    }

    void CalculateSpritePivot(Sprite reticleSprite)
    {
        Vector2 size = ImgSlot.GetComponent<RectTransform>().sizeDelta;
        Vector2 pixelPivot =  reticleSprite.pivot;
        Vector2 percentPivot = Vector2.right * (pixelPivot.x / size.x) + Vector2.up *(pixelPivot.y / size.y);
        ImgSlot.GetComponent<RectTransform>().anchoredPosition = percentPivot;
    }



}
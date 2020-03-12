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

    public GameObject InteractInstruction;
    public GameObject EatButtonInstruction;
    public GameObject LeanInstruction;
    public GameObject EndGameInstruction;


    private Camera MyCam;

    void Start()
    {
        MyCam = Camera.main;
        _mouseLookScript = FindObjectOfType<CornMouseLook>();
        ImgSlot = GameObject.Find("Reticle").GetComponent<Image>();
        //ImgSlot.gameObject.SetActive(false);
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        fadeImage.color = new Color(0,0,0,0);
        fadeImage.gameObject.SetActive(false);
        
        EndGameInstruction.SetActive(false);
        EatButtonInstruction.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_mouseLookScript.lockCursor || !_mouseLookScript.enableMouseLook) return;

        // ImgSlot.gameObject.SetActive(true);

        RaycastHit hitInfo = new RaycastHit();
        if (!Physics.Raycast(MyCam.ScreenPointToRay(Input.mousePosition), out hitInfo) ||
            hitInfo.collider == null) return;

        if (!Input.GetMouseButton(0))
        {
            if (GameManager.gameState > 0 && GameManager.gameState < 3)
            {
                if (hitInfo.collider.CompareTag("FoodItem"))
                {
                    ImgSlot.sprite = foodCursor;
                    InteractInstruction.SetActive(true);
                    EatButtonInstruction.SetActive(hitInfo.collider.GetComponent<NewFoodItemProperties>()
                                                       .foodState == 1); //if food cooked, enabled eat ui
                }
                else if (hitInfo.collider.CompareTag("Interactable"))
                {
                    ImgSlot.sprite = interactableCursor;
                    InteractInstruction.SetActive(true);
                }
                else if (hitInfo.collider.CompareTag("Look"))
                {
                    ImgSlot.sprite = lookCursor;
                    InteractInstruction.SetActive(true);
                }
                else
                {
                    ImgSlot.sprite = defaultCursor;
                    InteractInstruction.SetActive(false);
                    EatButtonInstruction.SetActive(false);
                }
            }
//            else if (GameManager.gameState == 2)
//            {
//                //EndGameInstruction.SetActive(true);
//                if (hitInfo.collider.CompareTag("Pickupable"))
//                    ImgSlot.sprite = interactableCursor;
//                else
//                {
//                    ImgSlot.sprite = defaultCursor;
//                    EatButtonInstruction.SetActive(false);
//                }
//            }
            else
            {
                if (hitInfo.collider.CompareTag("Interactable"))
                {
                   print("interactable");
                    ImgSlot.sprite = interactableCursor;
                    InteractInstruction.SetActive(true);
                }
                else
                {
                    ImgSlot.sprite = defaultCursor;
                    InteractInstruction.SetActive(false);
                    
                }
            }
        }
        else
        {
            InteractInstruction.SetActive(false);
            EatButtonInstruction.SetActive(false);
        }
    }

    public void FadeIn(float duration, Color ScreenColor)
    {
        StartCoroutine(FadingIn(duration, ScreenColor));
    }
   
    public void FadeOut(float duration, Color ScreenColor)
    {
        StartCoroutine(FadingOut(duration, ScreenColor));
    }


    private IEnumerator FadingIn(float duration, Color ScreenColor)
    {
        
        fadeImage.color = ScreenColor;
        var color = fadeImage.color;
        color.a = 1;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(true);
        
        
        Tween fadeImg = fadeImage.DOFade(0, duration);
        yield return fadeImg.WaitForCompletion();
        fadeImage.gameObject.SetActive(false);
        yield return true;
        

    }

  private IEnumerator FadingOut(float duration, Color ScreenColor)
   {
      
       
       fadeImage.color = ScreenColor;
       var color = fadeImage.color;
       color.a = 0;
       fadeImage.color = color;
       fadeImage.gameObject.SetActive(true);
             
       Tween showImg = fadeImage.DOFade(1, duration);
       yield return showImg.WaitForCompletion();
       
      
       
   }
}
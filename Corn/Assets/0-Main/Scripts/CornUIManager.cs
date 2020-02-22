using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CornUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Image ImgSlot;
    public Sprite foodCursor;
    public Sprite defaultCursor;
    public Sprite interactableCursor;

    public GameObject InteractInstruction;
    public GameObject EatButtonInstruction;
    public GameObject LeanInstruction;
    public GameObject EndGameInstruction;


    private Camera MyCam;

    void Start()
    {
        MyCam = Camera.main;
        ImgSlot = GameObject.Find("Reticle").GetComponent<Image>();
//        defaultCursor = transform.Find("DefaultCursor").GetComponent<Image>().mainTexture;
//        foodCursor = transform.Find("FoodCursor").GetComponent<Image>();
//        interactableCursor = transform.Find("InteractableCursor").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo = new RaycastHit();


        if (!Physics.Raycast(MyCam.ScreenPointToRay(Input.mousePosition), out hitInfo) ||
            hitInfo.collider == null ) return;
        
        if (!Input.GetMouseButton(0))
        {
            if (GameManager.gameState == 1)
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
                else
                {
                    ImgSlot.sprite = defaultCursor;
                    InteractInstruction.SetActive(false);
                    EatButtonInstruction.SetActive(false);
                }
            }
            else if (GameManager.gameState == 2)
            {
                if (hitInfo.collider.CompareTag("Pickupable"))
                    ImgSlot.sprite = interactableCursor;
            }
        }
        else
        {
            InteractInstruction.SetActive(false);
            EatButtonInstruction.SetActive(false);
        }

        
    }
}
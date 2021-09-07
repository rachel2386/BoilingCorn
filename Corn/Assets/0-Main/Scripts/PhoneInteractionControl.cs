using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhoneInteractionControl : MonoBehaviour
{
    [SerializeField] private bool phoneOn = false;
    [SerializeField] private GameObject phoneModel;
    private CornMouseLook _mouseLook;
    void Start()
    {
        _mouseLook = GetComponent<CornMouseLook>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            phoneOn = !phoneOn;
        }
        
        phoneModel.SetActive(phoneOn);
        _mouseLook.lockCursor = !phoneOn;
        _mouseLook.enableMouseLook = !phoneOn;

    }
    
    

}

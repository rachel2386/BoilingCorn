using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CornPanelBehavior : MonoBehaviour, IPointerUpHandler,IPointerDownHandler
{
    private bool mouseDown = false;
    private float dragSpeed = 25f;
    private RectTransform m_canvas;
    private CanvasScaler _canvasScaler;
    private RectTransform m_rectTrans;
    private Button closeWindowButton;
    
    private Camera myCam;
    // Start is called before the first frame update
    void Start()
    {
        m_rectTrans = GetComponent<RectTransform>();
        myCam = Camera.main;
        m_canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();//GameObject.Find("bg").GetComponent<RectTransform>();//
        closeWindowButton = GetComponentInChildren<Button>();
        PositionCloseButton();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseDown)
        {
          
             OnPanelDragged();
        }
        
        if(Input.GetKey("e"))
            MaximizeView();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
       mouseDown = true;
        BringClickedPanelForward();
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPanelRelease();
        mouseDown = false;
        
        // followingMouse = false;
        // movingPanel = null;
        // print("pointerlclicked");
    }

    void BringClickedPanelForward()
    {
        m_rectTrans.SetAsLastSibling();
    }
    
    void OnPanelDragged()
    {
        Cursor.visible = false; 
        
        if (!CornPanelManager.DraggingPanel)
        {
            CornPanelManager.DraggingPanel = true;
            CornPanelManager.DraggedPanel = m_rectTrans;
        }

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectTrans, Input.mousePosition, myCam, out var panelPos);
           
           Vector2 panelPos = m_rectTrans.anchoredPosition;

           
           
           panelPos.x += Input.GetAxis("Mouse X") * dragSpeed; 
           panelPos.y += Input.GetAxis("Mouse Y") * dragSpeed; 
           
            
           panelPos.x = Mathf.Clamp(panelPos.x, 
               m_canvas.rect.xMin-m_rectTrans.rect.xMin,m_canvas.rect.xMax-m_rectTrans.rect.xMax);
           
           panelPos.y = Mathf.Clamp(panelPos.y, 
               m_canvas.rect.yMin-m_rectTrans.rect.yMin, m_canvas.rect.yMax-m_rectTrans.rect.yMax);
           
            m_rectTrans.anchoredPosition = panelPos;
           
      
    }

    void OnPanelRelease()
    {
        Cursor.visible = true; 
        CornPanelManager.DraggingPanel = false;
        CornPanelManager.DraggedPanel = null;
    }

    void ResizePanel()
    {
        Vector2 la = m_rectTrans.rect.max;
        la.x++;
       // m_rectTrans.SetSizeWithCurrentAnchors(); = 
    }

    void MaximizeView()
    {
        Vector2 screenDimensions;
        screenDimensions.x = m_canvas.rect.width;
        screenDimensions.y = m_canvas.rect.height;
        m_rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, myCam.scaledPixelWidth);
        m_rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, myCam.scaledPixelHeight);
        PositionCloseButton();

    }

    void PositionCloseButton()
    {
        RectTransform ButtonTransform = closeWindowButton.GetComponent<RectTransform>();
        Vector2 buttonPos;
        buttonPos.x = m_rectTrans.rect.xMax - (ButtonTransform.rect.xMax * 2);
        buttonPos.y = m_rectTrans.rect.yMax - (ButtonTransform.rect.yMax * 2);
        ButtonTransform.anchoredPosition = buttonPos;
    }
}

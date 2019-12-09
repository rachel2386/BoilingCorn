using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CornPanelBehavior : MonoBehaviour, IPointerUpHandler,IPointerDownHandler
{
    private Camera myCam;
    
    private bool mouseDown = false;
    private float dragSpeed = 25f;
    private RectTransform m_canvas;
    private RectTransform m_rectTrans;

    private Button ControlButton;
    private RectTransform closeWindowButton;
    private RectTransform maxWinButton;
    private List<RectTransform> panelButtons = new List<RectTransform>();

    private int timesEnabled = 0;
    
    //different view modes
    private bool panelFullWinEnabled;
    public bool PanelFullWinEnabled
    {
        get => panelFullWinEnabled;
        set => panelFullWinEnabled = value;
    }
    
    Vector2 fullWinAnchorMin = new Vector2(0,0);
    Vector2 fullWinAnchorMax = new Vector2(1,1);
    Vector2 fullWinPivotPos = new Vector2(1f,1f);
    private Vector2 InitWinAnchorMin;
    private Vector2 InitWinAnchorMax;
    private Vector2 InitPivotPos;
    private Vector2 InitRectSize;
    private Vector2 InitRectPos = Vector2.zero;
    
    
    // Start is called before the first frame update
    private void Awake()
    {
        m_rectTrans = GetComponent<RectTransform>();
        ControlButton = GameObject.Find(gameObject.name + "Button").GetComponent<Button>();
        ControlButton.onClick.AddListener(OpenWindow);
        ControlButton.onClick.AddListener(BringClickedPanelForward);
        print(ControlButton.name);
        //InitPanelInfo();
    }

    void Start()
    {
        
        myCam = Camera.main;
        m_canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        InitPanelButtons();
        

    }

    private void OnEnable()
    {
        BringClickedPanelForward();
        
        if (timesEnabled == 1)
        {
           print("first time");
            InitPanelInfo();
           
        }
        timesEnabled++;
        

       
    }

    void InitPanelInfo()
    {
       
        //m_rectTrans.position = Input.mousePosition;
        InitRectPos = m_rectTrans.anchoredPosition;
        InitWinAnchorMin = m_rectTrans.anchorMin;
        InitWinAnchorMax = m_rectTrans.anchorMax;
        InitPivotPos = Vector2.one;
        InitRectSize = m_rectTrans.sizeDelta;

    }

    void InitPanelButtons()
    {
        closeWindowButton =transform.Find("Close").GetComponent<RectTransform>();
        maxWinButton = transform.Find("Maximize").GetComponent<RectTransform>();
        var buttonsInChildren = GetComponentsInChildren<Button>();
        
        if(buttonsInChildren.Length < 0) return;
        foreach (var button in buttonsInChildren)
        {
            panelButtons.Add(button.GetComponent<RectTransform>());
        }
       
        PositionButtons();
        maxWinButton.gameObject.GetComponent<Button>().onClick.AddListener(ToggleMaximizeView);
        closeWindowButton.gameObject.GetComponent<Button>().onClick.AddListener(CloseWindow);
        
       
       
       
       
    }

    
    void Update()
    {
        if (mouseDown)
        {
          if(!PanelFullWinEnabled)
             OnPanelDragged();
        }
   
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

    

    private void ToggleMaximizeView()
    {
       
        if (!panelFullWinEnabled)
        {
            panelFullWinEnabled = true;
            InitRectPos = m_rectTrans.anchoredPosition;
            m_rectTrans.anchoredPosition = Vector2.zero;
            m_rectTrans.anchorMin = fullWinAnchorMin;
            m_rectTrans.anchorMax = fullWinAnchorMax;
            
            //m_rectTrans.pivot = fullWinPivotPos;
            m_rectTrans.sizeDelta = Vector2.one;
            
        }
        else
        {
            panelFullWinEnabled = false;
            m_rectTrans.anchorMin = InitWinAnchorMin;
            m_rectTrans.anchorMax = InitWinAnchorMax;
            m_rectTrans.pivot = InitPivotPos;
            m_rectTrans.sizeDelta = InitRectSize;
            m_rectTrans.anchoredPosition = InitRectPos;


        }


        PositionButtons();
        

    }

    void CloseWindow()
    {
      
        gameObject.SetActive(false);
       
    }
    void OpenWindow()
         {
           
             gameObject.SetActive(true);
            
         }

    void PositionButtons()
    {
        Vector2 buttonPos;
        int distanceFromEdge = 2;
        
        if(panelButtons.Count > 0)
        for (int i = 0; i < panelButtons.Count; i++)
        {
            buttonPos.x = m_rectTrans.rect.xMax - (panelButtons[i].rect.xMax * distanceFromEdge);
            buttonPos.y = m_rectTrans.rect.yMax - (panelButtons[i].rect.yMax * 2);
            panelButtons[i].anchoredPosition = buttonPos;
            distanceFromEdge += 2;
        }
        
       
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CornPanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera myCam;
    private bool followingMouse = false;
    public static RectTransform DraggedPanel;
    public static bool DraggingPanel = false;
    private List<RectTransform> myPanels = new List<RectTransform>();

    void Start()
    {
        
        myCam = Camera.main;
        var panels = GameObject.FindGameObjectsWithTag("View");

        foreach (var p in panels)
        {
            myPanels.Add(p.GetComponent<RectTransform>());
            p.AddComponent<CornPanelBehavior>();
            p.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    

//    void BringClickedPanelForward(Vector2 screenPoint)
//    {
//        print("mouseP");
//        foreach (var panel in myPanels)
//        {
//            if (RectTransformUtility.RectangleContainsScreenPoint(panel, screenPoint))
//            {
//                followingMouse = true;
//                movingPanel = panel;
//                movingPanel.SetAsLastSibling();
//                break;
//            }
//        }
//    }

    
}
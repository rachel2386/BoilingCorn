using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CornPanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera myCam;
    //private bool followingMouse = false;
    public static RectTransform DraggedPanel;
    public static bool DraggingPanel = false;
    public bool EnableMouseFollow = false;
    private List<RectTransform> myPanels = new List<RectTransform>();

    void Start()
    {
        
        myCam = Camera.main;
        var panels = GameObject.FindGameObjectsWithTag("View");

        foreach (var p in panels)
        {
            myPanels.Add(p.GetComponent<RectTransform>());
            
            var panelBehaviorScript = p.AddComponent<CornPanelBehavior>();
            panelBehaviorScript.PanelFullWinEnabled = !EnableMouseFollow;
            
            if(!p.gameObject.name.Contains("Home"))
            p.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
   

    
}
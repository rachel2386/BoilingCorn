using System.Collections;
using System.Collections.Generic;
using DynamicPanels;
using UnityEngine;

[RequireComponent(typeof(DynamicPanelsCanvas))]
public class CornPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    private DynamicPanelsCanvas panelCreatorScript;
    
    void Start()
    {
        panelCreatorScript = GetComponent<DynamicPanelsCanvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePanel()
    {
        //panelCreatorScript.CreateInitialPanel(panelCreatorScript, )
    }
}

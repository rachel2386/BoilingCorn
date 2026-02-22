using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornEndlessModePotFoodTrigger : MonoBehaviour
{
    private CornEndlessModeMusicController _musicController;
    private Collider _triggerCollider;
    private bool firstFoodAdded = false;
    private bool isPotBoiling = false;
    // Start is called before the first frame update
    void Start()
    {
        
        _musicController = FindObjectOfType<CornEndlessModeMusicController>();
        _triggerCollider = GetComponent<Collider>();
        _triggerCollider.enabled = false;

        CornGameEvents.instance.OnEndlessModeBegin += ActivateTrigger;
        CornGameEvents.instance.OnStoveOff += ResetVariables;
        CornGameEvents.instance.OnPotBoiling += OnPotBoiled;
       
    }

    void ActivateTrigger()
    {
        _triggerCollider.enabled = true;



    }
    void OnTriggerEnter(Collider other)
    {
        if (GameManager.gameState != 4) return;
        if (!isPotBoiling) return;
        if (other.CompareTag("FoodItem"))
        {
           // CornGameEvents.instance.TriggerMusicNote();
         
            CornGameEvents.instance.UpdateFoodInPotCount(1);
            if (!firstFoodAdded)
            {
                firstFoodAdded = true;
                CornGameEvents.instance.FirstFoodAdded();
            }
                
        
        }
    
    
    }

    void OnTriggerExit(Collider other)
    {
        if (GameManager.gameState != 4) return;
        if (other.CompareTag("FoodItem"))
        {
            CornGameEvents.instance.UpdateFoodInPotCount(-1);

        }

    }

    void ResetVariables()
    {
        firstFoodAdded = false;
        isPotBoiling = false;

    }

    void OnPotBoiled()
    {
        isPotBoiling = true;
    
    }
}

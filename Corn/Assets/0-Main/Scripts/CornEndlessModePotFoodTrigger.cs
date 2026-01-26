using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornEndlessModePotFoodTrigger : MonoBehaviour
{
    private CornEndlessModeMusicController _musicController;
    private Collider _triggerCollider;
    // Start is called before the first frame update
    void Start()
    {
        _musicController = FindObjectOfType<CornEndlessModeMusicController>();
        _triggerCollider = GetComponent<Collider>();
       
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (GameManager.gameState != 4) return;
        if (other.CompareTag("FoodItem"))
        {
            CornGameEvents.instance.TriggerMusicNote();
            CornGameEvents.instance.UpdateFoodInPotCount(1);
        
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
}

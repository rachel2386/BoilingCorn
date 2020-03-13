using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeHolderBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private MeshRenderer mr;
    public bool hasChild = false;
    private List<GameObject> holderList;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        holderList = FindObjectOfType<CornItemManager>().FridgeHolders;
    }

    private void Update()
    {
       if(GameManager.gameState != 2) return;
        if (hasChild)
        {
            //mr.enabled = false;
            
            if (gameObject.CompareTag("Respawn"))
            {
                gameObject.tag = "Untagged";
                holderList.Remove(gameObject);
            }
            
        }
        else
        {
           // mr.enabled = true;
        }

       

    }

    // Update is called once per frame
//    private void OnMouseEnter()
//    {
//       if(GameManager.gameState == 2 && !hasChild)
//        mr.enabled = true;
//    }
//
//    private void OnMouseExit()
//    {
//        mr.enabled = false;
//    }
}

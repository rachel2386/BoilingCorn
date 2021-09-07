using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhoneButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject pageToControl;
    [SerializeField] private float pageScaleTime = 0.3f;
    
    // Start is called before the first frame update
    void Start()
    {
        pageToControl.transform.localScale = Vector3.zero;
     

     


    }

    // Update is called once per frame
    public void OnPointerClick(PointerEventData pointerEventData)
    {
       pageToControl.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
        pageToControl.transform.DOScale(Vector3.one, pageScaleTime);
    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawSphere( pageToControl.GetComponent<RectTransform>().TransformVector(pageToControl.GetComponent<RectTransform>().pivot),0.01f);
//    }
}

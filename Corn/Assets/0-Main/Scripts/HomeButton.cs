using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HomeButton : MonoBehaviour, IPointerUpHandler
{
    [SerializeField]private Transform[] pages;
    [SerializeField] private float pageScaleTime = 0.2f;
    private void Start()
    {
        if(pages.Length == 0)
            Debug.LogError("no pages assigned to home button");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       print("pressed");
        foreach (var p in pages)
        {
            p.GetComponent<RectTransform>().pivot = Vector2.right * 0.5f;
            p.DOScale(Vector3.zero, pageScaleTime);
        }
        
    }
}

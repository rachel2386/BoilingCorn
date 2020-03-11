using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinalBowlAnimation : MonoBehaviour
{
    public List<Transform> bowlsToAnimate = new List<Transform>();
    private int currentBowlIndex = 0;
    public Transform PositionAboveTrashCan;
    public bool StartToPlay = false;
    public bool IsPlaying = false;
    public bool AnimationComplete = false;
    public float BowlMoveSpeed = 1;
    public float BowlRotateSpeed = 1;
    private AudioSource myAs;
    public AudioClip clipToPlay; 

    private void Start()
    {
        myAs = GetComponent<AudioSource>();
        myAs.clip = clipToPlay;
        myAs.loop = true;
        myAs.Stop();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsPlaying)
        {
            StartAnimation();
        }
    }

    public void StartAnimation()
    {
        if(!myAs.isPlaying)
            myAs.Play();
        
        if (!IsPlaying)
        {
            StartCoroutine(PlayAnimation(bowlsToAnimate[currentBowlIndex]));
        }
    }

    private void MoveToNextBowl()
    {
        currentBowlIndex++;
        if (currentBowlIndex < bowlsToAnimate.Count)
        {
            StartCoroutine(PlayAnimation(bowlsToAnimate[currentBowlIndex]));
        }
        else
        {
            print("bowl animation sequence complete!");
            AnimationComplete = true;
            myAs.DOFade(0, 1);
            myAs.loop = false;
        }
    }
    
   

    
    IEnumerator PlayAnimation(Transform bowl)
    {
        IsPlaying = true;
        var InitPos = bowl.position;
        

        yield return new WaitForSeconds(2);
        
        Tween liftBowl = bowl.DOMove(Vector3.up * 0.5f, BowlMoveSpeed);
        liftBowl.SetRelative(true);
        liftBowl.SetSpeedBased(true);
        liftBowl.SetEase(Ease.OutSine);
        yield return liftBowl.WaitForCompletion();
        
        Tween moveBowl = bowl.DOMove(PositionAboveTrashCan.position, BowlMoveSpeed);
        moveBowl.SetSpeedBased(true);
        moveBowl.SetEase(Ease.OutSine);
        yield return moveBowl.WaitForCompletion();
        
        Tween rotateBowl = bowl.DORotate(Vector3.up * 180, BowlRotateSpeed);
        rotateBowl.SetSpeedBased(true);
        yield return rotateBowl.WaitForCompletion();
        
        Tween pourFood = bowl.DORotate(Vector3.forward * 180, BowlRotateSpeed);
        pourFood.SetRelative(true);
        pourFood.SetSpeedBased(true);
        yield return pourFood.WaitForCompletion();
       
       
        
        Tween resetRotation = bowl.DORotate(Vector3.zero, 200);
       resetRotation.SetSpeedBased(true);
       
       
       Tween liftBowl2 = bowl.DOMove(Vector3.up * 0.6f, BowlMoveSpeed);
       liftBowl2.SetRelative(true);
       liftBowl2.SetSpeedBased(true);
       liftBowl2.SetEase(Ease.OutSine);
       yield return liftBowl2.WaitForCompletion();
       
      
       
       var resetPos = InitPos;
       Tween resetPosition = bowl.DOMove(resetPos, BowlMoveSpeed);
       resetPosition.SetSpeedBased(true);
       resetPosition.SetEase(Ease.OutSine);
       
      // yield return resetPosition.WaitForCompletion();
      MoveToNextBowl();
       

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinalBowlAnimation : MonoBehaviour
{
    public List<Transform> bowlsToAnimate = new List<Transform>();
    [SerializeField] private int numberOfBowlsBeforeEnd = 6;
    private int currentBowlIndex = 0;
    public Transform PositionAboveTrashCan;
    private bool firstTimePlaying = true;
    public bool IsPlaying = false;
    public bool AnimationComplete = false;
    public float BowlMoveSpeed = 1;
    public float BowlRotateSpeed = 1;
    private AudioSource myAs;
    public AudioClip clipToPlay;
    private Camera myCam;
    private Vector3 camPos;

  

    private void Start()
    {
        myCam = Camera.main;
        myAs = GetComponent<AudioSource>();
        myAs.clip = clipToPlay;
        myAs.loop = true;
        myAs.Stop();

    }


    public void StartAnimation()
    {
       
        if(!myAs.isPlaying) myAs.Play();
        if (!IsPlaying)
        {
            GameManager.gameState = 3;
            camPos = myCam.transform.position;
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
        
        if(currentBowlIndex >= numberOfBowlsBeforeEnd)
        {
           EndOfAnimation();
        }
    }

   public void EndOfAnimation()
    {
        print("bowl animation sequence complete!");
        AnimationComplete = true;
        myAs.DOFade(0, 3);
        myAs.loop = false;
    }

    IEnumerator moveCam()
    {

        var mouseLook = FindObjectOfType<CornMouseLook>();
            mouseLook.enableMouseLook = false;
            
       Sequence camSequence = DOTween.Sequence();
       camSequence.Append(mouseLook.transform.DOLocalRotate(Vector3.zero, 30));
       camSequence.Join(myCam.transform.DOLocalRotate(new Vector3(62f,0f,0f), 30));
       camSequence.Join(myCam.transform.DOLocalMove(new Vector3(0.3f,2f,-0.6f), 60));
      yield return null;
    }

    
    IEnumerator PlayAnimation(Transform bowl)
    {
        IsPlaying = true;
        var InitPos = bowl.position;
        

        if (firstTimePlaying)
        {
            
            yield return new WaitForSeconds(1);
            Tween moveTrashcan = PositionAboveTrashCan.parent.DOMove(Vector3.forward * -1, 3);
            moveTrashcan.SetRelative(true);
            yield return moveTrashcan.WaitForCompletion();
            
        }
        
            Tween liftBowl = bowl.DOMove(camPos + Vector3.forward * 0.5f + Vector3.up * -0.5f, BowlMoveSpeed);
            //liftBowl.SetRelative(true);
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
            
            if (firstTimePlaying)
            {
                StartCoroutine(moveCam());
                firstTimePlaying = false;
            }
        
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
            yield return new WaitForSeconds(2);
            MoveToNextBowl();
        

        
       

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

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
    [SerializeField] private PlayableDirector endCredits;
    [SerializeField] private GameObject endCreditsUI;
    [SerializeField] private AudioSource ambientSound;
    
  

    private void Start()
    {
        myCam = Camera.main;
        endCreditsUI.SetActive(false);
        myAs = GetComponent<AudioSource>();
        myAs.clip = clipToPlay;
        myAs.loop = true;
        myAs.Stop();

    }


    public void StartAnimation()
    {
       
       if(!myAs.isPlaying) myAs.Play();
       myAs.volume = 0.3f;
       myAs.DOFade(1, 1);
       ambientSound.DOFade(0, 3);
       endCreditsUI.SetActive(true);
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
        
//        if(currentBowlIndex >= numberOfBowlsBeforeEnd)
//        {
//           EndOfAnimation();
//        }
    }

   public void EndOfAnimation()
    {
       AnimationComplete = true;
        myAs.DOFade(0, 3);
        myAs.loop = false;
        ambientSound.DOFade(0.2f, 2);
    }

    IEnumerator moveCam()
    {

        var mouseLook = FindObjectOfType<CornMouseLook>();
            mouseLook.enableMouseLook = false;
            
       Sequence camSequence = DOTween.Sequence();
       camSequence.Append(mouseLook.transform.DOLocalRotate(Vector3.zero, 30));
       camSequence.Join(myCam.transform.DOLocalRotate(new Vector3(62f,0f,0f), 30));
       camSequence.Join(myCam.transform.DOLocalMove(new Vector3(0.3f,2f,-0.6f), 60));
       endCredits.Play();
      yield return null;
    }

    
    IEnumerator PlayAnimation(Transform bowl)
    {
        IsPlaying = true;
        var InitPos = bowl.position;
        var InitRot = bowl.eulerAngles;

        var foodInBowl = bowl.GetComponentsInChildren<NewFoodItemProperties>();

        if (foodInBowl.Length <= 0) // if no food in bowl, move to next bowl.
        {
            MoveToNextBowl();
            yield break;
        }

        foreach (var food in foodInBowl)
        {
            food.GetComponent<Rigidbody>().isKinematic = true;
        }

        if (firstTimePlaying)
        {
            
            yield return new WaitForSeconds(1);
            Tween moveTrashcan = PositionAboveTrashCan.parent.DOMove(Vector3.forward * -1, 3);
            moveTrashcan.SetRelative(true);
            yield return moveTrashcan.WaitForCompletion();
            
        }
        
        Tween liftup = bowl.DOMove(Vector3.up * 0.2f, BowlMoveSpeed * 1.25f);
        liftup.SetRelative(true);
        liftup.SetSpeedBased(true);
        liftup.SetEase(Ease.OutSine);
        yield return liftup.WaitForCompletion();
        
        Tween liftBowl = bowl.DOMove(camPos + Vector3.forward * 0.5f + Vector3.up * -0.5f, BowlMoveSpeed* 1.2f);
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
            
            foreach (var food in foodInBowl)
            {
                food.GetComponent<Rigidbody>().isKinematic = false;
            }
            
            yield return pourFood.WaitForCompletion();
            
       
        
            Tween resetRotation = bowl.DORotate(InitRot, 200);
            resetRotation.SetSpeedBased(true);
       
       
            Tween liftBowl2 = bowl.DOMove(Vector3.up * 0.6f, BowlMoveSpeed);
            liftBowl2.SetRelative(true);
            liftBowl2.SetSpeedBased(true);
            liftBowl2.SetEase(Ease.OutSine);
            yield return liftBowl2.WaitForCompletion();
       
      
            Tween resetPosition = bowl.DOMove(InitPos, BowlMoveSpeed*2);
            resetPosition.SetSpeedBased(true);
            resetPosition.SetEase(Ease.OutSine);
       
            // yield return resetPosition.WaitForCompletion();
            yield return new WaitForSeconds(2f);
            MoveToNextBowl();
        

        
       

    }
}

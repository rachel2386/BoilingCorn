using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint), typeof(Collider), typeof(AudioSource))]
public class OpenDoors : MonoBehaviour
{
    public float closeAngle = 0;
    public float openAngle = 90;
    public float SpringForce = 2;
    public float SpringDamper = 1;
    private bool doorIsOpen = false;

    private float currentRotation;
    private float InitRotation = 0;
    private HingeJoint myHJ;
    private AudioSource myAS;

    public AudioClip OpenDoorClip;
    public AudioClip CloseDoorClip;
    public float AngleTolerence = 4f;

    // Start is called before the first frame update
    void Awake()
    {
        this.gameObject.tag = "Interactable";
        
        myHJ = GetComponent<HingeJoint>();
        myAS = GetComponent<AudioSource>();
        myHJ.useSpring = true;
        var springPos = myHJ.spring;
        springPos.spring = SpringForce;
        springPos.damper = SpringDamper;
        springPos.targetPosition = closeAngle;
        myHJ.spring = springPos;
    }

    private void OnMouseDown()
    {
        var springPos = myHJ.spring;
        if (!doorIsOpen)
        {
            springPos.targetPosition = openAngle;
            myHJ.spring = springPos;
            doorIsOpen = true;
            InitRotation = transform.eulerAngles.magnitude;
            StartCoroutine(PlayOpenDoorClip());
        }
        else
        {
            springPos.targetPosition = closeAngle;
            myHJ.spring = springPos;
            doorIsOpen = false;
            InitRotation = transform.eulerAngles.magnitude;
            StartCoroutine(PlayCloseDoorClip());
        }
    }

    private void Update()
    {
        currentRotation = transform.eulerAngles.magnitude;
        
        if(Input.GetKeyDown(KeyCode.Y))
        print("currentRotation = " + currentRotation + "InitRot = " +  "distance = " + Mathf.Abs(currentRotation - InitRotation));
    }

    IEnumerator PlayCloseDoorClip()
    {
        if (CloseDoorClip == null)
            yield break;

        while (Mathf.Abs(currentRotation - InitRotation) < openAngle -AngleTolerence)
        {
            if (doorIsOpen)
                yield break;

            yield return null;
        }

        //if()
        myAS.PlayOneShot(CloseDoorClip);
    }

    IEnumerator PlayOpenDoorClip()
    {
        if (OpenDoorClip == null)
            yield break;

//        while (Mathf.Abs(currentRotation - InitRotation) < openAngle - AngleTolerence)
//        {
//            yield return null;
//            if (!doorIsOpen)
//                yield break;
//        }

        myAS.PlayOneShot(OpenDoorClip);
    }
}
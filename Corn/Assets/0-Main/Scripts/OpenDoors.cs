using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint), typeof(Collider), typeof(AudioSource))]
public class OpenDoors : MonoBehaviour
{
    public float closeAngle = 0;
    public float openAngle = 90;

    private bool doorIsOpen = false;

    private HingeJoint myHJ;
    private AudioSource myAS;

    public AudioClip OpenDoorClip;

    public AudioClip CloseDoorClip;

    // Start is called before the first frame update
    void Start()
    {
        myHJ = GetComponent<HingeJoint>();
        myAS = GetComponent<AudioSource>();
        myHJ.useSpring = true;
        var springPos = myHJ.spring;
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

            if (OpenDoorClip != null)
                myAS.PlayOneShot(OpenDoorClip);
        }
        else
        {
            springPos.targetPosition = closeAngle;
            myHJ.spring = springPos;
            doorIsOpen = false;

            if (CloseDoorClip != null)
                myAS.PlayOneShot(CloseDoorClip);
        }
    }
}
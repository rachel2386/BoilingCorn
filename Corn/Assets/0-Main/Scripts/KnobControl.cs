using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class KnobControl : MonoBehaviour
{
    static int numOfFlames = 0;
    public Collider knob2;

    public bool ActivateKnob = false;
    // Start is called before the first frame update
    void Start()
    {
        knob2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (numOfFlames < 4) return;
        knob2.enabled = true;
       ActivateKnob = true;



    }

    public void PlusOne()
    {
        if(numOfFlames<4)
        numOfFlames++;
        print(numOfFlames);
    }
    
    public void MinusOne()
    {
        if(numOfFlames>0)
        numOfFlames--;
        print(numOfFlames);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class DrawerControl : MonoBehaviour
{
    [SerializeField]private Vector3 openPosition;
    private ConfigurableJoint _cjoint;
    [SerializeField] private bool drawerOpen = false;
    [SerializeField] private bool inverseDirection = false;
    [SerializeField]private float openSpeed = 2f;
    [SerializeField]private float damper = 1f;
    
    void Awake()
    {
        gameObject.tag = "Interactable";
        _cjoint = GetComponent<ConfigurableJoint>();
        var cjointXDrive = _cjoint.xDrive;

        cjointXDrive.positionSpring = openSpeed;
        cjointXDrive.positionDamper = damper;
        _cjoint.xDrive = cjointXDrive;
        _cjoint.yDrive = cjointXDrive;
        _cjoint.zDrive = cjointXDrive;

    }

    
    

    private void OnMouseDown()
    {
        var direction = inverseDirection ? -1 : 1;
        openPosition *= direction;
        
        _cjoint.targetPosition = !drawerOpen ? openPosition : Vector3.zero;
        drawerOpen = !drawerOpen;
        print("mouse clicked");
    }

}

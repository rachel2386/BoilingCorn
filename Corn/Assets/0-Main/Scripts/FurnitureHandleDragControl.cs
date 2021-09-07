using UnityEngine;

namespace Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class FurnitureHandleDragControl : MonoBehaviour
    {
        enum HandleType {Horizontal, Vertical, Generic}

        [SerializeField]private HandleType thisHandle;

        [SerializeField] private bool invertX = false;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float movementMultiplier = 10f;
    

        private bool _mouseDown = false;
   
        private Rigidbody myRB;
        private Camera myCam;
   
        private Vector3 mouseInit;
        private Vector3 mouseOffset;
        private float mouseZ;
        
        private Vector3 objectPosOnClick;
        
    
        void Start()
        {
            myRB = GetComponent<Rigidbody>();
            myCam = Camera.main;
            gameObject.tag = "Interactable";

        }

        // Update is called once per frame

        void OnMouseDown()
        {
     
            mouseZ = myCam.ScreenToWorldPoint(myRB.position).z;
            objectPosOnClick = myRB.position;
            mouseInit = GetMouseWorldPos();
            mouseOffset =objectPosOnClick - mouseInit;
            _mouseDown = true;

        }

        private void OnMouseUp()
        {
            _mouseDown = false;
        }

        Vector3 GetMouseWorldPos()
        {
            var mouseWorldPoint = Input.mousePosition;
            mouseWorldPoint.z = mouseZ;
            if (thisHandle == HandleType.Vertical)
                mouseWorldPoint.x = 0;
            else if(thisHandle == HandleType.Horizontal)
                mouseWorldPoint.y = 0;
            
            if (invertX)
                mouseWorldPoint.x *= -1;
            if (invertY)
                mouseWorldPoint.y *= -1;
            
            return myCam.ScreenToWorldPoint(mouseWorldPoint);
        }

        float GetMouseMagnitude()
        {
            var mouseMagnitude = GetMouseWorldPos().y - mouseInit.y;
        
            if(thisHandle == HandleType.Vertical)
                mouseMagnitude = GetMouseWorldPos().y - mouseInit.y;
            else
                mouseMagnitude = GetMouseWorldPos().x - mouseInit.x;

            if (invertX)
                mouseMagnitude *= -1;
        
            return mouseMagnitude;

        }

        private void FixedUpdate()
        {
            if (_mouseDown)
            {
                var targetPos = mouseOffset + GetMouseWorldPos();
              //  myRB.velocity = ((targetPos-myRB.position) /Time.fixedDeltaTime)* movementMultiplier;
               
              var moveDir = Vector3.Normalize(targetPos - myRB.position);
              
              myRB.velocity = moveDir * Time.fixedDeltaTime * movementMultiplier;

//
//                var moveDir = Vector3.Normalize(transform.forward);//                
//                if (thisHandle == HandleType.Vertical)
//                    moveDir = Vector3.Normalize(transform.up);
              //var currentPos = objectPosOnClick + moveDir * GetMouseMagnitude() * movementMultiplier;
              //myRB.velocity = (currentPos - objectPosOnClick) / Time.fixedDeltaTime;


            }
        }
    }
}

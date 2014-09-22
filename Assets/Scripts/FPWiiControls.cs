using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class FPWiiControls : MonoBehaviour {

    #region interface
    [DllImport ("UniWii")]
    private static extern int wiimote_count();
    [DllImport ("UniWii")]
    private static extern bool wiimote_available( int which );
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccX(int which);
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccY(int which);
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccZ(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButtonA(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButtonB(int which);
    [DllImport ("UniWii")]
    private static extern double wiimote_getBatteryLevel( int which );
    #endregion
    
    #region variables
    public GameController gc;
    public float jumpSpeed = 8.0F;
    public float rotationSpeed = 100.0F;
    public float gravity = 10.0F;
    private Vector3 moveDirection = Vector3.zero;
    private int wiiControllerIndex;
    #endregion
    
    // Use this for initialization
    void Start () {
//        wiiControllerIndex = gc.GetFirstPersonIndex();
        wiiControllerIndex = 1;
    }
    
    // Update is called once per frame
    void Update () {
        CharacterController controller = GetComponent<CharacterController>();
        
        // Rotate player around y-axis
        if (wiimote_available (wiiControllerIndex)) {
            float accX = wiimote_getAccX (wiiControllerIndex) -125;
            
            transform.Rotate(0, accX * rotationSpeed * Time.deltaTime, 0);
            if (controller.isGrounded) {
                if (wiimote_getButtonA(wiiControllerIndex))
                    moveDirection.y = jumpSpeed;
            }
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}

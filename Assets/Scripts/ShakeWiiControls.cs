using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class ShakeWiiControls : MonoBehaviour {

    [DllImport ("UniWii")]
    private static extern void wiimote_start();
    [DllImport ("UniWii")]
    private static extern void wiimote_stop();
    [DllImport ("UniWii")]
    private static extern int wiimote_count();
    [DllImport ("UniWii")]
    private static extern bool wiimote_available( int which );
    [DllImport ("UniWii")]
    private static extern float wiimote_getRoll(int which);
    [DllImport ("UniWii")]
    private static extern float wiimote_getPitch(int which);
    [DllImport ("UniWii")]
    private static extern float wiimote_getYaw(int which);
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
    private static extern bool wiimote_getButtonUp(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButtonLeft(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButtonRight(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButtonDown(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButton1(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButton2(int which);
    [DllImport ("UniWii")]
    private static extern double wiimote_getBatteryLevel( int which );

    [DllImport ("UniWii")]
    private static extern bool wiimote_isExpansionPortEnabled( int which );
    
    [DllImport ("UniWii")]
    private static extern byte wiimote_getNunchuckStickX(int which);
    
    [DllImport ("UniWii")]
    private static extern byte wiimote_getNunchuckAccX(int which);
    [DllImport ("UniWii")]
    private static extern byte wiimote_getNunchuckAccZ(int which);
    
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButtonNunchuckC(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButtonNunchuckZ(int which);

    public bool DEBUGGING = false;

    public GameController gc;
    public int flickThreshold = 25;
    public float flickScale = 6.0F;
    public float jumpSpeed = 8.0F;
    public float friction = 5.0F;
    public float breakPower = 10.0F;
    public float rotationSpeed = 10.0F;
    public float maxSpeed = 20.0F;
    public float gravity = 10.0F;

    private int wiiControllerIndex;
    private Vector3 moveDirection = Vector3.zero;
    private byte centerOffset = 125;
    private PlaySoundEffect soundEffectManager;
    // Use this for initialization
    void Start () 
    {
        soundEffectManager = GetComponent<PlaySoundEffect>();
        wiiControllerIndex = 0;
    }
    void OnGUI()
    {
        if (DEBUGGING)
        {
            GUILayout.BeginVertical("box");
            int c = wiimote_count();
            if (c > 0)
            {
                
                for (int i=0; i < c; i++)
                {
                    GUILayout.Label("Wiimote " + i + " found!");
                }
                float yaw = wiimote_getYaw(wiiControllerIndex);
                float pitch = wiimote_getPitch(wiiControllerIndex);
                float roll = wiimote_getRoll(wiiControllerIndex);
                
                byte accX = wiimote_getAccX(wiiControllerIndex);
                byte accY = wiimote_getAccY(wiiControllerIndex);
                byte accZ = wiimote_getAccZ(wiiControllerIndex);
                
                double battery = wiimote_getBatteryLevel(wiiControllerIndex);
                
                
                GUILayout.Label("Yaw: " + yaw);
                GUILayout.Label("Pitch: " + pitch);
                GUILayout.Label("Roll: " + roll);
                
                GUILayout.Label("AccX: " + accX);
                GUILayout.Label("AccY: " + accY);
                GUILayout.Label("AccZ: " + accZ);
                
                GUILayout.Label("Battery: " + battery);
                
                
            } else
            {
                GUILayout.Label("Press the '1' and '2' buttons on your Wii Remote.");
            }
            
            GUILayout.EndHorizontal();
        }

    }
    
    void Update () 
    {

        CharacterController controller = GetComponent<CharacterController>();
        
        // Rotate player around y-axis
        if (wiimote_available (wiiControllerIndex)) {

            int accX = Mathf.Abs( wiimote_getAccX(wiiControllerIndex) - centerOffset);
            int accY = Mathf.Abs( wiimote_getAccY(wiiControllerIndex) - centerOffset);
            int accZ = Mathf.Abs( wiimote_getAccZ(wiiControllerIndex) - centerOffset);
            int[] values = {accX, accY, accZ};
            int maxAcc = Mathf.Max(values);

            // check if nunchuck is available, if it is we use it to rotate
            if(wiimote_isExpansionPortEnabled(wiiControllerIndex))
            {
                accX = wiimote_getNunchuckAccX(wiiControllerIndex) - 125;
                transform.Rotate(0, accX * 4 * Time.deltaTime, 0);
            } // If another controller is present we use that for rotation
            else if(wiimote_available(1)) 
            {
                accY = wiimote_getAccY(1) - 125;
                transform.Rotate(0, -accY * rotationSpeed * Time.deltaTime, 0);
            }


            if (controller.isGrounded) {
                moveDirection.y = 0;
                if (maxAcc > flickThreshold)
                {   
                    float speed = (maxAcc - flickThreshold)*flickScale;
                    speed = Mathf.Clamp(speed, 0, maxSpeed);
                    Vector3 force = transform.forward * speed * Time.deltaTime;
                    moveDirection += force;
                    soundEffectManager.playMoveSound();
                }

                if (wiimote_getButtonB(wiiControllerIndex)) {
                    Vector3 breakForce = moveDirection * -1 * breakPower * Time.deltaTime;
                    moveDirection += breakForce;
                    soundEffectManager.playBrakeSound(controller.velocity.magnitude);
                }

                float fric = Mathf.Clamp(100 - friction, 0, 100);
                fric = fric / 100; // convert to percentage
                moveDirection *= fric;
                if(moveDirection.magnitude < 0.1)
                    moveDirection = Vector3.zero;

                if(wiimote_isExpansionPortEnabled(wiiControllerIndex))
                {

                    if (wiimote_getButtonNunchuckZ(wiiControllerIndex) || wiimote_getButtonNunchuckC(wiiControllerIndex))
                    {
                        soundEffectManager.playJumpSound();
                        moveDirection.y = jumpSpeed;
                    }
                }
                else if(wiimote_available(1))
                {
                    if (wiimote_getButton1(1) || wiimote_getButton2(1))
                    {
                        soundEffectManager.playJumpSound();
                        moveDirection.y = jumpSpeed;
                    }
                }
            }

            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        name = hit.gameObject.name;
        Debug.Log(hit.normal);
        
        
        if(hit.gameObject.name.Equals("Walls"))
        {
            float mag = Vector3.Dot(moveDirection, hit.normal);
            moveDirection -= (mag * hit.normal);
        }
    }
}

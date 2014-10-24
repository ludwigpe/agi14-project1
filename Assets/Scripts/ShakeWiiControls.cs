using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// Shake wii controls. This is the scirpt that calls and handle input from the wiimotes and
/// controls the player. It handles the player the same way as ice controls do.
/// </summary>
public class ShakeWiiControls : MonoBehaviour {

    #region IMPORTS
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
    private static extern void wiimote_rumble( int which, float duration);
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
    #endregion 

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

    private Vector3 moveDirection = Vector3.zero;
    private byte centerOffset = 125;
    private PlaySoundEffect soundEffectManager;
    private bool wiimote1Available = false;
    private bool wiimote2Available = false;

    // Use this for initialization
    void Start () 
    {
        soundEffectManager = GetComponent<PlaySoundEffect>();
    }
    
    void Update () 
    {
        CharacterController controller = GetComponent<CharacterController>();
        wiimote1Available = wiimote_available(0);
        wiimote2Available = wiimote_available(1);
        // Rotate player around y-axis
        if (wiimote1Available) {

            int accX = Mathf.Abs( wiimote_getAccX(0) - centerOffset);
            int accY = Mathf.Abs( wiimote_getAccY(0) - centerOffset);
            int accZ = Mathf.Abs( wiimote_getAccZ(0) - centerOffset);
            int[] values = {accX, accY, accZ};
            int maxAcc = Mathf.Max(values);

            // check if nunchuck is available, if it is we use it to rotate
            if(wiimote_isExpansionPortEnabled(0))
            {
                accX = wiimote_getNunchuckAccX(0) - 125;
                transform.Rotate(0, accX * 4 * Time.deltaTime, 0);
            } // If another controller is present we use that for rotation
            else if(wiimote2Available) 
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
                    soundEffectManager.PlayMoveSound();
                }

                if (wiimote_getButtonB(0)) {
                    Vector3 breakForce = moveDirection * -1 * breakPower * Time.deltaTime;
                    moveDirection += breakForce;

                    // play brake sound, according to movement along x and z-axis
                    Vector2 forward = new Vector2(moveDirection.x, moveDirection.z);
                    soundEffectManager.PlayBrakeSound(forward.magnitude);
                }

                float fric = Mathf.Clamp(100 - friction, 0, 100);
                fric = fric / 100; // convert to percentage
                moveDirection *= fric;
                if(moveDirection.magnitude < 0.1)
                    moveDirection = Vector3.zero;

                if(wiimote_isExpansionPortEnabled(0))
                {
                    if (wiimote_getButtonNunchuckZ(0) || wiimote_getButtonNunchuckC(0))
                    {
                        soundEffectManager.PlayJumpSound();
                        moveDirection.y = jumpSpeed;
                    }
                }
                else if(wiimote2Available)
                {
                    if (wiimote_getButton1(1) || wiimote_getButton2(1))
                    {
                        soundEffectManager.PlayJumpSound();
                        moveDirection.y = jumpSpeed;
                    }
                }
            }

            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    /// <summary>
    /// Draws the controller info. Render the wiicontroler values inside the container Rect.
    /// </summary>
    /// <param name="container">Container.</param>
    /// <param name="controllerIndex">Controller index.</param>
    void DrawControllerInfo(Rect container, int controllerIndex)
    {
        GUILayout.BeginArea(container);
        bool available = (controllerIndex == 0) ? wiimote1Available : wiimote2Available;
        if(available)
        {
            GUILayout.Label("Wiimote " + controllerIndex + " found!");

            float yaw = wiimote_getYaw(controllerIndex);
            float pitch = wiimote_getPitch(controllerIndex);
            float roll = wiimote_getRoll(controllerIndex);
            
            byte accX = wiimote_getAccX(controllerIndex);
            byte accY = wiimote_getAccY(controllerIndex);
            byte accZ = wiimote_getAccZ(controllerIndex);
            
            double battery = wiimote_getBatteryLevel(controllerIndex);
            
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
        
        GUILayout.EndArea();
    }
    
    /// <summary>
    /// The player has collided with something.
    /// If the collision is with a wall we remove the velocity in the direction towards the wall.
    /// </summary>
    /// <param name="hit">Hit.</param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.name.Equals("Walls"))
        {
            float speed = Mathf.Pow(moveDirection.x, 2) + Mathf.Pow(moveDirection.z, 2);
            if( speed > 1 )
            {
                if(wiimote1Available)
                {
//                    wiimote_rumble(0, 1.0f);
                }
            }
            float mag = Vector3.Dot(moveDirection, hit.normal);
            moveDirection -= (mag * hit.normal);
        }
    }
}

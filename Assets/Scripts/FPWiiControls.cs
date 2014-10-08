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
    public bool DEBUGGING = false;
    private PlaySoundEffect soundEffectManager;
    #endregion
    
    // Use this for initialization
    void Start () {
//        wiiControllerIndex = gc.GetFirstPersonIndex();
        soundEffectManager = GetComponent<PlaySoundEffect>();
        wiiControllerIndex = 1;
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
                
                byte accX = wiimote_getAccX(wiiControllerIndex);
                byte accY = wiimote_getAccY(wiiControllerIndex);
                byte accZ = wiimote_getAccZ(wiiControllerIndex);
                
                double battery = wiimote_getBatteryLevel(wiiControllerIndex);
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

    
    // Update is called once per frame
    void Update () {
        CharacterController controller = GetComponent<CharacterController>();
        
        // Rotate player around y-axis
        if (wiimote_available (wiiControllerIndex)) {
            float accX = wiimote_getAccX (wiiControllerIndex) -125;
            
            transform.Rotate(0, accX * rotationSpeed * Time.deltaTime, 0);
            if (controller.isGrounded) {
                moveDirection.y = 0;
                if (wiimote_getButtonA(wiiControllerIndex))
                {
                    Debug.Log("YOu Did it");
                    moveDirection.y = jumpSpeed;
                    soundEffectManager.playJumpSound();
                }
                    
            }
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}

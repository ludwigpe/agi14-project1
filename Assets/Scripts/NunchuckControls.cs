using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class NunchuckControls : MonoBehaviour {

    [DllImport ("UniWii")]
    private static extern bool wiimote_available( int which );

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

    public float jumpSpeed = 8.0F;
    public float rotationSpeed = 50.0F;
    public float gravity = 10.0F;
    public bool DEBUGGING = false;
    private Vector3 moveDirection = Vector3.zero;
    private PlaySoundEffect soundEffectManager;
    private int wiiControllerIndex;
	// Use this for initialization
	void Start () {
        soundEffectManager = GetComponent<PlaySoundEffect>();
        wiiControllerIndex = 0;
	}

    void OnGUI()
    {
        if (DEBUGGING)
        {
            GUILayout.BeginVertical("box");
            if (wiimote_available (wiiControllerIndex) && wiimote_isExpansionPortEnabled(wiiControllerIndex))
            {
                byte stickX = wiimote_getNunchuckStickX(wiiControllerIndex);
                byte accX = wiimote_getNunchuckAccX(wiiControllerIndex);
                byte accZ = wiimote_getNunchuckAccZ(wiiControllerIndex);
                bool btnC = wiimote_getButtonNunchuckC(wiiControllerIndex);
                bool btnZ = wiimote_getButtonNunchuckZ(wiiControllerIndex);

                GUILayout.Label("AccX: " + accX);
                GUILayout.Label("AccZ: " + accZ);
                GUILayout.Label("Stick X: " + stickX);
                GUILayout.Label("Button C: " + btnC);
                GUILayout.Label("Button Z: " + btnZ);
                
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
        if (wiimote_available (wiiControllerIndex) && wiimote_isExpansionPortEnabled(wiiControllerIndex) ) 
        {


            float accX = wiimote_getNunchuckAccX(wiiControllerIndex) - 125;
            transform.Rotate(0, accX * rotationSpeed * Time.deltaTime, 0);
            if (controller.isGrounded) {
                moveDirection.y = 0;
                if (wiimote_getButtonNunchuckZ(wiiControllerIndex) || wiimote_getButtonNunchuckC(wiiControllerIndex))
                {
                    soundEffectManager.playJumpSound();
                    moveDirection.y = jumpSpeed;
                }
                    
            }
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
	}
}

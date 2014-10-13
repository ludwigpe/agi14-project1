using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// Wii controller handler.
/// </summary>
public class WiiControllerHandler : MonoBehaviour {

    [DllImport ("UniWii")]
    private static extern void wiimote_start();
    [DllImport ("UniWii")]
    private static extern void wiimote_stop();
    [DllImport ("UniWii")]
    private static extern int wiimote_count();
    [DllImport ("UniWii")]
    private static extern bool wiimote_available( int which );
    [DllImport ("UniWii")]
    private static extern double wiimote_getBatteryLevel( int which );
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccX(int which);
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccY(int which);
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccZ(int which);
    [DllImport ("UniWii")]
    private static extern float wiimote_getRoll(int which);
    [DllImport ("UniWii")]
    private static extern float wiimote_getPitch(int which);
    [DllImport ("UniWii")]
    private static extern float wiimote_getYaw(int which);

    public GameController gameController;
    public Rect batteryContainer;
    public Texture2D backgroundImg;
    public Texture2D foregroundImg;
    private bool isDone = false;

	// Use this for initialization
	void Start () {
        int c = wiimote_count();
        if( c <= 0)
            wiimote_start();
	}
	
	// Update is called once per frame
	void Update () {
        int c = wiimote_count();
        if (c >= 1 && !isDone)
        {
            gameController.PlayersConnected = true;
            isDone = true;
        }
	}

    void OnApplicationQuit() 
    {
        int c = wiimote_count();
        if( c > 0)
            wiimote_stop();
    }

    /// <summary>
    /// Display some text before controller is connected
    /// </summary>
    /// <param name="container">Container where to render inside.</param>
    /// <param name="controllerIndex">Wiimote controller index.</param>
    public void DrawConnectionGuide(Rect container, int controllerIndex)
    {
        if(!wiimote_available(controllerIndex))
        {
            GUILayout.BeginArea(container);
            GUILayout.Label("Press the '1' and '2' buttons on your Wii Remote.");
            GUILayout.EndArea();
        }
    }

    /// <summary>
    /// Draws an indicator of how much battery is left in controller with index controllerIndex.
    /// </summary>
    /// <param name="container">Container.</param>
    /// <param name="controllerIndex">controller index.</param>
    public void DrawBatteryIndicator(Rect container, int controllerIndex)
    {
        if (wiimote_available(controllerIndex))
        {
            GUILayout.BeginArea(container);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            float battery = (float)wiimote_getBatteryLevel(controllerIndex);
            batteryContainer.y = container.height - batteryContainer.height;

            Rect frontRect = batteryContainer;
            frontRect.width -= 20.0F;
            frontRect.width *= battery;
            frontRect.height = 20.0F;
            frontRect.y += 20.0F;
            frontRect.x += 15.0F;
            GUI.DrawTexture(frontRect, foregroundImg);
            GUI.DrawTexture(batteryContainer, backgroundImg);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    /// <summary>
    /// Draws the debug info. It will show the vital information retrieved from the wiimote
    /// </summary>
    /// <param name="container">Container.</param>
    /// <param name="controllerIndex">Controller index.</param>
    public void DrawDebugInfo(Rect container, int controllerIndex)
    {
        GUILayout.BeginArea(container);
        bool available = wiimote_available(controllerIndex);
        if (available)
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
        } 

        GUILayout.EndArea();
    }

}

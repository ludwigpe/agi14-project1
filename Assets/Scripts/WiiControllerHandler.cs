using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class WiiControllerHandler : MonoBehaviour {

    [DllImport ("UniWii")]
    private static extern void wiimote_start();
    
    [DllImport ("UniWii")]
    private static extern void wiimote_stop();
    
    [DllImport ("UniWii")]
    private static extern int wiimote_count();
    
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccX(int which);
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccY(int which);
    [DllImport ("UniWii")]
    private static extern byte wiimote_getAccZ(int which);

    public GameController gameController;
    private bool isDone = false;

	// Use this for initialization
	void Start () {
        int c = wiimote_count();
        if( c <= 0)
            wiimote_start();
	}
	
    void OnGUI()
    {

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

    public void drawWiimoteGUI(Rect rect)
    {
        int c = wiimote_count();
        GUILayout.BeginArea(rect);
        if (c > 0)
        {
            for (int i = 0; i < c; i++)
            {
                GUILayout.Label("Wiimote " + i + " is connected!");
            }
        } else
        {
            GUILayout.Label("Press 1 and 2 to start wiimote");
        }

        GUILayout.EndArea();
    }

}

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class WiiStart : MonoBehaviour {
    [DllImport("UniWii")]
    private static extern void wiimote_start();
    [DllImport("UniWii")]
    private static extern void wiimote_stop();
    [DllImport("UniWii")]
    private static extern int wiimote_count();
	// Use this for initialization
	void Start () {
        wiimote_start();
	}

    void OnGUI()
    {
        int c = wiimote_count();
        for (int i = 0; i < c; i++)
        {
            GUILayout.Label("Wiimote " + i + " connected!");
        }

        GUILayout.Label("Press 1 and 2 on wiimote to connect");

    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F5))
        { 
            // Load the game scene!
            Application.LoadLevel("start");
        }
	}

    void OnApplicationQuit()
    {
        wiimote_stop();
        Debug.Log("STOP IN WIISTART");
    }
}

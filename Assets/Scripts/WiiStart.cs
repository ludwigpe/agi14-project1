using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// Responsible for establishing the initial connection with the Wiimotes and Unity.
/// </summary>
public class WiiStart : MonoBehaviour {
    [DllImport("UniWii")]
    private static extern void wiimote_start();
    [DllImport("UniWii")]
    private static extern void wiimote_stop();
    [DllImport("UniWii")]
    private static extern int wiimote_count();
	
    /// <summary>
    /// Use this for initialization
	/// </summary>
	void Start () 
    {
        wiimote_start();
	}

    /// <summary>
    /// Render to GUI.
    /// </summary>
    void OnGUI()
    {
        int c = wiimote_count();
        for (int i = 0; i < c; i++)
        {
            GUILayout.Label("Wiimote " + i + " connected!");
        }

        GUILayout.Label("Press 1 and 2 on wiimote to connect");

    }

	/// <summary>
    /// Update is called once per frame
	/// </summary>
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.F5))
        { 
            // Load the game scene!
            Application.LoadLevel("TronLevel");
        }
	}

    /// <summary>
    /// Executed on application shut down.
    /// </summary>
    void OnApplicationQuit()
    {
        wiimote_stop();
    }
}

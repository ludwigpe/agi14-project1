using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class MenuScript : MonoBehaviour {
    [DllImport ("UniWii")]
    private static extern void wiimote_start();
    [DllImport ("UniWii")]
    private static extern void wiimote_stop();
    [DllImport ("UniWii")]
    private static extern int wiimote_count();
	// Use this for initialization
	void Start () {
        wiimote_start();
	}
	
    void OnGUI()
    {
        int c = wiimote_count();

        GUILayout.BeginArea(new Rect(Screen.width/2 - 70, Screen.height/2 -100, 140, 200));
        if (c == 0)
        {
            GUILayout.Label("Press 1 and 2 on the wii controller!");
        }
        else if (c == 1)
        {
            GUILayout.Label("Waiting for second player");
            GUILayout.Label("Press 1 and 2 on the wii controller!");
        } 
        else
        {
            if(GUILayout.Button("Start Game"))
            {
                Application.LoadLevel("start");
            }
            
        }

    }

	// Update is called once per frame
	void Update () {
	
	}
}

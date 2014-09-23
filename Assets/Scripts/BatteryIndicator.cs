using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class BatteryIndicator : MonoBehaviour {


    [DllImport ("UniWii")]
    private static extern int wiimote_count();
    [DllImport ("UniWii")]
    private static extern double wiimote_getBatteryLevel( int which );

    public Texture2D backgroundImg;
    public Texture2D foregroundImg;
    public Rect p1_batteryRect;
    public Rect p2_batteryRect;

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI()
    {
        int c = wiimote_count();
        //c = 2;
        for (int i = 0; i < c; i++)
        {
            float offset = i%2;
            double b = wiimote_getBatteryLevel(i);

            Debug.Log("b: " + b);

            float battery = (float)b;
            Debug.Log("Battery: " + battery);
            //battery = 1.0F;

            if(i == 0)
            {

                GUI.DrawTexture(p1_batteryRect, backgroundImg);
                Rect frontRect = p1_batteryRect;
                frontRect.width *= battery;
                GUI.DrawTexture(frontRect, foregroundImg);
            }
            else
            {
                GUI.DrawTexture(p2_batteryRect, backgroundImg);
                Rect frontRect = p2_batteryRect;
                frontRect.width *= battery;
                GUI.DrawTexture(frontRect, foregroundImg);
            }
        }

    }
}

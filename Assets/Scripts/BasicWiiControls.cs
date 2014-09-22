using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class BasicWiiControls : MonoBehaviour {
	#region interface
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
	private static extern double wiimote_getBatteryLevel( int which );
	#endregion

	#region variables
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 10.0F;
	public float friction = 5.0F;
	public float breakPower = 10.0F;
	public float rotationSpeed = 100.0F;
	private Vector3 moveDirection = Vector3.zero;
	#endregion

	// Use this for initialization
	void Start () {
		wiimote_start ();
	}

	private String display;
	
	void OnGUI() {
		int c = wiimote_count();
		Debug.Log (wiimote_available (0));
		Debug.Log (wiimote_available (1));
		Debug.Log (wiimote_available (2));
		Debug.Log (wiimote_available (3));
		
		Debug.Log (c);
		if (c>0) {
			display = "";
			for (int i=0; i<=c-1; i++) {
				display += "Wiimote " + i + " found!\n";
			}
			float yaw = wiimote_getYaw(0);
			float pitch = wiimote_getPitch(0);
			float roll = wiimote_getRoll(0);
			
			byte accX = wiimote_getAccX(0);
			byte accY = wiimote_getAccY(0);
			byte accZ = wiimote_getAccZ(0);

			double battery = wiimote_getBatteryLevel(0);

			GUILayout.BeginVertical("box");
			GUILayout.Label("Yaw: " + yaw);
			GUILayout.Label("Pitch: " + pitch);
			GUILayout.Label("Roll: " + roll);
			
			GUILayout.Label("AccX: " + accX);
			GUILayout.Label("AccY: " + accY);
			GUILayout.Label("AccZ: " + accZ);

			GUILayout.Label("Battery: " + battery);
			
			GUILayout.EndHorizontal();
			
		}
		else display = "Press the '1' and '2' buttons on your Wii Remote.";
		
		GUI.Label( new Rect(10,Screen.height-100, 500, 100), display);
	}

	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		
		// Rotate player around y-axis
		if (wiimote_available (0)) {
			float accX = wiimote_getAccX (0) -125;
			
			transform.Rotate(0, accX * rotationSpeed * Time.deltaTime, 0);
			if (controller.isGrounded) {
				moveDirection.y = 0;
				if (wiimote_getButtonUp(0)) {
					// player pressed up-key so applie some force to the movement
					Vector3 force = transform.forward * speed * Time.deltaTime;
					moveDirection += force;
				}
				if (wiimote_getButtonB(0)) {
					Vector3 breakForce = moveDirection * -1 * breakPower * Time.deltaTime;
					moveDirection += breakForce;
				}
				if (wiimote_getButtonA(0))
					moveDirection.y = jumpSpeed;
				
				float fric = Mathf.Clamp(100 - friction, 0, 100);
				fric = fric / 100; // convert to percentage
				moveDirection *= fric;
			}
			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move(moveDirection * Time.deltaTime);
		}
	}

	void OnApplicationQuit() {
		wiimote_stop();
	}
}

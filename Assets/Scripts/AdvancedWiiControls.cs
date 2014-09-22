using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class AdvancedWiiControls : MonoBehaviour {
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
	public float rotationSpeed = 10.0F;
	public byte flickMinAccY = 150;
	public byte precisionMinAccY = 130;
	public float precisionMaxSpeed = 1.5F;
	public float forceScale = 2.0F;
	public float precisionScale = 1.0F;
	public float maxForce = 100.0F;
    public float maxFlickTime = 0.5F;

	private Vector3 moveDirection = Vector3.zero;
	private byte maxAccY = 0;
	private bool hasStartedFlick = false;
    private float startFlickTime;
	#endregion
	
	// Use this for initialization
	void Start () {
		wiimote_start ();
	}
	
	private String display;
	
	void OnGUI() {
		int c = wiimote_count();

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
			float velocity = getCurrentSpeed();

			GUILayout.BeginVertical("box");
			GUILayout.Label("Yaw: " + yaw);
			GUILayout.Label("Pitch: " + pitch);
			GUILayout.Label("Roll: " + roll);
			
			GUILayout.Label("AccX: " + (accX));
			GUILayout.Label("AccY: " + (accY));
			GUILayout.Label("AccZ: " + (accZ));
			
			GUILayout.Label("Battery: " + battery);
			GUILayout.Label("velocity: " + velocity);
			GUILayout.EndHorizontal();
			
		}
		else display = "Press the '1' and '2' buttons on your Wii Remote.";
		
		GUI.Label( new Rect(10,Screen.height-100, 500, 100), display);
	}
	
//	 Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		
		// Rotate player around y-axis
		if (wiimote_available (0)) {
			transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0);

			if (controller.isGrounded) {
				moveDirection.y = 0;
//				chooseMovement();
                chooseMovementAdvanced();

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

	void chooseMovementAdvanced() 
	{
        byte accY = wiimote_getAccY (0);
		if (hasStartedFlick)
        {
            Debug.Log("has started flick!");
            maxAccY = (byte)Mathf.Max(maxAccY, accY);
            if (accY < flickMinAccY)
            {
                float dt = Time.time - startFlickTime;
                Debug.Log("has completed flick!");
                if(dt <= maxFlickTime)
                {
                    float diff = maxAccY - flickMinAccY;
                    advancedFlick(diff);
                }

                maxAccY = 0;
                hasStartedFlick = false;
            }


        }
        else
        {
            if(accY > flickMinAccY)
            {
                hasStartedFlick = true;
                startFlickTime = Time.time;
            }

        }

	}

	void chooseMovement()
	{
		byte accY = wiimote_getAccY (0);
		if (accY > flickMinAccY)
		{
			float diff = accY - flickMinAccY;
			basicFlick(diff);
		}
		else if (accY > precisionMinAccY)
		{
			float diff = accY - precisionMinAccY;
			precisionMovement(diff);
		}
	}
	void precisionMovement(float amount) 
	{
		float currentSpeed = getCurrentSpeed ();
		Vector3 movDir = moveDirection.normalized;
		float dot = Vector3.Dot (movDir, transform.forward);
		// calculate the relative speed in the new direction
		float relSpeed = currentSpeed * dot;

		float forceImpulse = amount*precisionScale;
		forceImpulse = Mathf.Clamp(forceImpulse, 0, maxForce);

		Debug.Log("relSpeed " + relSpeed);
		if (relSpeed + forceImpulse < precisionMaxSpeed) 
		{
			applyForceDelta (forceImpulse);
		}

	}
    void advancedPrecisionMovement(float amount)
    {
        float currentSpeed = getCurrentSpeed ();
        Vector3 movDir = moveDirection.normalized;
        float dot = Vector3.Dot (movDir, transform.forward);
        // calculate the relative speed in the new direction
        float relSpeed = currentSpeed * dot;
        
        float forceImpulse = amount*precisionScale;
        forceImpulse = Mathf.Clamp(forceImpulse, 0, maxForce);
        
        Debug.Log("relSpeed " + relSpeed);
        if (relSpeed + forceImpulse < precisionMaxSpeed) 
        {
            applyForce(forceImpulse);
        }
    }
	void basicFlick(float amount) 
    {

//		float forceImpulse = forceScale*accY + forceOffset + 10;
		float forceImpulse = amount*forceScale;
		forceImpulse = Mathf.Clamp(forceImpulse, 0, maxForce);
		applyForceDelta (forceImpulse);
			
			
	}

	void advancedFlick(float amount)
	{
        float forceImpulse = amount*forceScale;
        forceImpulse = Mathf.Clamp(forceImpulse, 0, maxForce);
        applyForce (forceImpulse);
	}
    void applyForce(float force)
    {
        Vector3 forceVec = transform.forward * force;
        moveDirection += forceVec;
    }
	void applyForceDelta(float force) 
	{
		Vector3 forceVec = transform.forward * force * Time.deltaTime;
		moveDirection += forceVec;
	}
	float getCurrentSpeed()
	{
		return moveDirection.magnitude;
	}
	void basicRotation() {
		float accX = wiimote_getAccX (0) -125;
		transform.Rotate(0, accX * rotationSpeed * Time.deltaTime, 0);
	}
	void OnApplicationQuit() {
		wiimote_stop();
	}
}

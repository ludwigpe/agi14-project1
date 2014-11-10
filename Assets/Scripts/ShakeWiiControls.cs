using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// Shake wii controls. This is the scirpt that calls and handle input from the wiimotes and
/// controls the player. It handles the player the same way as ice controls do.
/// </summary>
public class ShakeWiiControls : MonoBehaviour {

    #region IMPORTS
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
    private static extern bool wiimote_getButton1(int which);
    [DllImport ("UniWii")]
    private static extern bool wiimote_getButton2(int which);
    [DllImport ("UniWii")]
    private static extern void wiimote_rumble( int which, float duration);
    [DllImport ("UniWii")]
    private static extern double wiimote_getBatteryLevel( int which );
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
    #endregion 

    public bool DEBUGGING = false;

    public GameController gc;
    public int flickThreshold = 25;
    public float flickScale = 6.0F;
    public float jumpSpeed = 8.0F;
    public float friction = 5.0F;
    public float breakPower = 10.0F;
    public float rotationSpeed = 10.0F;
    public float maxSpeed = 20.0F;
    public float gravity = 10.0F;
    public int EMPThreshold = 90;
	public GameObject collisionEffectPrefab;

    public Transform emp_prefab;
    private Vector3 moveDirection = Vector3.zero;
    private byte centerOffset = 125;
    private PlaySoundEffect soundEffectManager;
    private bool wiimote1Available = false;
    private bool wiimote2Available = false;
    private GameController gameController;
    private CharacterController charController;
    private AnimationManager animationManager;


    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () 
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("Cannot find 'GameController' script");
        }
        // this is an ugly hack to get the pellet collision working properly.
        moveDirection = transform.forward * 0.1F;
        animationManager = GetComponent<AnimationManager>();
        soundEffectManager = GetComponent<PlaySoundEffect>();
        charController = GetComponent<CharacterController>();
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () 
    {
        wiimote1Available = wiimote_available(0);
        wiimote2Available = wiimote_available(1);
        
        // Rotate player around y-axis
        if (wiimote1Available && !gameController.ControlsDisabled) {

            int accX = Mathf.Abs( wiimote_getAccX(0) - centerOffset);
            int accY = Mathf.Abs( wiimote_getAccY(0) - centerOffset);
            int accZ = Mathf.Abs( wiimote_getAccZ(0) - centerOffset);
            int[] values = {accX, accY, accZ};
            int maxAcc = Mathf.Max(values);

            // check if nunchuck is available, if it is we use it to rotate
            if(wiimote_isExpansionPortEnabled(0))
            {
                accX = wiimote_getNunchuckAccX(0) - 125;
                transform.Rotate(0, accX * 4 * Time.deltaTime, 0);
            } // If another controller is present we use that for rotation
            else if(wiimote2Available) 
            {
                accY = wiimote_getAccY(1) - 125;
                transform.Rotate(0, -accY * rotationSpeed * Time.deltaTime, 0);
            }

            if (charController.isGrounded) {
                moveDirection.y = 0;
                if (maxAcc > flickThreshold)
                {   
                    float speed = (maxAcc - flickThreshold)*flickScale;
                    speed = Mathf.Clamp(speed, 0, maxSpeed);
                    Vector3 force = transform.forward * speed * Time.deltaTime;
                    moveDirection += force;
                    animationManager.PlayMoveAnimation();
                    soundEffectManager.PlayMoveSound();
                }

                if (wiimote_getButtonB(0)) {
                    Vector3 breakForce = moveDirection * -1 * breakPower * Time.deltaTime;
                    moveDirection += breakForce;

                    // play brake sound, according to movement along x and z-axis
                    Vector2 forward = new Vector2(moveDirection.x, moveDirection.z);
                    soundEffectManager.PlayBrakeSound(forward.magnitude);
                }

                float fric = Mathf.Clamp(100 - friction, 0, 100);
                fric = fric / 100; // convert to percentage
                moveDirection *= fric;
                if(moveDirection.magnitude < 0.1)
                    moveDirection = Vector3.zero;

                if(wiimote_isExpansionPortEnabled(0))
                {
                    if (wiimote_getButtonNunchuckZ(0) || wiimote_getButtonNunchuckC(0))
                    {
                        soundEffectManager.PlayJumpSound();
                        moveDirection.y = jumpSpeed;
                    }
                }
                else if(wiimote2Available)
                {
                    if (wiimote_getButton1(1) || wiimote_getButton2(1))
                    {
                        soundEffectManager.PlayJumpSound();
                        moveDirection.y = jumpSpeed;
                    }
                }
            }
            else
            {
                // pacman is mid air
                // Check some conditions to ensure pacman can release his EMP.
                if (gameController.IsEMPReady)
                {
                    int accZ2 = Mathf.Abs(wiimote_getAccZ(1) - centerOffset);
                    Debug.Log(accZ + " " + accZ2);
                    Debug.Log("thresh: " + EMPThreshold);
                    if (accZ >= EMPThreshold && accZ2 >= EMPThreshold)
                    {
                        // increase the moveDirection downwards to make it look like a forcefull impact on the ground.
                        moveDirection = Vector3.zero;
                        moveDirection.y -= 100;
                        // Trigger the EMP effect!
                        TriggerEMP();
                    }
                }
            }
            moveDirection.y -= gravity * Time.deltaTime;
            charController.Move(moveDirection * Time.deltaTime);
        }
    }

    /// <summary>
    /// Draws the controller info. Render the wiicontroler values inside the container Rect.
    /// </summary>
    /// <param name="container">Container.</param>
    /// <param name="controllerIndex">Controller index.</param>
    void DrawControllerInfo(Rect container, int controllerIndex)
    {
        GUILayout.BeginArea(container);
        bool available = (controllerIndex == 0) ? wiimote1Available : wiimote2Available;
        if(available)
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
            
        } else
        {
            GUILayout.Label("Press the '1' and '2' buttons on your Wii Remote.");
        }
        
        GUILayout.EndArea();
    }
    
    /// <summary>
    /// The player has collided with something.
    /// If the collision is with a wall we remove the velocity in the direction towards the wall.
    /// </summary>
    /// <param name="hit">Hit.</param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.name.Equals("Walls"))
        {
            float mag = Vector3.Dot(moveDirection, hit.normal);
            moveDirection -= (mag * hit.normal);
			
			if((mag*hit.normal).magnitude > 1 && collisionEffectPrefab != null)
			{
                soundEffectManager.PlayHitWallSound();
				GameObject ps = (GameObject) Instantiate(collisionEffectPrefab, hit.point, new Quaternion(0,0,0,0));
				ParticleSystemCustom psComponent = ps.GetComponent<ParticleSystemCustom>();
				psComponent.emitterDirection = hit.normal;
				psComponent.maxSpeed = (mag*hit.normal).magnitude;
			}
        }
    }

    /// <summary>
    /// This function will trigger Pac-Man's EMP-effect.
    /// This will both instatiate the EMP-special effect
    /// aswell as tell the Floor to start the shockwave.
    /// </summary>
    void TriggerEMP()
    {
        gameController.SavedPellets = 0;
        GameObject.FindWithTag("Level1").GetComponent<ShockWave>().StartShockWave(this.transform.position);
        soundEffectManager.PlayEMPSound();
        Instantiate(emp_prefab, transform.position, emp_prefab.localRotation);
    }
}

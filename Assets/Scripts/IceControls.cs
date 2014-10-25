using UnityEngine;
using System.Collections;

/// <summary>
/// The script that takes input from keyboard and controls the player
/// like he is on ice.
/// </summary>
public class IceControls : MonoBehaviour 
{
    public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 10.0F;
    public float friction = 5.0F;
    public float breakPower = 10.0F;
    public float rotationSpeed = 100.0F;
	public GameObject collisionEffectPrefab;
	private Vector3 moveDirection = Vector3.zero;

    // Link to components
    public Transform emp_prefab;
    private PlaySoundEffect soundEffectManager;
    private CharacterController charController;
    private AnimationManager animationManager;
    private GameController gameController;

    // Use this for initialization
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
	// Update is called once per frame
	void Update () 
    {
        if (!gameController.ControlsDisabled)
        {
            // Rotate player around y-axis
            transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0);
            if (charController.isGrounded)
            {
                moveDirection.y = 0;
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    // player pressed up-key so applie some force to the movement
                    Vector3 force = transform.forward * speed * Time.deltaTime;
                    moveDirection += force;
                    animationManager.PlayMoveAnimation();
                    soundEffectManager.PlayMoveSound();
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Vector3 breakForce = moveDirection * -1 * breakPower * Time.deltaTime;
                    moveDirection += breakForce;

                    // play brake sound, according to movement along x and z-axis
                    Vector2 forward = new Vector2(moveDirection.x, moveDirection.z);
                    soundEffectManager.PlayBrakeSound(forward.magnitude);
                }
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                    soundEffectManager.PlayJumpSound();
                }

                float fric = Mathf.Clamp(100 - friction, 0, 100);
                fric = fric / 100; // convert to percentage
                moveDirection *= fric;
            }
            else 
            { 
                // pacman is mid air
                // Check some conditions to ensure pacman can release his EMP.
                if (gameController.IsEMPReady)
                {
                    if (Input.GetKeyDown(KeyCode.E))
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
				GameObject ps = (GameObject) Instantiate(collisionEffectPrefab, hit.point, new Quaternion(0,0,0,0));
				ParticleSystemCustom psComponent = ps.GetComponent<ParticleSystemCustom>();
				psComponent.emitterDirection = hit.normal;
				psComponent.maxSpeed = (mag*hit.normal).magnitude;
			}
        }
    }

    /// <summary>
    /// This function will trigger Pac-Man's EMP-effect
    /// This will both instatiate the EMP-special effect aswell as tell the Floor to
    /// start the shockwave
    /// </summary>
    void TriggerEMP()
    {
        gameController.SavedPellets = 0;
        GameObject.FindWithTag("Level1").GetComponent<ShockWave>().StartShockWave(this.transform.position);
        soundEffectManager.PlayEMPSound();
        Instantiate(emp_prefab, transform.position, emp_prefab.localRotation);
    }
}

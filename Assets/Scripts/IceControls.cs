using UnityEngine;
using System.Collections;

public class IceControls : MonoBehaviour 
{
    public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 10.0F;
    public float friction = 5.0F;
    public float breakPower = 10.0F;
    public float rotationSpeed = 100.0F;
	private Vector3 moveDirection = Vector3.zero;
    
    private const float MIN_VEL_MAG = 0.1f; // If velocity is lesser than this, set it to zero

    // AudioSources
    public AudioSource audio_source_break;
    public AudioSource audio_source_move;
    private bool breakSoundStarted = false;

    // Sounds
    public AudioClip sound_move_pacman;
    public AudioClip sound_jump;
	
    // Use this for initialization
	void Start () 
    {
        audio.clip = sound_move_pacman;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    CharacterController controller = GetComponent<CharacterController>();

        // Stop break sound when button is released
        if (breakSoundStarted && !Input.GetKey(KeyCode.DownArrow))
        {
            audio_source_break.Stop();
            breakSoundStarted = false;
        }
    
        // Rotate player around y-axis
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0);
		if (controller.isGrounded) 
        {
            moveDirection.y = 0;
            if (Input.GetKey(KeyCode.UpArrow)) 
            {
                // player pressed up-key so applie some force to the movement
                Vector3 force = transform.forward * speed * Time.deltaTime;
                moveDirection += force;
                if (!audio_source_move.isPlaying)
                {
                    audio_source_move.Play();
                }
            }
            if (Input.GetKey(KeyCode.DownArrow)) 
            {
                Vector3 breakForce = moveDirection * -1 * breakPower * Time.deltaTime;
                moveDirection += breakForce;

                // Make sure to only play break sound once
                if (!audio_source_break.isPlaying && !breakSoundStarted && controller.velocity.magnitude > MIN_VEL_MAG)
                {
                    audio_source_break.Play();
                    breakSoundStarted = true;
                }
            }
            if (Input.GetButton("Jump"))
            {
                AudioSource.PlayClipAtPoint(sound_jump, transform.position);
                moveDirection.y = jumpSpeed;
            }
            
            float fric = Mathf.Clamp(100 - friction, 0, 100);
            fric = fric / 100; // convert to percentage
            moveDirection *= fric;
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
}

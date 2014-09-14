using UnityEngine;
using System.Collections;

public class IceControls : MonoBehaviour {
    public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 10.0F;
    public float friction = 5.0F;
    public float rotationSpeed = 100.0F;
	private Vector3 moveDirection = Vector3.zero;
	// Use this for initialization
	void Start () {
 
	}
	
	// Update is called once per frame
	void Update () {
	    CharacterController controller = GetComponent<CharacterController>();
    
        // Rotate player around y-axis
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0);
		if (controller.isGrounded) {
            moveDirection.y = 0;
            if (Input.GetKey(KeyCode.UpArrow)) {
                // player pressed up-key so applie some force to the movement
                Vector3 force = transform.forward * speed * Time.deltaTime;
                moveDirection += force;
            }
			if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;
            
            float fric = Mathf.Clamp(100 - friction, 0, 100);
            fric = fric / 100; // convert to percentage
            moveDirection *= fric;
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
	
}

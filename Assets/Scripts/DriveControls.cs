using UnityEngine;
using System.Collections;

public class DriveControls : MonoBehaviour 
{
    public float speed = 6.0F;
    public float breakSpeed = 1.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 10.0F;
    public float friction = 5.0F;
    public float rotationSpeed = 100.0F;
    private Vector3 moveDirection = Vector3.zero;
    public float currentSpeed = 0.0F;
	
    // Use this for initialization
	void Start () 
    {
    }
	
	// Update is called once per frame
	void Update () 
    {
        CharacterController controller = GetComponent<CharacterController>();

        // Rotate player around y-axis
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0);
       
        if (controller.isGrounded)
        {
            moveDirection = transform.forward;
            moveDirection.y = 0;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // player pressed up-key so apply some force to the movement
                currentSpeed += speed;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
               currentSpeed =  Mathf.Clamp(currentSpeed - breakSpeed, 0, currentSpeed);
            }
            float fric = Mathf.Clamp(100 - friction, 0, 100);
            fric = fric / 100; // convert to percentage
            currentSpeed *= fric;
            moveDirection *= currentSpeed;

            if (Input.GetButton("Jump")) moveDirection.y = jumpSpeed;  
        }
        
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
	}
}

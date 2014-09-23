using UnityEngine;
using System.Collections;

public class DeathCheck : MonoBehaviour
{
	// Death
	private const int DEATH_HEIGHT = 2;
	private const int DESTROY_HEIGHT = 500;

    // Link with GameController
    private GameController gameController;

    /// <summary>
    ///  Use this for initialization
    /// </summary>
    void Start()
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
    }

    /// <summary>
    /// Something has collided with the ControllerCollider.
    /// </summary>
    /// <param name="collider">Collider object.</param>
    void OnControllerColliderHit(ControllerColliderHit collider)
    {
        if (collider.gameObject.CompareTag("Enemy") && !gameController.GameLost && !gameController.GameWon)
        {
            Kill();
        }
    }

    /// <summary>
    /// Disables unit and marks the game as lost.
    /// </summary>
    public void Kill()
    {
        bool gameRunning = !gameController.GameLost && !gameController.GameWon;
        if (gameRunning) 
        {
            gameController.GameLost = true;

            MonoBehaviour[] scriptComponents = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scriptComponents)
            {
                script.enabled = false;
            }
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (transform.position.y < -DEATH_HEIGHT) {
			gameController.GameLost = true;
		} 
		if (transform.position.y < -DESTROY_HEIGHT) {
			Destroy(this.gameObject);
		}
    }
}

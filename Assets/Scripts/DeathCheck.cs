using UnityEngine;
using System.Collections;

/// <summary>
/// Death check.
/// This class is used to check if the player has been killed by a ghost 
/// or fallen out from the map.
/// </summary>
public class DeathCheck : MonoBehaviour
{
	// Death
	private const int DEATH_HEIGHT = 2;
	private const int DESTROY_HEIGHT = 120;

    // Link with GameController
    private GameController gameController;

    // Use this for initialization
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
                if(script != null)
                    script.enabled = false;
            }
            SetCameraDefaultPos();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if player has fallen below the map.
        if (transform.position.y < -DEATH_HEIGHT) {
            Kill();

		}
    }

    /// <summary>
    /// Reset the camera to the default topdown position
    /// </summary>
    void SetCameraDefaultPos()
    {
        GameObject defaultPos = GameObject.FindGameObjectWithTag("DEFAULTCAMPOS");
        this.camera.transform.position = defaultPos.transform.position;
        this.camera.transform.rotation = defaultPos.transform.rotation;
    }
}

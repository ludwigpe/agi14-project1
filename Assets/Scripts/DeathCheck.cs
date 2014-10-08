using UnityEngine;
using System.Collections;

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
            SetCameraDefaultPos();
            MonoBehaviour[] scriptComponents = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scriptComponents)
            {
                script.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -DEATH_HEIGHT) {
            Kill();

		}
    }

    void SetCameraDefaultPos()
    {
        GameObject defaultPos = GameObject.FindGameObjectWithTag("DEFAULTCAMPOS");
        this.camera.transform.position = defaultPos.transform.position;
        this.camera.transform.rotation = defaultPos.transform.rotation;
    }
}

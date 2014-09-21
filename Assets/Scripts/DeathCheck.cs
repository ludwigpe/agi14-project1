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
    /// Something has collided with Pacman.
    /// </summary>
    /// <param name="other">Collider object.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            gameController.GameLost = true;
            this.gameObject.SetActive(false);
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

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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            gameController.GameLost = true;
            MonoBehaviour[] scriptComponents = this.GetComponents<MonoBehaviour>();
            foreach(MonoBehaviour script in scriptComponents)
            {
                script.enabled = false;
            }
            other.audio.Stop();
            this.camera.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -DEATH_HEIGHT) {
			gameController.GameLost = true;
            this.camera.enabled = false;
		} 
		else if (transform.position.y < -DESTROY_HEIGHT) {
			Destroy(this.gameObject);
		}

    }
}

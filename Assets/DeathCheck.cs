using UnityEngine;
using System.Collections;

public class DeathCheck : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100)
        {
            gameController.GameLost = true;
            Destroy(this.gameObject);
        }
    }
}

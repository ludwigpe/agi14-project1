using UnityEngine;
using System.Collections;

// Component that makes an object follow another object.
public class FollowTargetScript : MonoBehaviour {

	public Transform target;
	public float persistentChaseDistance;		// At which distance the ghost will follow more precisely
	public float destinationUpdateFrequency;	// Amount of seconds between every update of the destination
	NavMeshAgent agent;

    // GameController link
    private GameController gameController;

	void Start () {
		agent = GetComponent<NavMeshAgent>();
        audio.Play();

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("Cannot find 'GameController' script");
        }

		// Update target position at a slowed down frequency to simulate AI stupidity
		InvokeRepeating("UpdateDestination", 0.0f, destinationUpdateFrequency);
	}
	
	// Update is called once per frame
	void Update () {


        // Stop idle sound when game is over
        if (gameController.GameLost || gameController.GameWon)
        {
            audio.Stop();
        }

		// Update the target destination every frame when the two objects are close to eachother
        if (!target)
            return;

		if (Vector3.Distance(transform.position, target.position) <= persistentChaseDistance)
        {
            UpdateDestination();
        }

			
	}

	// Update this objects target destination.
	void UpdateDestination () {
        if (!target)
        {
            agent.SetDestination(this.transform.position);
        } else
        {
            agent.SetDestination(target.position);
        }
		
	}
}

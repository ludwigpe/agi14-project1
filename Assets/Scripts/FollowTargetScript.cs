using UnityEngine;
using System.Collections;

/// <summary>
/// This class makes the object it is attached to follow a target in a stupid way.
/// </summary>
public class FollowTargetScript : MonoBehaviour
{
    // Follow Target
    public Transform target;
    public float persistentChaseDistance;		// At which distance the ghost will follow more precisely
    public float destinationUpdateFrequency;	// Amount of seconds between every update of the destination
    NavMeshAgent agent;
    private bool followTarget = true;

    // GameController link
    private GameController gameController;

    /// <summary>
    /// Initializes the gameobject.
    /// </summary>
    void Start()
    {
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

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Stop idle sound when game is over
        if (gameController.GameLost || gameController.GameWon)
        {
            audio.Stop();
            CancelInvoke("UpdateDestination");
        }
        else if (followTarget)
        {
            // Update the target destination every frame when the two objects are close to eachother
            if (!target)
                return;

            if (Vector3.Distance(transform.position, target.position) <= persistentChaseDistance)
            {
                UpdateDestination();
            }
        }
	}

    /// <summary>
    /// Will stop the agent and cancel updates to its pathfinding.
    /// </summary>
    public void StopFollowingTarget()
    {
        agent.Stop();
        followTarget = false;

        CancelInvoke("UpdateDestination");
    }

    /// <summary>
    /// Agent resumes its following of target (and pathfinding resumes).
    /// </summary>
    public void ResumeFollowingTarget()
    {
        agent.Resume();
        followTarget = true;

        // Update target position at a slowed down frequency to simulate AI stupidity
        InvokeRepeating("UpdateDestination", 0.0f, destinationUpdateFrequency);
    }

	/// <summary>
    /// Updates the destination for the NavMeshAgents path.
    /// </summary>
	void UpdateDestination() 
    {
        if (!target)
        {
            agent.SetDestination(this.transform.position);
            Debug.Log("MY");
        } else
        {
            agent.SetDestination(target.position);
        }
	}
}
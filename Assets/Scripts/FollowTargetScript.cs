using UnityEngine;
using System.Collections;

/// <summary>
/// Component that makes an object follow another object.
/// </summary>
 
public class FollowTargetScript : MonoBehaviour
{
    public Transform target;
    public float persistentChaseDistance;		// At which distance the ghost will follow more precisely
    public float destinationUpdateFrequency;	// Amount of seconds between every update of the destination
    NavMeshAgent agent;

    // GameController link
    private GameController gameController;

    /// <summary>
    /// Initialize Ghost.
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
        }

        // Update the target destination every frame when the two objects are close to eachother
        if (Vector3.Distance(transform.position, target.position) <= persistentChaseDistance)
            UpdateDestination();
    }

    /// <summary>
    /// Ghost has collided with something.
    /// </summary>
    /// <param name="collision">Collision object.</param>
    void OnCollisionEnter(Collision collision)
    {
        GameObject collidee = collision.gameObject;

        if (collidee.CompareTag("Player") && !gameController.GameLost && !gameController.GameWon)
        {
            DeathCheck deathCheck = collidee.GetComponent<DeathCheck>();
            if (!deathCheck.IsDead)
            {
                CharacterController charController = collidee.GetComponent<CharacterController>();
                charController.enabled = false;
                gameController.ControlsDisabled = true;

                AnimationManager animationManager = collidee.GetComponent<AnimationManager>();
                animationManager.PlayDeathAnimation();

                PlaySoundEffect soundEffectManager = collidee.GetComponent<PlaySoundEffect>();
                soundEffectManager.playLifeLostSound();

                deathCheck.IsDead = true;
            }
        }
    }

    /// <summary>
    /// Update this objects target destination.
    /// </summary>
    void UpdateDestination()
    {
        agent.SetDestination(target.position);
    }
}

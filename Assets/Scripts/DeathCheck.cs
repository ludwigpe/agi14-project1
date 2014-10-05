using UnityEngine;
using System.Collections;

/// <summary>
/// DeathCheck is responsible for checking whether
/// the unit should die or not. 
/// </summary>
public class DeathCheck : MonoBehaviour
{
	// Death
	private const int DEATH_HEIGHT = 2;
	private const int DESTROY_HEIGHT = 500;

    // Link with GameController
    private GameController gameController;
    private AnimationManager animationManager;
    private PlaySoundEffect soundEffectManager;

    private bool isDead = false;

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

        animationManager = GetComponent<AnimationManager>();
        soundEffectManager = GetComponent<PlaySoundEffect>();
    }

    /// <summary>
    /// Something has collided with the ControllerCollider.
    /// </summary>
    /// <param name="collider">Collider object.</param>
    void OnControllerColliderHit(ControllerColliderHit collider)
    {
        if (!isDead && collider.gameObject.CompareTag("Enemy") && !gameController.GameLost && !gameController.GameWon)
        {
            CharacterController charController = GetComponent<CharacterController>();
            charController.enabled = false;
            gameController.ControlsDisabled = true;

            soundEffectManager.PlayLifeLostSound();
            animationManager.PlayDeathAnimation();
            isDead = true;
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

    public bool IsDead
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
        }
    }
}

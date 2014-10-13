using UnityEngine;
using System.Collections;

/// <summary>
/// Handles what happens when something collides with a pellet.
/// </summary>
public class Pellet_Collision : MonoBehaviour 
{
	public GameObject pickupParticleSystem;

	// How many points one pellet is worth
	public int scoreValue = 10;

	// Link to the game controller
	private GameController gameController;

    // Sounds
    public AudioClip sound_consume_pellet;

	/// <summary>
    /// Use this for initialization.
	/// </summary>
	void Start () 
    {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		else
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
		gameController.IncrementPelletCounter();
	}
	
	/// <summary>
	/// Something has collided with the pellet.
	/// </summary>
	/// <param name="other">Collider object.</param>
	void OnTriggerEnter(Collider other) 
    {
		if (other.gameObject.CompareTag("Player")) 
        {
			gameController.AddScore(scoreValue);
			gameController.DecrementPelletCounter();
            
            float comboValue = gameController.IncreaseComboCounter();
            PlaySoundEffect playSoundEffect = other.gameObject.GetComponent<PlaySoundEffect>();
            playSoundEffect.PlayEatPellet(comboValue);

			Destroy (this.gameObject);
			Instantiate(pickupParticleSystem, transform.position, transform.rotation);
		}
	}
}

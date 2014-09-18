﻿using UnityEngine;
using System.Collections;

public class Pellet_Collision : MonoBehaviour 
{
	// How many points one pellet is worth
	public int scoreValue = 10;

	// Link to the game controller
	private GameController gameController;

    // Sounds
    public AudioClip sound_consume_pellet;

	// Use this for initialization
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
            AudioSource.PlayClipAtPoint(sound_consume_pellet, transform.position);
			Destroy (this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () 
    {
	}
}
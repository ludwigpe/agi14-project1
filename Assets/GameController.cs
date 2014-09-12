using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	// Keeps track of nr of pellets left, when zero => victory
	private int nrPelletsLeft;

	// Text displayed at completion of game
	public GUIText victoryText;
	private bool gameIsOver = false;

	// Score counter
	public GUIText scoreText;
	private int scoreCounter;

	// Use this for initialization
	void Start () {
		scoreCounter = 0;
		UpdateScore ();
	}
	
	// Update is called once per frame
	void Update () {
		if( !victoryText.guiText.enabled && nrPelletsLeft <= 0 ){
			victoryText.guiText.enabled = true;
			gameIsOver = true;
		}
	}
	
	/// <summary>
	/// Adds points to the score.
	/// </summary>
	/// <param name="points">Amount of points to add.</param>
	public void AddScore(int points){
		scoreCounter += points;
		UpdateScore ();
	}

	/// <summary>
	/// Increments the pellet counter.
	/// </summary>
	public void IncrementPelletCounter(){
		nrPelletsLeft++;
	}

	/// <summary>
	/// Decrements the pellet counter.
	/// </summary>
	public void DecrementPelletCounter(){
		nrPelletsLeft--;
	}

	/// <summary>
	/// Updates the score.
	/// </summary>
	void UpdateScore() {
		scoreText.text = "Score: " + scoreCounter;
	}
}

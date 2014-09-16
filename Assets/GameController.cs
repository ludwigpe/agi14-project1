using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    // Connection to player object
    public GameObject player;

    // Game time 
    public GUIText timeText;
    private int secondsPassed;
    private const int MAX_TIME = 255;

	// Keeps track of nr of pellets left, when zero => victory
	private int nrPelletsLeft;

	// Text displayed at completion of game
	public GUIText victoryText;
    public GUIText failureText;
	private bool gameLost = false;
    private bool gameWon = false;
    private bool gameIsOver = false;

	// Score counter
	public GUIText scoreText;
	private int scoreCounter;

	// Use this for initialization
	void Start () {
		scoreCounter = 0;
        secondsPassed = 0;
		UpdateScore ();
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameIsOver)
        {
            UpdateTimeText();
            CheckVictoryConditions();

            if (gameWon)
            {
                victoryText.guiText.enabled = true;
                gameIsOver = true;
            }
            else if (gameLost)
            {
                failureText.guiText.enabled = true;
                gameIsOver = true;
            }
        }
        else{
        }
	}

    void OnGUI(){
        if (gameIsOver){
            // Create a restart button
            if (GUI.Button(new Rect(Screen.width / 2 - 140, Screen.height / 2 + 30, 200, 40), "Restart"))
            {
                Application.LoadLevel("start");
            }
        }
    }

    /// <summary>
    /// Updates the time text to time passed since beginning of the game in seconds.
    /// </summary>
    private void UpdateTimeText(){
        secondsPassed = (int)Mathf.Floor(Time.timeSinceLevelLoad);
        timeText.text = "Time left: " + (MAX_TIME - secondsPassed);
    }

    /// <summary>
    /// Check victory/losing conditions.
    /// </summary>
    private void CheckVictoryConditions(){
        if (nrPelletsLeft <= 0){
            gameWon = true;
        }
        else if (secondsPassed > MAX_TIME){
            gameLost = true;
            Destroy(player.gameObject);
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

    #region Accessors
    public bool GameLost{
        get
        {
            return gameLost;
        }
        set
        {
            gameLost = value;
        }
    }

    public bool GameWon
    {
        get
        {
            return gameWon;
        }
        set
        {
            gameWon = value;
        }
    }
    #endregion
}

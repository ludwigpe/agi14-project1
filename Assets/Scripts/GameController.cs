using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class GameController : MonoBehaviour {

    [DllImport ("UniWii")]
    private static extern void wiimote_start();
    [DllImport ("UniWii")]
    private static extern void wiimote_stop();
    [DllImport ("UniWii")]
    private static extern int wiimote_count();

    // Connection to player object
    public GameObject player;
    public Transform spawnPoint;
    private bool playerCreated = false;

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
	void Start () 
    {
		scoreCounter = 0;
        secondsPassed = 0;
		UpdateScore ();
//        wiimote_start();

	}

    void InstatiatePlayer()
    {
        playerCreated = true;
        GameObject playerObject;
        playerObject = Instantiate(player, spawnPoint.position, Quaternion.identity) as GameObject;
        playerObject.GetComponent<FPWiiControls>().gc = this;
        playerObject.GetComponent<ShakeWiiControls>().gc = this;
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

    void OnGUI()
    {
        int c = wiimote_count();
        GUILayout.BeginVertical("box");
        if (c == 0)
        {
            GUILayout.Label("Press 1 and 2 on the wii controller!");
        }
        else if (c == 1)
        {
            GUILayout.Label("Waiting for second player");
            GUILayout.Label("Press 1 and 2 on the wii controller!");
        } 
        else
        {
//            if(!playerCreated)
//                InstatiatePlayer();
        }

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
    /// <summary>
    /// Gets wiimote index for the third person controls.
    /// </summary>
    /// <returns>The third person index.</returns>
    public int GetThirdPersonIndex()
    {
        return 0;
    }
    /// <summary>
    /// Gets wiimote index for the first person controls.
    /// </summary>
    /// <returns>The first person index.</returns>
    public int GetFirstPersonIndex()
    {
        return 1;
    }
    /// <summary>
    /// Raises the application quit event.
    /// Close all connections to wiimotes
    /// </summary>
    void OnApplicationQuit() 
    {
//        wiimote_stop();
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

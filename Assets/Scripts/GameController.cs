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
    private bool gameIsOver = true;
    private string playerName = "";
    private const int MAX_NAME_LENGTH = 3;

	// Score counter
	public GUIText scoreText;
	private int scoreCounter;

	// Use this for initialization
	void Start () 
    {
        ResetGame();
		UpdateScore ();
        wiimote_start();

	}

    void InstatiatePlayer()
    {

        playerCreated = true;
        GameObject playerObject;
        playerObject = Instantiate(player, spawnPoint.position, Quaternion.identity) as GameObject;
        playerObject.GetComponent<FPWiiControls>().gc = this;
        playerObject.GetComponent<ShakeWiiControls>().gc = this;
        Camera.main.GetComponent<SmoothFollow>().target = playerObject.transform;

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
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                gameIsOver = true;

            }
            else if (gameLost)
            {
                failureText.guiText.enabled = true;
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                gameIsOver = true;

            }
        }
        else{
        }   
	}

    void OnGUI()
    {
        int c = wiimote_count();


        if (gameIsOver)
        {
            GUILayout.BeginArea(new Rect(Screen.width/2 - 70, Screen.height/2 -100, 140, 200));
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
                if(gameLost || gameWon)
                {
                    if(GUILayout.Button("Reset game"))
                    {
                        Application.LoadLevel("start");
                    }
                    playerName = GUILayout.TextField(playerName, MAX_NAME_LENGTH);

                }
                else
                {
                    if(GUILayout.Button("Start Game"))
                    {
                        InstatiatePlayer();
                        gameIsOver = false;
                    }
                }

            }
            GUILayout.EndArea();
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
//            Destroy(player.gameObject);
//            playerCreated = false;
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
        wiimote_stop();
    }

    void ResetGame()
    {
        gameIsOver = true;
        gameWon = false;
        gameLost = false;
        playerCreated = false;

        scoreCounter = 0;
        secondsPassed = 0;
        victoryText.enabled = false;
        failureText.enabled = false;

    }

    IEnumerator FadeEndScreen(GUIText gui)
    {
        for (float f = 1f; f >= 0; f -= 0.1f) 
        {
            Color c = gui.color;
            c.a = f;
            gui.color = c;
            yield return new WaitForSeconds(.1f);
        }
        ResetGame();

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

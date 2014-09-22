using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

<<<<<<< HEAD
public class GameController : MonoBehaviour {

    [DllImport ("UniWii")]
    private static extern void wiimote_start();
    [DllImport ("UniWii")]
    private static extern void wiimote_stop();
    [DllImport ("UniWii")]
    private static extern int wiimote_count();

    public bool DEBUGGING;
=======
/// <summary>
/// The GameController component is responsible for maintaining overall
/// gameplay functionality such as spawning AI and so forth.
/// </summary>
public class GameController : MonoBehaviour
{
>>>>>>> 5f0b5ce... High Score System
    // Connection to player object
    public GameObject playerPrefab;
    private GameObject player;
    public Transform spawnPoint;
//    private bool playerCreated = false;


    // Game time
    public GUIText timeText;
    private int secondsPassed;
    private const int MAX_TIME = 60; // In seconds, max time that a game session is allowed to take

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

    // Sound clips
    public AudioClip sound_victory;
    public AudioClip sound_lost;

    // Prefabs
    public Transform ghost_prefab; // Prefab for ai ghosts

    // AI Spawn Points (to disable a certain AI simple skip giving it a spawn pos)
    public Transform spawn_pos_blinky;
    public Transform spawn_pos_inky;
    public Transform spawn_pos_pinky;
    public Transform spawn_pos_clyde;

    // AI Materials
    public Material inky_mat;
    public Material pinky_mat;
    public Material clyde_mat;

	/// Use this for initialization
	void Start ()
    {
        ResetGame();
		UpdateScore ();
        if(!DEBUGGING)
            wiimote_start();

	}

    void InstatiatePlayer()
    {

        player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity) as GameObject;
        player.GetComponent<FPWiiControls>().gc = this;
        player.GetComponent<ShakeWiiControls>().gc = this;
        if (DEBUGGING)
        {
            player.GetComponent<FPWiiControls>().enabled = false;
            player.GetComponent<ShakeWiiControls>().enabled = false;
        }
        Camera.main.GetComponent<SmoothFollow>().target = player.transform;
        Camera.main.rect = new Rect(0.0F, 0.0F, 0.5F, 1.0F);

    }
    /// <summary>
    /// Creates and setups the four AI ghosts.
    /// </summary>
    void Instantiate_AI()
    {
        Transform ai_ghost;
        FollowTargetScript follow_target_script;
        Renderer mesh_renderer;

        // Blinky
        if (spawn_pos_blinky)
        {
            ai_ghost = (Transform)Instantiate(ghost_prefab, spawn_pos_blinky.position, spawn_pos_blinky.rotation);
            follow_target_script = ai_ghost.GetComponent<FollowTargetScript>();
            follow_target_script.target = player.transform;
        }

        // Pinky
        if (spawn_pos_pinky)
        {
            ai_ghost = (Transform)Instantiate(ghost_prefab, spawn_pos_pinky.position, spawn_pos_pinky.rotation);
            follow_target_script = ai_ghost.GetComponent<FollowTargetScript>();
            follow_target_script.target = player.transform;
            mesh_renderer = ai_ghost.GetComponentInChildren<Renderer>();
            mesh_renderer.material = pinky_mat;
        }

        // Inky
        if (spawn_pos_inky)
        {
            ai_ghost = (Transform)Instantiate(ghost_prefab, spawn_pos_inky.position, spawn_pos_inky.rotation);
            follow_target_script = ai_ghost.GetComponent<FollowTargetScript>();
            follow_target_script.target = player.transform;
            mesh_renderer = ai_ghost.GetComponentInChildren<Renderer>();
            mesh_renderer.material = inky_mat;
        }

        // Clyde
        if (spawn_pos_clyde)
        {
            ai_ghost = (Transform)Instantiate(ghost_prefab, spawn_pos_clyde.position, spawn_pos_clyde.rotation);
            follow_target_script = ai_ghost.GetComponent<FollowTargetScript>();
            follow_target_script.target = player.transform;
            mesh_renderer = ai_ghost.GetComponentInChildren<Renderer>();
            mesh_renderer.material = clyde_mat;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (!gameIsOver)
        {
            UpdateTimeText();
            CheckVictoryConditions();

            if (gameWon)
            {
                Camera.main.rect = new Rect(0.0F, 0.0F, 1.0F, 1.0F);
                victoryText.guiText.enabled = true;
                gameIsOver = true;
                AudioSource.PlayClipAtPoint(sound_victory, transform.position);
                MonoBehaviour[] scriptComponents = player.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scriptComponents)
                {
                    script.enabled = false;
                }
            }
            else if (gameLost)
            {
                Camera.main.rect = new Rect(0.0F, 0.0F, 1.0F, 1.0F);
                failureText.guiText.enabled = true;
                gameIsOver = true;
                AudioSource.PlayClipAtPoint(sound_lost, transform.position);
            }
        }
        else{
        }
	}

    void OnGUI()
    {
        int c = 2;
        if(!DEBUGGING)
            c = wiimote_count();


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
                        Instantiate_AI();
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

    private void UpdateTimeText()
    {
        secondsPassed = (int)Mathf.Floor(Time.timeSinceLevelLoad);
        timeText.text = "Time left: " + (MAX_TIME - secondsPassed);
    }

    /// <summary>
    /// Check victory/losing conditions.
    /// </summary>
    private void CheckVictoryConditions()
    {
        if (nrPelletsLeft <= 0)
        {
            gameWon = true;
        }
        else if (secondsPassed >= MAX_TIME)
        {
            gameLost = true;
            MonoBehaviour[] scriptComponents = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scriptComponents)
            {
                script.enabled = false;
            }
        }
    }

	/// <summary>
	/// Adds points to the score.
	/// </summary>
	/// <param name="points">Amount of points to add.</param>
	public void AddScore(int points)
    {
		scoreCounter += points;
		UpdateScore ();
	}

	/// <summary>
	/// Increments the pellet counter.
	/// </summary>

	public void IncrementPelletCounter()
    {
		nrPelletsLeft++;
	}

	/// <summary>
	/// Decrements the pellet counter.
	/// </summary>

	public void DecrementPelletCounter()
    {

		nrPelletsLeft--;
	}

	/// <summary>
	/// Updates the score.
	/// </summary>
	void UpdateScore()
    {
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
        if(!DEBUGGING)
            wiimote_stop();
    }

    void ResetGame()
    {
        gameIsOver = true;
        gameWon = false;
        gameLost = false;
//        playerCreated = false;

        scoreCounter = 0;
        secondsPassed = 0;
        victoryText.enabled = false;
        failureText.enabled = false;

    }
    #region Accessors
    public bool GameLost
    {

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

    public int Score
    {
        get
        {
            return scoreCounter;
        }
    }
    #endregion
}

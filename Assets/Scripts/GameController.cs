using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// The GameController component is responsible for maintaining overall
/// gameplay functionality such as spawning AI and so forth.
/// </summary>
public class GameController : MonoBehaviour {

    public bool DEBUGGING;
    public Rect restartButton;

    // Connection to player object
    public GameObject playerPrefab;
    private GameObject player;
    public Transform spawnPoint;

    public const int MAX_TIME = 90; // In seconds, max time that a game session is allowed to take
    private float inGameTimePassed;

	// Keeps track of nr of pellets left, when zero => victory
	private int nrPelletsLeft;
	private bool gameLost = false;
    private bool gameWon = false;
    private bool gameIsOver = true;
    private bool controlsDisabled = false;
    private bool playersConnected = false;
    private bool gameStarted = false;

	private int scoreCounter;

    // Combo counter
    private float comboCounter;                 // Current combo value
    private float comboDecayRate = 1F;          // At which rate that the combo should decay
    private float comboIncrease = 0.75F;        // How much combo should be increased per pellet consumed
    private Color minComboColor = Color.green;  // Color of lowest combo
    private Color maxComboColor = Color.red;    // Color of highest combo
    private float useMaxComboColorValue = 5F;   // Value at which the maxComboColor is used

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

	//// <summary>
    /// Use this for initialization.
    /// </summary>
	void Start ()
    {
        ResetGame();
	}

    /// <summary>
    /// Instatiate playerprefab to the scene. Set the camera to follow the player.
    /// </summary>
    void InstatiatePlayer()
    {
        player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity) as GameObject;
        player.GetComponent<ShakeWiiControls>().gc = this;
        if (DEBUGGING)
        {
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
        if(Input.GetKeyDown(KeyCode.F2))
        {
            Application.LoadLevel("start");
        }
        if (!gameIsOver && gameStarted)
        {
            inGameTimePassed += Time.deltaTime;
            UpdateComboCounter(Time.deltaTime);
            // check if player has won or lost
            CheckVictoryConditions();

            if (gameWon)
            {
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
                gameIsOver = true;
                AudioSource.PlayClipAtPoint(sound_lost, transform.position);
            }
 
        }

        if (gameIsOver && !gameStarted)
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                InstatiatePlayer();
                Instantiate_AI();
                gameIsOver = false;
                gameStarted = true;
            }
        }
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
        else if (inGameTimePassed >= MAX_TIME)
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
    /// Decreases the combo counter based on time passed.
    /// </summary>
    /// <param name="dt">Amount of seconds passed.</param>
    private void UpdateComboCounter(float dt)
    {
        comboCounter = Mathf.Max(comboCounter - dt * comboDecayRate, 1);
    }

    /// <summary>
    /// Increases the value of the combo counter.
    /// </summary>
    /// <returns>Current value of combo counter.</returns>
    public float IncreaseComboCounter()
    {
        comboCounter += comboIncrease;
        return comboCounter;
    }

    public int GetComboCounter()
    {
        return Mathf.FloorToInt(comboCounter);
    }

	/// <summary>
	/// Adds points to the score.
	/// </summary>
	/// <param name="points">Amount of points to add.</param>
	public void AddScore(int points)
    {
		scoreCounter += points;
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
    /// Resets the game.
    /// </summary>
    void ResetGame()
    {
        gameIsOver = true;
        gameWon = false;
        gameLost = false;
        comboCounter = 1f;
        scoreCounter = 0;
        inGameTimePassed = 0;
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

    public int SecondsLeft
    {
        get
        {
            return MAX_TIME - Mathf.FloorToInt(inGameTimePassed);
        }
    }

    public bool GameIsOver
    {
        get
        {
            return gameIsOver;
        }
        set
        {
            gameIsOver = value;
        }
    }

    public bool PlayersConnected
    {

        get
        {
            return playersConnected;
        }
        set
        {
            playersConnected = value;
        }
    }

    public bool ControlsDisabled
    {
        get
        {
            return controlsDisabled;
        }
        set
        {
            controlsDisabled = value;
        }
    }
    #endregion
}

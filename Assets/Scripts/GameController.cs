using UnityEngine;
using System.Collections;

/// <summary>
/// The GameController component is responsible for maintaining overall 
/// gameplay functionality such as spawning AI and so forth.
/// </summary>
public class GameController : MonoBehaviour 
{
    // Connection to player object
    public GameObject player;

    // Game time 
    public GUIText timeText;
    private float inGameTimePassed;   // In seconds
    private const int MAX_TIME = 255; // In seconds, max time that a game session is allowed to take

	// Keeps track of nr of pellets left, when zero => victory
	private int nrPelletsLeft;

	// Text displayed at completion of game
	public GUIText victoryText;
    public GUIText failureText;

    // Booleans
	private bool gameLost = false;
    private bool gameWon = false;
    private bool gameIsOver = false;
    private bool gameStarted = false;
    private bool controlsDisabled = false;

	// Score counter
	public GUIText scoreText;
	private int scoreCounter;
    
    // Combo counter
    public GUIText comboText;                   // Shows current achieved pellet combo
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

	/// Use this for initialization
	void Start () 
    {
		scoreCounter = 0;
        comboCounter = 1f;
        inGameTimePassed = 0;
		UpdateScoreText();
        Instantiate_AI();
        gameStarted = true;
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
	
	/// <summary>
    /// Update is called once per frame.
	/// </summary>
	void Update () 
    {
        if (!gameIsOver && gameStarted)
        {
            UpdateTimeText();
            UpdateComboText();
            UpdateComboCounter(Time.deltaTime);
            CheckVictoryConditions();

            if (gameWon)
            {
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
                failureText.guiText.enabled = true;
                gameIsOver = true;
                AudioSource.PlayClipAtPoint(sound_lost, transform.position);
            }
        }
	}

    /// <summary>
    /// Is called once (or more) per frame, updates the GUI.
    /// </summary>
    void OnGUI()
    {
        if (gameIsOver)
        {
            // Create a restart button
            if (GUI.Button(new Rect(Screen.width / 2 - 140, Screen.height / 2 + 240, 280, 40), "Restart"))
            {
                Application.LoadLevel("start");
            }
        }
    }

    /// <summary>
    /// Updates the time text to the time passed since beginning of the game (in seconds).
    /// </summary>
    void UpdateTimeText()
    {
        inGameTimePassed += Time.deltaTime;
        int secondsPassed = Mathf.FloorToInt(inGameTimePassed);
        timeText.text = "Time left: " + (MAX_TIME - secondsPassed);
    }

    /// <summary>
    /// Updates the text used to display current pellet combo.
    /// </summary>
    void UpdateComboText()
    {
        float colorScale = Mathf.Clamp(comboCounter / useMaxComboColorValue, 0, 1);
        comboText.text = "Combo: " + Mathf.FloorToInt(comboCounter) + "X";
        comboText.color = Color.Lerp(minComboColor, maxComboColor, colorScale);
    }

    /// <summary>
    /// Updates the score text.
    /// </summary>
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + scoreCounter;
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
	/// Takes an input amount of points, multiplies it with the current combo multiplier
    /// and then adds it to the current score.
	/// </summary>
	/// <param name="points">Amount of points to add.</param>
	public void AddScore(int points)
    {
        int scoreMultiplier = Mathf.FloorToInt(Mathf.Max(comboCounter, 1));
        scoreCounter += points * scoreMultiplier;
		UpdateScoreText();
	}

	/// <summary>
	/// Increments the pellet counter by one.
	/// </summary>
	public void IncrementPelletCounter()
    {
		nrPelletsLeft++;
	}

	/// <summary>
	/// Decrements the pellet counter by one.
	/// </summary>
	public void DecrementPelletCounter()
    {
		nrPelletsLeft--;
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

    /// <summary>
    /// Decreases the combo counter based on time passed.
    /// </summary>
    /// <param name="dt">Amount of seconds passed.</param>
    private void UpdateComboCounter(float dt)
    {
        comboCounter = Mathf.Max(comboCounter - dt * comboDecayRate, 1);
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

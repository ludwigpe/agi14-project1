﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

/// <summary>
/// The GameController component is responsible for maintaining overall
/// gameplay functionality such as spawning AI and so forth.
/// </summary>
public class GameController : MonoBehaviour {
    
    // General
    public const int MAX_TIME = 90; // In seconds, max time that a game session is allowed to take
    private float inGameTimePassed;
    public bool DEBUGGING;
    private string currentLevel;   

    // Connection to player object
    public GameObject playerPrefab;
    private GameObject player;
    public Transform spawnPoint;

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
    private float comboCounter;     // Current combo value
    [Tooltip("At which rate that the combo should decay")]
    public float comboDecayRate = 0.85F;
    [Tooltip("How much combo should be increased per pellet consumed")]    
    public float comboIncrease = 0.75F;
    [Tooltip("How high of combo before player can release EMP")]   
    public int minEMPCombo = 3;
    [Tooltip("How many pellets need to be consumed before releasing EMP")]   
    public int minEMPPellets = 10;
    private int savedPellets = 0;

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

    // Log data list
    private List<string> logStrings;
    private StringBuilder sb;

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
        audio.Stop();   // Stop the menu music
        GetComponent<AudioListener>().enabled = false;

        player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity) as GameObject;
        player.GetComponent<ShakeWiiControls>().gc = this;
        if (DEBUGGING)
        {
            //player.GetComponent<ShakeWiiControls>().enabled = false;
        }
        Camera.main.GetComponent<SmoothFollow>().target = player.transform;
        Camera.main.rect = new Rect(0.0F, 0.0F, 0.5F, 1.0F);
        // create the logs directory if id does not already exist
        if (!Directory.Exists("Logs"))
            Directory.CreateDirectory("Logs");
        sb.AppendLine("forward.x\tforward.z\tvelocity.x\tvelocity.y\tvelocity.z\tposition.x\tposition.y\tposition.z\tscore\tcombo\tsecondsPassed");
        sb.Append(Application.loadedLevelName);
        InvokeRepeating("LogPlayerData", 1.0f, 1.0f);
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
    /// Update is called once per frame
	/// </summary>
	void Update ()
    {
        // START
        if(Input.GetKeyDown(KeyCode.F2))
        {
            Application.LoadLevel("TronLevel");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Application.LoadLevel("IceLevel"); 
        }
        // MUSIC 
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ToggleMusic();
        }
        // Shut down game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (!gameIsOver && gameStarted)
        {
            inGameTimePassed += Time.deltaTime;
            UpdateComboCounter(Time.deltaTime);
            CheckVictoryConditions();

            if (gameWon)
            {
                WriteLogToFile();
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
                WriteLogToFile();
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
    /// Mutes/Unmutes the music.
    /// </summary>
    private void ToggleMusic()
    {
        audio.mute = !audio.mute;
        int val = (audio.mute) ? 1 : 0;
        PlayerPrefs.SetInt("MuteMusic", val);
        PlayerPrefs.Save();
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
            gameWon = true;
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

	/// <summary>
	/// Adds points to the score.
	/// </summary>
	/// <param name="points">Amount of points to add.</param>
	public void AddScore(int points)
    {
        int scoreMultiplier = Mathf.FloorToInt(Mathf.Max(comboCounter, 1));
        scoreCounter += points * scoreMultiplier;
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
        savedPellets++;
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
        logStrings = new List<string>();
        sb = new StringBuilder();
        if( PlayerPrefs.HasKey("MuteMusic") && PlayerPrefs.GetInt("MuteMusic") == 1)
        {
            ToggleMusic();
        }

        GetComponent<AudioListener>().enabled = true;
        audio.Play();   // Start the menu music
    }
    /// <summary>
    /// Appends the player log data to the List with all logged data.
    /// </summary>
    /// <param name="forward">Forward Vector of player.</param>
    /// <param name="moveDir">Move direction of player.</param>
    /// <param name="position">Position of player.</param>
    /// <param name="score">Current player score.</param>
    /// <param name="combo">Current player combo.</param>
    public void LogPlayerData()
    {
        Vector3 forward = player.transform.forward;
        Vector3 moveDir = player.GetComponent<CharacterController>().velocity;
        Vector3 position = player.transform.position;
        int combo = Mathf.FloorToInt(ComboCounter);
        // new line
        sb.Append("\n");

        sb.Append(forward.x);
        sb.Append("\t");
        sb.Append(forward.z);

        sb.Append("\t");
        sb.Append(moveDir.x);
        sb.Append("\t");
        sb.Append(moveDir.y);
        sb.Append("\t");
        sb.Append(moveDir.z);

        sb.Append("\t");
        sb.Append(position.x);
        sb.Append("\t");
        sb.Append(position.y);
        sb.Append("\t");
        sb.Append(position.z);

        sb.Append("\t");
        sb.Append(Score);

        sb.Append("\t");
        sb.Append(combo);

        sb.Append("\t");
        sb.Append(inGameTimePassed);
    }

    /// <summary>
    /// Cancels the repeating logging and writes all logged data to file.
    /// </summary>
    private void WriteLogToFile()
    {
        CancelInvoke("LogPlayerData");
        string path ="Logs/" +  DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
        System.IO.StreamWriter file = new System.IO.StreamWriter(path);
        file.Write(sb.ToString());
        file.Close();
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

    public float ComboCounter
    {
        get
        {
            return comboCounter;
        }
		set
		{
			comboCounter = value;
		}
    }

    public GameObject PacMan
    {
        get
        {
            return player;
        }
    }

    public bool IsEMPReady
    {
        get
        {
            return (ComboCounter >= minEMPCombo || savedPellets >= minEMPPellets);
        }
    }

    public int SavedPellets
    {

        get
        {
            return savedPellets;
        }
        set
        {
            savedPellets = value;
        }
    }
    #endregion
}

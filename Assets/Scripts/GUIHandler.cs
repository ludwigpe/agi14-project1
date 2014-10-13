﻿using UnityEngine;
using System.Collections;

/// <summary>
/// GUI handler should handle all GUI rendered. 
/// </summary>
public class GUIHandler : MonoBehaviour 
{
    public WiiControllerHandler wiiHandler;

    public GUIStyle endTextStyle;
    public GUIStyle scoreStyle;

    public string winText;
    public string failText;

    private GameController gameController;
    private HighScore scoreGUI;
    private Rect p1Rect;
    private Rect p2Rect;

	// Use this for initialization
	void Start () {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
            scoreGUI = gameControllerObject.GetComponent<HighScore>();
        }
        else
        {
            Debug.Log("Cannot find 'GameController' script");
        }
        p1Rect = new Rect(0, 0, Screen.width / 2, Screen.height);
        p2Rect = new Rect(Screen.width/2, 0, Screen.width / 2, Screen.height);
	}
	
    /// <summary>
    /// GUI related update. Calls different GUI-scripts and gives them a container to draw inside.
    /// </summary>
    void OnGUI()
    {
        wiiHandler.DrawConnectionGuide(p1Rect, 0);
        wiiHandler.DrawConnectionGuide(p2Rect, 1);

        if (gameController.GameIsOver)
        {
            wiiHandler.DrawBatteryIndicator(p1Rect, 0);
            wiiHandler.DrawBatteryIndicator(p2Rect, 1);

            if (gameController.GameLost)
            {
                // the game is lost. Draw failText to both viewports in red.
                endTextStyle.normal.textColor = Color.red;
                DrawTextCenter(p1Rect, failText, endTextStyle);
                DrawTextCenter(p2Rect, failText, endTextStyle);

                // the player has won or lost the game. Present the highscore list
                scoreGUI.RenderHighScoreList(p1Rect);
                scoreGUI.RenderHighScoreList(p2Rect);
            } else if (gameController.GameWon)
            {
                // the game is win. Draw winText to both viewports in yellow.
                endTextStyle.normal.textColor = Color.yellow;
                DrawTextCenter(p1Rect, winText, endTextStyle);
                DrawTextCenter(p2Rect, winText, endTextStyle);

                // the player has won or lost the game. Present the highscore list
                scoreGUI.RenderHighScoreList(p1Rect);
                scoreGUI.RenderHighScoreList(p2Rect);
            }

        } else
        {
            // game is in session so render the score!
            scoreStyle.normal.textColor = Color.white;
            string timeLeft = gameController.SecondsLeft.ToString();
            string score = gameController.Score.ToString();
            DrawScoreAndTime(p1Rect, timeLeft, score, scoreStyle);
            DrawScoreAndTime(p2Rect, timeLeft, score, scoreStyle);
        }

        if (gameController.DEBUGGING)
        {
            wiiHandler.DrawDebugInfo(p1Rect, 0);
            wiiHandler.DrawDebugInfo(p2Rect, 1);
        }
    }

    /// <summary>
    /// Draws the text in the center of the container Rect.
    /// </summary>
    /// <param name="container">Container.</param>
    /// <param name="text">Text.</param>
    /// <param name="style">Style.</param>
    void DrawTextCenter(Rect container, string text, GUIStyle style)
    {
        GUILayout.BeginArea(container);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(text, style );
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws the score and time in the lower right corner of the container Rect.
    /// </summary>
    /// <param name="container">Container.</param>
    /// <param name="timeLeft">Time left.</param>
    /// <param name="score">Score.</param>
    /// <param name="style">Style.</param>
    void DrawScoreAndTime(Rect container, string timeLeft, string score, GUIStyle style)
    {
        GUILayout.BeginArea(container);
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.Label("Time left: " + timeLeft, scoreStyle);
        GUILayout.Label("Score: " + score, scoreStyle);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}

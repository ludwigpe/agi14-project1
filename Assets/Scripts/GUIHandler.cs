using UnityEngine;
using System.Collections;

/// <summary>
/// GUI handler should handle all GUI rendered. 
/// </summary>
public class GUIHandler : MonoBehaviour 
{
    public HighScore scoreGUI;
    public WiiControllerHandler wiiHandler;

    public GUIStyle endTextStyle;
    public GUIStyle scoreStyle;

    public string winText;
    public string failText;

    private GameController gameController;
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
        if (!gameController.PlayersConnected)
        {
            GUILayout.BeginArea(p1Rect);
            GUILayout.Label("Press 1 and 2 to start wiimote");
            GUILayout.EndArea();

            GUILayout.BeginArea(p2Rect);
            GUILayout.Label("Press 1 and 2 to start wiimote");
            GUILayout.EndArea();
        }
        if (gameController.GameLost || gameController.GameWon)
        {
            if (gameController.GameLost)
            {
                // the game is lost. Draw failText to both viewports in red.
                endTextStyle.normal.textColor = Color.red;
                DrawTextCenter(p1Rect, failText, endTextStyle);
                DrawTextCenter(p2Rect, failText, endTextStyle);
            } else if (gameController.GameWon)
            {
                // the game is win. Draw winText to both viewports in yellow.
                endTextStyle.normal.textColor = Color.yellow;
                DrawTextCenter(p1Rect, winText, endTextStyle);
                DrawTextCenter(p2Rect, winText, endTextStyle);
            }
            // the player has won or lost the game. Present the highscore list
            scoreGUI.RenderHighScoreList(p1Rect);
            scoreGUI.RenderHighScoreList(p2Rect);
        } else
        {
            // game is in session so render the score!
            scoreStyle.normal.textColor = Color.white;
            string timeLeft = gameController.SecondsLeft.ToString();
            string score = gameController.Score.ToString();
            DrawScoreAndTime(p1Rect, timeLeft, score, scoreStyle);
            DrawScoreAndTime(p2Rect, timeLeft, score, scoreStyle);
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

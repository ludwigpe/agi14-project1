using UnityEngine;
using System.Collections;

/// <summary>
/// GUI handler should handle all GUI rendered. 
/// </summary>
public class GUIHandler : MonoBehaviour 
{
	// Texts
	public GUIStyle endTextStyle;
	public GUIStyle endScoreTextStyle;
    public GUIStyle scoreStyle;
	public GUIStyle comboStyle;
    public GUIStyle empStyle;
	public string winText;
	public string failText;
	public string endScoreText;

    // Connections
    public WiiControllerHandler wiiHandler;
    private GameController gameController;
    private HighScore scoreGUI;
    
	// Containers
	private Rect p1Rect;
	private Rect p2Rect;

    // Combo colors
    private Color minComboColor = Color.green;  // Color of lowest combo
    private Color maxComboColor = Color.red;    // Color of highest combo
    private float useMaxComboColorValue = 5F;   // Value at which the maxComboColor is used

	/// <summary>
    /// Use this for initialization
	/// </summary>
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
		endScoreTextStyle.padding.top = endTextStyle.padding.top + endTextStyle.fontSize + endTextStyle.padding.bottom;
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
				
				// Present the players' score
				endScoreTextStyle.normal.textColor = Color.white;
				DrawTextCenter(p1Rect, endScoreText+gameController.Score, endScoreTextStyle);
				DrawTextCenter(p2Rect, endScoreText+gameController.Score, endScoreTextStyle);

                // the player has won or lost the game. Present the highscore list
                scoreGUI.RenderHighScoreList(p1Rect);
                scoreGUI.RenderHighScoreList(p2Rect);
            } 
            else if (gameController.GameWon)
            {
                // the game is win. Draw winText to both viewports in yellow.
                endTextStyle.normal.textColor = Color.yellow;
                DrawTextCenter(p1Rect, winText, endTextStyle);
                DrawTextCenter(p2Rect, winText, endTextStyle);
				
				// Present the players' score
				endScoreTextStyle.normal.textColor = Color.white;
				DrawTextCenter(p1Rect, endScoreText+gameController.Score, endScoreTextStyle);
				DrawTextCenter(p2Rect, endScoreText+gameController.Score, endScoreTextStyle);

				// Present the players' score
				endScoreTextStyle.normal.textColor = Color.white;
				DrawTextCenter(p1Rect, endScoreText+gameController.Score, endScoreTextStyle);
				DrawTextCenter(p2Rect, endScoreText+gameController.Score, endScoreTextStyle);

                // the player has won or lost the game. Present the highscore list
                scoreGUI.RenderHighScoreList(p1Rect);
                scoreGUI.RenderHighScoreList(p2Rect);
            }
        } 
        else
        {
            // game is in session so render the score!
            scoreStyle.normal.textColor = Color.white;
            string timeLeft = gameController.SecondsLeft.ToString();
            string score = gameController.Score.ToString();
            DrawScoreAndTime(p1Rect, timeLeft, score, scoreStyle);
            DrawScoreAndTime(p2Rect, timeLeft, score, scoreStyle);
            DrawComboCounter(p1Rect);
            DrawComboCounter(p2Rect);
            DrawEMPStatus(p1Rect);
            DrawEMPStatus(p2Rect);
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
        GUILayout.Label("TIME LEFT: " + timeLeft, scoreStyle);
        GUILayout.Label("SCORE: " + score, scoreStyle);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws the current score combo.
    /// </summary>
    /// <param name="container">Containing rectangle.</param>
    void DrawComboCounter(Rect container)
    {
        float combo = gameController.ComboCounter;
        GUILayout.BeginArea(container);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        float colorScale = Mathf.Clamp(combo / useMaxComboColorValue, 0, 1);
        comboStyle.normal.textColor = Color.Lerp(minComboColor, maxComboColor, colorScale);
        GUILayout.Label("COMBO: " + Mathf.FloorToInt(combo) + "X", comboStyle);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    /// <summary>
    /// Draw the status of the EMP. 
    /// </summary>
    /// <param name="container">Containing rectangle for text.</param>
    void DrawEMPStatus(Rect container)
    {
        GUILayout.BeginArea(container);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
       
        if (gameController.IsEMPReady)
        {
            empStyle.normal.textColor = minComboColor;
            GUILayout.Label("EMP Ready",  empStyle);
        }
        else
        {
            empStyle.normal.textColor = maxComboColor;
            GUILayout.Label("EMP not ready", empStyle);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}

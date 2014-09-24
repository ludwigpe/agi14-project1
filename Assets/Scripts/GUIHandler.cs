using UnityEngine;
using System.Collections;

public class GUIHandler : MonoBehaviour {


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
//            wiiHandler.drawWiimoteGUI(p1Rect);
//            wiiHandler.drawWiimoteGUI(p2Rect);
        }
        if (gameController.GameLost || gameController.GameWon)
        {
            if (gameController.GameLost)
            {
                endTextStyle.normal.textColor = Color.red;
                drawTextCenter(p1Rect, failText, endTextStyle);
                drawTextCenter(p2Rect, failText, endTextStyle);
            } else if (gameController.GameWon)
            {
                endTextStyle.normal.textColor = Color.yellow;
                drawTextCenter(p1Rect, winText, endTextStyle);
                drawTextCenter(p2Rect, winText, endTextStyle);
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
            drawScoreAndTime(p1Rect, timeLeft, score, scoreStyle);
            drawScoreAndTime(p2Rect, timeLeft, score, scoreStyle);

        }
    }

	// Update is called once per frame
	void Update () {
	
	}

    void drawTextCenter(Rect container, string text, GUIStyle style)
    {
        GUILayout.BeginArea(container);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(text, style );
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void drawScoreAndTime(Rect container, string timeLeft, string score, GUIStyle style)
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

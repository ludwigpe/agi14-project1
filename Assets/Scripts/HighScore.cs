using UnityEngine;
using System.Collections;

/// <summary>
/// The HighScore component takes care of everything related to the high score table.
/// </summary>
public class HighScore : MonoBehaviour
{
    // GUI Styles
    public GUIStyle scoreTableStyle;
    public GUIStyle scoreIndexStyle;
    public GUIStyle scorePointStyle;
    public GUIStyle scoreEntryStyle;

    // Colors
    public Color flashTextColor1 = Color.red;
    public Color flashTextColor2 = Color.yellow;
    public Color defaultTextColor = Color.white;

    private float flashColorScale = 0f;    // Current progress of color transition
    private float flashColorSign = 1f;     // Is the flash going to 2:nd color or back to 1:st?

    // Link to GameController
    private GameController gameController;

    // High Score logic
    private bool checkScore = true;
    private bool inputName = false;
    private int takenEntryIndex;
    private int maxScoreNameLength = 20;    // Max characters that score name entry can consist of

    // Use this for initialization
    void Start()
    {

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (inputName)
        {
            EnterYourName(takenEntryIndex);
        }
    }

    /// <summary>
    /// Queues the Player to enter his/her name. Input will end when enter is pressed.
    /// </summary>
    /// <param name="index">HighScore entry index.</param>
    public void EnterYourName(int index)
    {
        string name = PlayerPrefs.GetString(index + "HScoreName");
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (name.Length != 0)
                {
                    name = name.Substring(0, name.Length - 1);
                }
            }
            else
            {
                if (c == '\n' || c == '\r')
                {
                    inputName = false;
                    PlayerPrefs.Save();
                }
                else if (name.Length + 1 <= maxScoreNameLength)
                {
                    name += c;
                }
            }
        }
        PlayerPrefs.SetString(index + "HScoreName", name);
    }

    /// <summary>
    /// Adds an entry to the High Score table.
    /// </summary>
    /// <param name="score">Score of entry.</param>
    public int GetScoreEntryIndex(int score)
    {
        int newScore = score;
        string newName = "[ENTER YOUR NAME]";
        int oldScore;
        string oldName;

        bool scoreFoundEntry = false;
        int entryIndex = -1;

        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey(i + "HScore"))
            {
                if (PlayerPrefs.GetInt(i + "HScore") < newScore || scoreFoundEntry && PlayerPrefs.GetInt(i + "HScore") == newScore)
                {
                    // New score is higher than the stored score - move old entry one step down
                    oldScore = PlayerPrefs.GetInt(i + "HScore");
                    oldName = PlayerPrefs.GetString(i + "HScoreName");
                    PlayerPrefs.SetInt(i + "HScore", newScore);
                    PlayerPrefs.SetString(i + "HScoreName", newName);
                    newScore = oldScore;
                    newName = oldName;

                    if (!scoreFoundEntry)
                    {
                        entryIndex = i;
                        scoreFoundEntry = true;
                    }
                }
            }
            else
            {
                PlayerPrefs.SetInt(i + "HScore", newScore);
                PlayerPrefs.SetString(i + "HScoreName", newName);
                newScore = 0;
                newName = "";

                if (!scoreFoundEntry)
                {
                    entryIndex = i;
                    scoreFoundEntry = true;
                }
            }
        }
        return entryIndex;
    }

   public void RenderHighScoreList(Rect container)
    {
        float width = Mathf.Min(500, container.width);
        float height = Mathf.Min(450, container.height);
        float left = ((container.width - width) / 2) + container.x;
        float top = (container.height - height) / 2;
        Rect highScoreRect = new Rect(left, top, width, height);
        // Did we make the high score?
        if (checkScore)
        {
            int newScore = gameController.Score;
            takenEntryIndex = GetScoreEntryIndex(newScore);
            if (takenEntryIndex != -1)
            {
                inputName = true;
                checkScore = false;
            }
        }
        
        float entryHeight = 35;
        GUILayout.BeginArea(highScoreRect, "High Score", scoreTableStyle);
        for (int i = 0; i < 10; i++)
        {
            
            // Mark selected entry with color
            if (inputName && i == takenEntryIndex)
            {
                flashColorScale += Time.deltaTime * flashColorSign;
                if (flashColorScale >= 1 || flashColorScale <= 0)
                {
                    flashColorSign *= -1;
                }
                GUI.contentColor = Color.Lerp(flashTextColor1, flashTextColor2, flashColorScale);
            }
            else
            {
                GUI.contentColor = defaultTextColor;
            }
            
            string entryName = PlayerPrefs.GetString(i + "HScoreName");
            int entryScore = PlayerPrefs.GetInt(i + "HScore");

            GUILayout.BeginHorizontal();

            GUILayout.Label( (i+1)+ ": ", scoreIndexStyle, GUILayout.Width(10.0F), GUILayout.Height(entryHeight));
            GUILayout.Label( entryName, scoreEntryStyle, GUILayout.Width(highScoreRect.width*0.6F), GUILayout.Height(entryHeight));
            GUILayout.Label( entryScore.ToString(), scorePointStyle, GUILayout.Height(entryHeight));
           
            GUILayout.EndHorizontal();

        }
        GUILayout.EndArea();
    }
}

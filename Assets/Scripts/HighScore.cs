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
    private bool firstCharacterEnter = true; // Is the character entered the first character to be entered?
    private int takenEntryIndex;
    private int maxScoreNameLength = 20;    // Max characters that score name entry can consist of
    private string levelName;

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
        levelName = Application.loadedLevelName;
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
        string currentNameKey = levelName + "HScoreName" + index;

        string name = PlayerPrefs.GetString(currentNameKey);
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

                    if (firstCharacterEnter)
                    {
                        name = c.ToString();
                        firstCharacterEnter = false;
                    }
                    else
                    {
                        name += c;
                    }
                }
            }
        }
        PlayerPrefs.SetString(currentNameKey, name);
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
            string currentScoreKey = levelName + "HScore" + i;
            string currentNameKey = levelName + "HScoreName" + i;

            if (PlayerPrefs.HasKey(currentScoreKey))
            {
                if (PlayerPrefs.GetInt(currentScoreKey) < newScore || scoreFoundEntry && PlayerPrefs.GetInt(currentScoreKey) == newScore)
                {
                    // New score is higher than the stored score - move old entry one step down
                    oldScore = PlayerPrefs.GetInt(currentScoreKey);
                    oldName = PlayerPrefs.GetString(currentNameKey);
                    PlayerPrefs.SetInt(currentScoreKey, newScore);
                    PlayerPrefs.SetString(currentNameKey, newName);
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
                PlayerPrefs.SetInt(currentScoreKey, newScore);
                PlayerPrefs.SetString(currentNameKey, newName);
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

   /// <summary>
   /// Renders the high score list to the input container.
   /// </summary>
   /// <param name="container">Contaning rectangle to render to.</param>
   public void RenderHighScoreList(Rect container)
   {
        float width = Mathf.Min(500, container.width);
        float height = Mathf.Min(480, container.height);
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

            string currentScoreKey = levelName + "HScore" + i;
            string currentNameKey = levelName + "HScoreName" + i;
            string entryName = PlayerPrefs.GetString(currentNameKey);
            int entryScore = PlayerPrefs.GetInt(currentScoreKey);

            GUILayout.BeginHorizontal();

            GUILayout.Label( (i+1)+ ": ", scoreIndexStyle, GUILayout.Width(10.0F), GUILayout.Height(entryHeight));
            GUILayout.Label( entryName, scoreEntryStyle, GUILayout.Width(highScoreRect.width*0.6F), GUILayout.Height(entryHeight));
            GUILayout.Label( entryScore.ToString(), scorePointStyle, GUILayout.Height(entryHeight));

            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }
}

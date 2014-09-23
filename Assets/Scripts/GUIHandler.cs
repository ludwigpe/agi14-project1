using UnityEngine;
using System.Collections;

public class GUIHandler : MonoBehaviour {

    private GameController gameController;
    private Rect p1Rect;
    private Rect p2Rect;
	// Use this for initialization
	void Start () {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
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

    }

	// Update is called once per frame
	void Update () {
	
	}
}

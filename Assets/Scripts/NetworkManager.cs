using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

    bool DEBUGGING = false;

	// Use this for initialization
    private const string typeName = "MegatronsSuperAwesomeGame"; // this is the name of the game
    private const string gameName = "theBestRoom"; // this is the name of the game room within the game
	void Start () 
    {
        if(!DEBUGGING)
        {
            bool useNat = !Network.HavePublicAddress();
            Network.InitializeServer(4, 1337, useNat);
        }
        
	}
    void OnGUI()
    {
 
        GUI.Box(new Rect(10.0f, 10.0f, 100.0f, 90.0f), "Loader Menu");

        if (GUI.Button(new Rect(20, 40, 80, 20), "Level 1"))
        {
            Application.LoadLevel("scene1");
        }

    }
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnServerInitialized()
    {
        Debug.Log("Server initialized and ready");
    }
}

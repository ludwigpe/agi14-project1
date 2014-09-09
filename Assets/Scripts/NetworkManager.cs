using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerType { 
    PLAYER,
    HELPER,
    ENEMY
};

public class NetworkManager : MonoBehaviour {

    private int playerCount = 0;
    private int maxMessages = 200;
    private Queue messages;
    private const string typeName = "MegatronsSuperAwesomeGame"; // this is the name of the game
    private const string gameName = "theBestRoom"; // this is the name of the game room within the game

    private NetworkPlayer mainPlayer;
    private List<NetworkPlayer> helpers;
    private List<NetworkPlayer> enemies;

    public Vector2 scrollPos;
    
	void Start () 
    {
        // initialize lists
        helpers = new List<NetworkPlayer>();
        enemies = new List<NetworkPlayer>();
        messages = new Queue(maxMessages);

        // initialize the server
        bool useNat = !Network.HavePublicAddress();
        Network.InitializeServer(4, 1337, useNat);
	}

    // The OnGUI method is called every frame and re-rendered.
    void OnGUI()
    {
        if (Network.isServer)
        {
            // Create a scroll view and show all the messages on the server.
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
            foreach (string m in messages)
            {
                GUILayout.Label(m);
            }
            GUILayout.EndScrollView();
        }


    }
    
    void OnServerInitialized()
    {
        AddMessage("Server initialized registering with master server...");
        MasterServer.RegisterHost(typeName, gameName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        switch (msEvent)
        { 
            case MasterServerEvent.RegistrationSucceeded:
                AddMessage("Server registered successfully");
                break;

            case MasterServerEvent.RegistrationFailedGameType:
                AddMessage("Failed to register gametype: " + typeName);
                break;

            case MasterServerEvent.RegistrationFailedGameName:
                AddMessage("Failed to register Game name: " + gameName);
                break;
        }
   } 

    void OnPlayerConnected(NetworkPlayer player)
    {
        AddMessage("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        AddMessage("Clean up after player " + player);
     
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
        playerCount--;
        AddMessage("Player " + player.ipAddress + " has disconnected. (players left: " + playerCount + ")" );

        if (player == mainPlayer)
            // we should probably end the game here 
            DisconnectGame();
        else if (helpers.Contains(player))
            helpers.Remove(player);
        else if (enemies.Contains(player))
            enemies.Remove(player);

    }

    private void DisconnectGame()
    {
        Network.Disconnect();
        playerCount = 0;

        // these might not be needed if OnPlayerDisconnected is fired
        helpers.Clear();
        enemies.Clear();
    }

    [RPC]
    void GetPlayerType(NetworkPlayer player, string deviceType) {
        PlayerType type = PlayerType.PLAYER;
        switch (deviceType) { 
            case "Handheld":
                type = (helpers.Count > enemies.Count)? PlayerType.ENEMY : PlayerType.HELPER;
                if (helpers.Count > enemies.Count)
                {
                    type = PlayerType.ENEMY;
                    enemies.Add(player);
                }
                else {
                    type = PlayerType.HELPER;
                    helpers.Add(player);
                }
                break;
            default:
                type = PlayerType.PLAYER;
                mainPlayer = player;
                break;
        }
        AddMessage("Player " + player.ipAddress + " with devicetype: " + deviceType + " is a " + type.ToString());

        networkView.RPC("SetPlayerType", player, type.ToString());
    }

    [RPC]
    void SetPlayerType(string type)
    {
        return;
    }


    void AddMessage(string message) {
        if (messages.Count >= maxMessages) messages.Dequeue();
        messages.Enqueue(message);
    }


}

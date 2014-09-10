using UnityEngine;
using System.Collections;

public class ClientScript : MonoBehaviour {
    
    private const string typeName = "MegatronsSuperAwesomeGame"; // this is the name of the game
    public GameObject playerPrefab;
    public Transform playerSpawnPoints;
    private HostData[] hostList;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI() {
        if (Network.isClient) return;
        
        int cWidth = 200;
        GUILayout.BeginArea(new Rect(Screen.width / 2 - cWidth / 2, 200, 200, 100));
        GUILayout.BeginVertical("box");

        if (hostList != null)
        {
            foreach (HostData host in hostList) {
                if (GUILayout.Button(host.gameName))
                    JoinServer(host);
            }
        }
        if(GUILayout.Button("Refresh"))
            RefreshHostList();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void JoinServer(HostData host) {
        Network.Connect(host);
    }
    private void RefreshHostList() {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    // This function is called when the client successfully has connected to the server
    void OnConnectedToServer() {

        Debug.Log("Connected to server");
        networkView.RPC("GetPlayerType", RPCMode.Server, Network.player, SystemInfo.deviceType.ToString());
        
    }

    void OnDisconnectedFromServer(NetworkDisconnection info) {
        Debug.Log("Disconnected from Server. Scene will restart");
        Application.LoadLevel(Application.loadedLevel);
    }

    [RPC]
    void SetPlayerType(string type) {
        switch (type) { 
            case "PLAYER":
                InitializePlayer();
                break;
            case "HELPER":
                InitializeHelper();
                break;
            case "ENEMY":
                InitializeEnemy();
                break;
        }
    }
    [RPC]
    void GetPlayerType(NetworkPlayer player, string deviceType)
    {
        return;
    }
    private void InitializeEnemy()
    {
        throw new System.NotImplementedException();
    }

    private void InitializeHelper()
    {
		Debug.Log ("hejsan");
    }

    private void InitializePlayer()
    {
        Network.Instantiate(playerPrefab, playerSpawnPoints.position, Quaternion.identity, 0);
    }

}

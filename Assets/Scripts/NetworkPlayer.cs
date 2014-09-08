using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnNetworkLoadedLevel(){
		Debug.Log ("The level has been loaded, I need to initialize myself")
	}
}

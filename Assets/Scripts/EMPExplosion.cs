using UnityEngine;
using System.Collections;

public class EMPExplosion : MonoBehaviour {

    // Update is called once per frame
	void Update () {
        transform.Rotate(Time.smoothDeltaTime * 200, 0, 0);
        transform.localScale += Mathf.Sin(Time.time) * new Vector3(50* Time.smoothDeltaTime, 50*Time.smoothDeltaTime, 50*Time.smoothDeltaTime);
	}

}

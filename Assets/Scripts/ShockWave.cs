using UnityEngine;
using System.Collections;

public class ShockWave : MonoBehaviour {

    float radius = 0.0F;
    public float maxRadius = 100.0F;
    public float animationSpeed = 10.0F;
    bool updateRadius = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F4))
        { 
            updateRadius = true;
            radius = 0.0F;
        }
        if(updateRadius)
        {
            radius += Time.deltaTime * animationSpeed;
            renderer.material.SetFloat("_Radius", radius);
            if (radius > maxRadius)
                updateRadius = false;
        }
	}
}

using UnityEngine;
using System.Collections;

public class ShockWave : MonoBehaviour {

    float radius = 0.0F;
    public float maxRadius = 100.0F;
    public float animationSpeed = 10.0F;
    public float glow = 5.0F;
    public Shader regular;
    public Shader shockwave;
    bool updateRadius = false;
	// Use this for initialization
	void Start () {
        renderer.material.shader = regular;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F4))
        { 
          // Vector3 origin = GameObject.Find("Pacman").transform.position;
           Vector3 origin = Vector3.one;
           StartShockWave(origin);
        }
        if(updateRadius)
        {
            radius += Time.deltaTime * animationSpeed;
            Debug.Log(radius);
            //glow = Mathf.Lerp(glow, 1.0F, Time.deltaTime);
           // glow = glow / radius;
            renderer.material.SetFloat("_Radius", radius);
            renderer.material.SetFloat("_SelfIllu",  Mathf.Lerp(glow, 1.0F, Time.deltaTime));

            if (radius > maxRadius)
            {
                StopShockWave();

            }
                
        }
	}

    void StartShockWave(Vector3 origin)
    {
        renderer.material.shader = shockwave;
        Vector4 o = new Vector4(origin.x, origin.y, origin.z);
        o.w = 1.0F;
        renderer.material.SetVector("_Origin", o);
        updateRadius = true;
        radius = 0.0F;
    }

    void StopShockWave()
    {
        renderer.material.shader = regular;
        updateRadius = false;
    }
}

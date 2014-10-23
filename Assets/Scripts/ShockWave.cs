using UnityEngine;
using System.Collections;

/// <summary>
/// This script is responsible for starting and stopping the shockwave.
/// It updates the radius of the shockwave so the shader knows how far the blast
/// has gone. It will be triggered by pacman
/// </summary>
public class ShockWave : MonoBehaviour {

    float radius = 0.0F;
    public float maxRadius = 100.0F;
    public float animationSpeed = 10.0F;
    public float powerOffset = 40.0F;
    private ArrayList shockwaveMaterials = new ArrayList();
    bool updateRadius = false;
	// Use this for initialization
	void Start () {
        shockwaveMaterials.Add(GameObject.FindWithTag("Floor").renderer.material);
        foreach (Material m in GameObject.FindWithTag("Walls").renderer.materials)
        {
            shockwaveMaterials.Add(m);
        }
        foreach (Material m in shockwaveMaterials)
        {
            m.SetFloat("_Radius", maxRadius);
            m.SetFloat("_PowerOffset", powerOffset);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(updateRadius)
        {
            radius += Time.deltaTime * animationSpeed;
            //renderer.material.SetFloat("_Radius", radius);
            foreach (Material m in shockwaveMaterials)
            {
                m.SetFloat("_Radius", radius);
            }

            if (radius > maxRadius)
            {
                StopShockWave();
            }
        }
	}

    /// <summary>
    /// Starts the shockwave. It sets the shaders origin to let it know where 
    /// the shockwave originates from and initializes the radius to 15 so 
    /// pacman is not below the shockwave in the beginning.
    /// </summary>
    /// <param name="origin"></param>
    public void StartShockWave(Vector3 origin)
    {
        Vector4 o = new Vector4(origin.x, origin.y, origin.z);
        o.w = 1.0F;
        //renderer.material.SetVector("_Origin", o);
        //renderer.material.se
        foreach (Material m in shockwaveMaterials)
        {
            m.SetVector("_Origin", o);
        }

        updateRadius = true;
        radius = 15.0F;
    }
    /// <summary>
    /// Stop updating the shockwave radius.
    /// </summary>
    void StopShockWave()
    {
        updateRadius = false;
    }
}

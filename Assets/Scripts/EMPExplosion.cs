using UnityEngine;
using System.Collections;

/// <summary>
/// An EMP explosion is an effect that consists of three components:
/// * An (electricity) sphere
/// * A particle blast
/// * A shockwave effect (shader)
/// </summary>
public class EMPExplosion : MonoBehaviour {

    // EMP Parameters
    public float diameter;
    public float rotationSpeed;
    public float explosionSpeed;
    private float currScale = 0;
    private float startedAt;

    // Link to explosion spheres
    public Transform empSphere01;
    public Transform empSphere02;
    public Transform empEffect;

    /// <summary>
    /// Starts the EMP explosion.
    /// </summary>
    void Start()
    {
        particleSystem.Play();
        startedAt = Time.time;
    }
   
    /// <summary>
    /// Rotates the spheres and scales them according to current scale.
    /// Will also make the explosion spheres more transperent as the explosion progresses.
    /// </summary>
	void Update () 
    {
        empSphere01.Rotate(Time.smoothDeltaTime * rotationSpeed, 0, 0);
        empSphere02.Rotate(Time.smoothDeltaTime * rotationSpeed, 0, 0);

        float scale = currScale + Time.smoothDeltaTime * explosionSpeed;
        scale = Mathf.Clamp(scale, 0, diameter);

        float alphaScale = scale / diameter;
        alphaScale = Mathf.Cos(alphaScale * Mathf.PI / 2);

        Color tintColor = empSphere01.renderer.material.GetColor("_TintColor");
        tintColor.a = alphaScale;
        empSphere01.renderer.material.SetColor("_TintColor", tintColor);
        empSphere02.renderer.material.SetColor("_TintColor", tintColor);
        
        if (scale == diameter && particleSystem.isStopped)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Vector3 newScale = new Vector3(scale, scale, scale);
            empSphere01.localScale = newScale;
            empSphere02.localScale = newScale;
            empEffect.localScale = newScale;

            currScale = scale;
        }
    }

    #region Accessors
    public float StartedAt
    {
        get
        {
            return startedAt;
        }
    }
    #endregion
}

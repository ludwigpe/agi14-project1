using UnityEngine;
using System.Collections;

/// <summary>
/// Responsible for the Pellets' pulsating lighting.
/// </summary>
public class PelletLight : MonoBehaviour {
	
    // Pulsating light 
    private float theta = 0.0F;
	public float maxRange = 3.0F;
	public float minRange = 2.0F;
	private float rangeAmp;
	private float rangeOffset;
	public float glowSpeed = 2.0F;
	public float glowConstant = 2.0F;
	private Light lightObject;
	private float minIntensity;
    private bool isToggled = true;

    // Flicker
    private bool flicker = false;
    private float startFlickerTime;
    private float nextFlickerTime = 0;
    private float endFlickerTime = 0;
    private float totalFlickerLength = 1;
    private float minFlickerInterval = 0.01f;
    private float maxFlickerInterval = 0.3f;

    private Material turnedOnMaterial;
    public Material turnedOffMaterial;
	
    /// <summary>
    /// Use this for initialization
    /// </summary>
	void Start () 
    {
		lightObject = gameObject.GetComponent<Light>();
		minIntensity = lightObject.intensity;
		rangeAmp = (maxRange - minRange) / 2;
		rangeOffset = (maxRange + minRange) / 2;
        turnedOnMaterial = renderer.material;
	}

    /// <summary>
    /// Flickers the light.
    /// </summary>
    private void FlickerLight()
    {
        lightObject.enabled = !lightObject.enabled;
        if (lightObject.enabled)
        {
            renderer.material = turnedOnMaterial;
        }
        else
        {
            renderer.material = turnedOffMaterial;
        }
    }
    
	/// <summary>
    /// Update is called once per frame
	/// </summary>
	void Update () 
    {
        if (isToggled)
        {
            // Flicker with the light for a while to simulate it coming back on
            if (flicker)
            {
                if (Time.time > startFlickerTime)
                {
                    if (Time.time > endFlickerTime)
                    {
                        flicker = false;
                        lightObject.enabled = true;
                        renderer.material = turnedOnMaterial;
                    }
                    else if (Time.time > nextFlickerTime)
                    {
                        FlickerLight();
                        nextFlickerTime += Random.Range(minFlickerInterval, maxFlickerInterval);
                    }
                }
            }
            else
            {
                lightObject.intensity = (glowConstant * Mathf.Sin(theta)) + minIntensity + glowConstant;
                lightObject.range = (rangeAmp * Mathf.Sin(theta)) + rangeOffset;
                theta += (2 * Mathf.PI * Time.deltaTime) / glowSpeed;

                if (theta > (2 * Mathf.PI))
                    theta -= (2 * Mathf.PI);
            }
        }
	}

    /// <summary>
    /// Turns off the light of the pellet.
    /// </summary>
    public void TurnOffLight()
    {
        isToggled = false;
        lightObject.enabled = false;
        renderer.material = turnedOffMaterial;
        theta = 0;
    }

    /// <summary>
    /// Turns on the light of the pellet after some time.
    /// After being turned on the light will flicker for some seconds.
    /// </summary>
    /// <param name="time">Time at which light should be turned on.</param>
    public void TurnOnLightAtTime(float time)
    {
        isToggled = true;
        flicker = true;
        startFlickerTime = time;

        nextFlickerTime = startFlickerTime + Random.Range(minFlickerInterval, maxFlickerInterval);
        endFlickerTime = startFlickerTime + totalFlickerLength;
    }
}

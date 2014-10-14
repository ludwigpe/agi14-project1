using UnityEngine;
using System.Collections;

/// <summary>
/// Responsible for the Pellets' pulsating lighting.
/// </summary>
public class PelletLight : MonoBehaviour {
	private float theta = 0.0F;
	public float maxRange = 3.0F;
	public float minRange = 2.0F;
	private float rangeAmp;
	private float rangeOffset;
	public float glowSpeed = 2.0F;
	public float glowConstant = 2.0F;
	private Light lightObject;
	private float minIntensity;
	// Use this for initialization
	void Start () {
		lightObject = gameObject.GetComponent<Light> ();
		minIntensity = lightObject.intensity;
		rangeAmp = (maxRange - minRange) / 2;
		rangeOffset = (maxRange + minRange) / 2;
	}
	
	// Update is called once per frame
	void Update () {

		lightObject.intensity = (glowConstant*Mathf.Sin (theta)) + minIntensity + glowConstant;
		lightObject.range = (rangeAmp * Mathf.Sin (theta)) + rangeOffset;
		theta += (2 * Mathf.PI * Time.deltaTime)/glowSpeed;

		if (theta > (2 * Mathf.PI))
			theta -= (2 * Mathf.PI);
	}
}

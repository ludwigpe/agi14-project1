using UnityEngine;
using System.Collections;

public class ParticleMovement : MonoBehaviour {
	public float startLifeTime;
	public float fadeOutTime;
	public Vector3 speedVector;
	public Vector3 gravity;
	private float lifeTimeLeft;

	// Use this for initialization
	void Start () {
		lifeTimeLeft = startLifeTime;
	}
	
	/// <summary>
	/// Updates this particles current position and velocity as well as its remaining lifetime and
	/// whether it is still alive.
	/// </summary>
	public void Update() {
		if (lifeTimeLeft > 0) {
			float dTime = Time.deltaTime;
			lifeTimeLeft -= dTime;
			speedVector += gravity * dTime;
			transform.position += (speedVector * dTime);

			if (lifeTimeLeft < fadeOutTime) {
				Color c = renderer.material.color;
				renderer.material.color = new Color(c.r, c.g, c.b, lifeTimeLeft / fadeOutTime);
			}
		}
	}
}

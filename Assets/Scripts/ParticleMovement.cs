using UnityEngine;
using System.Collections;

public class ParticleMovement : MonoBehaviour {
	public float startLifeTime;
	public float fadeOutTime;
	public Vector3 speedVector;
	public Vector3 gravity;
	public bool isDead = false;
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
		if (!isDead) {
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
			else {
				Kill();
				isDead = true;
			}
		}
	}
	
	/// <summary>
	/// Determines whether this particle is dead.
	/// </summary>
	/// <returns><c>true</c> if the particle is dead; otherwise, <c>false</c>.</returns>
	public bool IsDead() {
		return isDead;
	}
	
	/// <summary>
	/// Remove this particle's representation in the world.
	/// </summary>
	public void Kill() {
		Destroy (gameObject);
	}
}

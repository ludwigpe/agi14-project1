using UnityEngine;
using System.Collections;

public class ParticleMovement : MonoBehaviour {
	public float lifeTime;
	public Vector3 speedVector;
	public Vector3 gravity;
	public bool isDead = false;

	// Use this for initialization
	void Start () {
	
	}
	
	/// <summary>
	/// Updates this particles current position and velocity as well as its remaining lifetime and
	/// whether it is still alive.
	/// </summary>
	public void Update() {
		if (!isDead) {
			if (lifeTime > 0) {
				float dTime = Time.deltaTime;
				lifeTime -= dTime;
				speedVector += gravity * dTime;
				transform.position += (speedVector * dTime);
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

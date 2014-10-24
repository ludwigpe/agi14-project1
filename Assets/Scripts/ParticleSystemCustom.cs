using UnityEngine;
using System.Collections;

/// <summary>
/// Particle system. This component makes its GameObject act as an emitter for particles
/// </summary>
public class ParticleSystemCustom : MonoBehaviour 
{
	public GameObject particlePrefab1;
	public GameObject particlePrefab2;
	public bool isContinuous = true;
	public int particleAmount;
	public Vector3 emitterDirection;
	public Vector3 gravity = new Vector3 (0, -10, 0);
	public float minAngle = 0;
	public float maxAngle = 180;
	public float minSpeed = 3;
	public float maxSpeed = 3;
	public Material particleMaterial;
	public float particleLifeTime;
	public float particleFadeOutTime;

	private float timeSinceLastSpawn = 0;

	/// <summary>
	/// Create a burst of particles and removes the particle system if the variable isContinuous is false; otherwise, do nothing.
	/// </summary>
	void Start () 
	{
		Transform targetFP = GameObject.Find("Pacman").transform;
		Transform targetTP = Camera.main.transform;
		if (!isContinuous) {
			for (int i = 0; i < particleAmount; i++) {
				// Calculate the particle's original vector for start speed
				Vector3 tmpvector = Vector3.Cross(Random.onUnitSphere, emitterDirection);
				Quaternion rotation = Quaternion.AngleAxis(Random.Range(minAngle, maxAngle), tmpvector);
				tmpvector = rotation * emitterDirection;
				tmpvector.Normalize();
				Vector3 startVector = Random.Range(minSpeed, maxSpeed) * tmpvector;
				CreateParticle (particlePrefab1, startVector, targetFP); // Camera.current.transform <-- first person camera
				CreateParticle (particlePrefab2, startVector, targetTP);
			}
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Spawns a particle
	/// </summary>
	/// <param name="particlePrefab">the particle prefab to spawn.</param>
	/// <param name="startVector">Start vector for the particle's original velocity.</param>
	/// <param name="target">The target that the particle should be facing.</param>
	private void CreateParticle(GameObject particlePrefab, Vector3 startVector, Transform target) 
	{
		GameObject particle = (GameObject)Instantiate (particlePrefab, transform.position, transform.rotation);
		if(particleMaterial != null) particle.renderer.material = particleMaterial;

		ParticleLookAtTarget look = particle.GetComponent<ParticleLookAtTarget>();
		look.target = target;

		ParticleMovement particleMovement = particle.GetComponent<ParticleMovement> ();
		particleMovement.speedVector = startVector;
		particleMovement.startLifeTime = particleLifeTime;
		particleMovement.fadeOutTime = particleFadeOutTime;
		particleMovement.gravity = gravity;
		Destroy (particle, particleLifeTime);
	}

	/// <summary>
	/// If the particle system is continuous (isContinuous == true): spawn particles with frequency depening on
	/// the variable particleAmount. do nothing if the particle system is not continuous.
	/// </summary>
	void Update () 
	{
		timeSinceLastSpawn += Time.deltaTime;
		if (particleAmount > 0 && timeSinceLastSpawn > 1 / particleAmount) {
			timeSinceLastSpawn = 0;
			Transform targetFP = GameObject.Find ("Pacman").transform;
			Transform targetTP = Camera.main.transform;

			// Calculate the particle's original vector for start speed
			Vector3 tmpvector = Vector3.Cross (Random.onUnitSphere, emitterDirection);
			Quaternion rotation = Quaternion.AngleAxis (Random.Range (minAngle, maxAngle), tmpvector);
			tmpvector = rotation * emitterDirection;
			tmpvector.Normalize ();
			Vector3 startVector = Random.Range (minSpeed, maxSpeed) * tmpvector;
			CreateParticle (particlePrefab1, startVector, targetFP); // Camera.current.transform <-- first person camera
			CreateParticle (particlePrefab2, startVector, targetTP);
		}
	}


	/// <summary>
	/// Kill this particle system.
	/// </summary>
	void Kill ()
	{
		Destroy (gameObject);
	}
}

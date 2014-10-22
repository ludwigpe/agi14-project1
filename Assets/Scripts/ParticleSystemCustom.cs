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

	void Start () 
	{
		if (!isContinuous) {
			for (int i = 0; i < particleAmount; i++) {
				// Calculate the particle's original vector for start speed
				Vector3 tmpvector = Vector3.Cross(Random.insideUnitSphere, emitterDirection);
				Quaternion rotation = Quaternion.AngleAxis(Random.Range(minAngle, maxAngle), tmpvector);
				tmpvector = rotation * emitterDirection;
				tmpvector.Normalize();
				Vector3 startVector = Random.Range(minSpeed, maxSpeed) * tmpvector;

				CreateParticle (particlePrefab1, startVector, Camera.current.transform); // Camera.current.transform <-- first person camera
				CreateParticle (particlePrefab2, startVector, Camera.main.transform);
			}
			Destroy(gameObject);
		}
	}


	private void CreateParticle(GameObject particlePrefab, Vector3 startVector, Transform camera) 
	{
		GameObject particle = (GameObject)Instantiate (particlePrefab, transform.position, transform.rotation);
		particle.renderer.material = particleMaterial;

		ParticleLookAtTarget look = particle.GetComponent<ParticleLookAtTarget>();
		look.target = camera;

		ParticleMovement particleMovement = particle.GetComponent<ParticleMovement> ();
		particleMovement.speedVector = startVector;
		particleMovement.startLifeTime = particleLifeTime;
		particleMovement.fadeOutTime = particleFadeOutTime;
		particleMovement.gravity = gravity;
	}

	void Update () 
	{

	}
}

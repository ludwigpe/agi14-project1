using UnityEngine;
using System.Collections;

/// <summary>
/// Particle system. This component makes its GameObject act as an emitter for particles
/// </summary>
public class ParticleSystemCustom : MonoBehaviour 
{
	public GameObject particlePrefab;
	public Color particleColor = Color.white;
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

	private ArrayList particleList = new ArrayList();

	void Start () 
	{
		if (!isContinuous) {
			for (int i = 0; i < particleAmount; i++) {
				GameObject particle = (GameObject)Instantiate(particlePrefab, transform.position, transform.rotation);
				particle.renderer.material = particleMaterial;
				ParticleMovement particleMovement = particle.GetComponent<ParticleMovement>();

				// calc particle Direction
				Vector3 tmpvector = Vector3.Cross(Random.insideUnitSphere, emitterDirection);
				Quaternion rotation = Quaternion.AngleAxis(Random.Range(minAngle, maxAngle), tmpvector);
				Vector3 tmpvector2 = rotation * emitterDirection;
				tmpvector2.Normalize();

				// Calculate the particle's speedVector
				particleMovement.speedVector = Random.Range(minSpeed, maxSpeed) * tmpvector2;
				particleMovement.lifeTime = particleLifeTime;
				particleMovement.gravity = gravity;
				ParticleLookAtTarget look = particle.GetComponent<ParticleLookAtTarget>();
				look.target = Camera.main.transform;
			}
			Destroy(gameObject);
		}
	}

	void Update () 
	{

	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Particle system. This component makes its GameObject act as an emitter for particles
/// </summary>
public class ParticleSystem : MonoBehaviour 
{
	public GameObject particlePrefab;
	public Color particleColor = Color.white;
	public bool isContinuous = true;
	public int particleAmount;
	//public Vector3 emitterDirection;
	public Vector3 gravity = new Vector3 (0, -10, 0);
	//public float minAngle = 0;
	//public float maxAngle = 2+Mathf.PI;
	public float speed = 3;
	public float particleLifeTime;

	private ArrayList particleList = new ArrayList();

	void Start () 
	{
		if (!isContinuous) {
			for (int i = 0; i < particleAmount; i++) {
				GameObject particle = (GameObject)Instantiate(particlePrefab, transform.position, transform.rotation);
				ParticleMovement particleMovement = particle.GetComponent<ParticleMovement>();
				particleMovement.speedVector = Random.Range(0, Mathf.Abs(speed)) * Random.insideUnitSphere;
				particleMovement.lifeTime = particleLifeTime;
				particleMovement.gravity = gravity;
				ParticleLookAtTarget look = particle.GetComponent<ParticleLookAtTarget>();
				look.target = Camera.main.transform;
			}
		}
	}

	void Update () 
	{

	}
}

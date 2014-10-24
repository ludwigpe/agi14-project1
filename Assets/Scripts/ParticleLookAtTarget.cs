using UnityEngine;
using System.Collections;

/// <summary>
/// This component makes the particle always face the target.
/// </summary>
public class ParticleLookAtTarget : MonoBehaviour 
{
	public Transform target;

	void Update () 
	{
		if (target != null) transform.rotation = Quaternion.LookRotation(transform.position - target.position);
	}
}

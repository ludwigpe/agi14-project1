using UnityEngine;
using System.Collections;

// Component that makes an object follow another object.
public class FollowTargetScript : MonoBehaviour {

	public Transform target;
	public float persistentChaseDistance;		// At which distance the ghost will follow more precisely
	public float destinationUpdateFrequency;	// Amount of seconds between every update of the destination
	NavMeshAgent agent;

	void Start () {
		agent = GetComponent<NavMeshAgent>();

		// Update target position at a slowed down frequency to simulate AI stupidity
		InvokeRepeating("UpdateDestination", 0.0f, destinationUpdateFrequency);
	}
	
	// Update is called once per frame
	void Update () {

		// Update the target destination every frame when the two objects are close to eachother
		if (Vector3.Distance(transform.position, target.position) <= persistentChaseDistance)
			UpdateDestination();
	}

	// Update this objects target destination.
	void UpdateDestination () {
		agent.SetDestination(target.position);
	}
}

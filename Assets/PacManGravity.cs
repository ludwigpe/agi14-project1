using UnityEngine;
using System.Collections;

/// <summary>
/// Will attract and move pellet towards Pac-Man when near.
/// </summary>
public class PacManGravity : MonoBehaviour
{
    private GameObject pacman;
    private Transform pellet;
    private float pelletRadius;

    // Gravity related
    private bool applyGravity;
    private Vector3 velocity;
    public float velocityDecay = 0.95F;        // How much velocity decays at each update
    public float moveThreshold = 0.01F;        // Minimum speed required to start moving
    public float pacGravity    = 50F;          // How strong the attraction to Pac-Man is 
    
    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        applyGravity = false;
        pellet = transform.parent;
        velocity = Vector3.zero;

        // We need the size of the pellet in order to do a SphereCast of correct size
        SphereCollider[] sphereColliders = GetComponentsInParent<SphereCollider>();
        pelletRadius = sphereColliders[sphereColliders.Length - 1].radius * pellet.localScale.x;
    }
    
    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (velocity.magnitude > moveThreshold)
        {
            CollisionResolve();
            pellet.Translate(velocity * Time.deltaTime);

            if (velocity.magnitude <= moveThreshold)
            {
                velocity = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Does a SphereCast in currently heading direction.
    /// If collision is detected then velocity vector is changed so that 
    /// pellet does not move through colliding object.
    /// </summary>
    private void CollisionResolve()
    {
        Ray direction = new Ray(pellet.position, velocity.normalized);
        RaycastHit[] hits = Physics.SphereCastAll(direction, pelletRadius, velocity.magnitude * Time.deltaTime, 1);

        // Did we hit anything?
        if (hits != null)
        {
            foreach (RaycastHit hit in hits)
            {
                float dot = Vector3.Dot(velocity, hit.normal);
                Vector3 result = hit.normal * dot;
                velocity -= result;
            }
        }
    }

    /// <summary>
    /// Update which is called on a fixed time interval basis.
    /// Physics should be applied in a time consistent manner.
    /// </summary>
    void FixedUpdate()
    {
        velocity *= velocityDecay;
        if (applyGravity)
        {
            Vector3 pacmanPos = pacman.transform.position;
            Vector3 pelletPos = pellet.position;
            Vector3 newPos = pacmanPos - pelletPos;
            Vector3 direction = newPos.normalized;

            float dist = newPos.magnitude;
            float force = pacGravity / Mathf.Pow(dist, 2);
            velocity += direction * force * Time.deltaTime;
        }
    }

    /// <summary>
    /// Something has entered the zone of attraction of the pellet.
    /// </summary>
    /// <param name="other">Collider object.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            applyGravity = true;
            pacman = other.gameObject;
        }
    }

    /// <summary>
    /// Something has left the zone of attraction of the pellet.
    /// </summary>
    /// <param name="other">Collider object.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            applyGravity = false;
            pacman = null;
        }
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Responsible for handling Collisions between the Ghost and other objects.
/// </summary>
public class GhostCollision : MonoBehaviour 
{
    private bool useCollision = true;

    // Flicker
    private bool flicker = false;
    private bool turnedOn = false;
    private float startFlickerTime;
    private float nextFlickerTime = 0;
    private float endFlickerTime = 0;
    private float totalFlickerLength = 2;
    private float minFlickerInterval = 0.01f;
    private float maxFlickerInterval = 0.3f;

    // EMP related
    private bool empEncountered = false;
    private float empStartTime;                 // Time at which EMP was created
    private float empHitTime;                   // Time at which Ghost was hit by the EMP
    private Material[] turnedOnMaterials;       // Materials used when turned on
    private Material[] turnedOffMaterials;      // Materials used when turned off
    public Material turnedOffMaterial;          // Material to use when disabled

    // Links
    private Collider empCollider;
    private GameController gameController;
    private FollowTargetScript followTarget;

	/// <summary>
    /// Use this for initialization.
	/// </summary>
	void Start () 
    {
        Renderer unit_renderer = GetComponentInChildren<Renderer>();
        turnedOnMaterials = unit_renderer.materials;
        int nrMaterials = turnedOnMaterials.Length;
        turnedOffMaterials = new Material[nrMaterials];

        // Init turned off materials
        for (int i = 0; i < nrMaterials; i++)
        {
            turnedOffMaterials[i] = turnedOffMaterial;
        }

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("Cannot find 'GameController' script");
        }
        followTarget = GetComponent<FollowTargetScript>();
	}
	
	/// <summary>
    /// Update is called once per frame.
	/// </summary>
	void Update () {
        // Temporarily disabled by EMP
        if (empEncountered)
        {
            // EMP is now gone
            if (empCollider == null)
            {
                float empDestroyedTime = Time.time;
                float timeAfterEMP = empHitTime - empStartTime;
                float turnOnTime = empDestroyedTime + timeAfterEMP;
                TurnOnGhostAtTime(turnOnTime);
                empEncountered = false;
            }
        }
        else if (flicker)
        {
            if (Time.time > startFlickerTime)
            {
                if (Time.time > endFlickerTime)
                {
                    TurnOnGhost();
                    flicker = false;
                }
                else if (Time.time > nextFlickerTime)
                {
                    FlickerGhost();
                    nextFlickerTime += Random.Range(minFlickerInterval, maxFlickerInterval);
                }
            }
        }
	}

    /// <summary>
    /// Turns off the Ghost, thus disabling its pathfinding, audio and changing its material to mirror this.
    /// </summary>
    private void TurnOffGhost()
    {
        audio.Stop();
        useCollision = false;
        followTarget.StopFollowingTarget();

        Renderer unit_renderer = GetComponentInChildren<Renderer>();
        unit_renderer.materials = turnedOffMaterials;
    }

    /// <summary>
    /// Turns on the Ghost, enabling its pathfinding, audio and changing its material.
    /// </summary>
    private void TurnOnGhost()
    {
        audio.Play();
        useCollision = true;
        followTarget.ResumeFollowingTarget();

        Renderer unit_renderer = GetComponentInChildren<Renderer>();
        unit_renderer.materials = turnedOnMaterials;
    }

    /// <summary>
    /// Makes the Ghost flicker between turned on and off materials.
    /// </summary>
    private void FlickerGhost()
    {
        Renderer unit_renderer = GetComponentInChildren<Renderer>();
        turnedOn = !turnedOn;

        if (turnedOn)
        {
            unit_renderer.materials = turnedOnMaterials;
        }
        else
        {
            unit_renderer.materials = turnedOffMaterials;
        }
    }

    /// <summary>
    /// Turns on the Ghost after some time.
    /// After being turned on the Ghost will flicker for some seconds.
    /// </summary>
    /// <param name="time">Time at which Ghost should be turned on.</param>
    private void TurnOnGhostAtTime(float time)
    {
        flicker = true;
        startFlickerTime = time;
        nextFlickerTime = startFlickerTime + Random.Range(minFlickerInterval, maxFlickerInterval);
        endFlickerTime = startFlickerTime + totalFlickerLength;
    }

    /// <summary>
    /// Ghost has collided with a trigger collider.
    /// </summary>
    /// <param name="other">Trigger collider.</param>
    void OnTriggerEnter(Collider other)
    {
        // EMP
        if (other.gameObject.name.Equals("EMP_EFFECT"))
        {
            empCollider = other;
            TurnOffGhost();

            empEncountered = true;
            empStartTime = empCollider.GetComponentInParent<EMPExplosion>().StartedAt;
            empHitTime = Time.time;
        }
    }

    /// <summary>
    /// Ghost has collided with something.
    /// </summary>
    /// <param name="collision">Collision object.</param>
    void OnCollisionEnter(Collision collision)
    {
        if (useCollision)
        {
            GameObject collidee = collision.gameObject;
            if (collidee.CompareTag("Player") && !gameController.GameLost && !gameController.GameWon)
            {
                DeathCheck deathCheck = collision.gameObject.GetComponent<DeathCheck>();
                if (!deathCheck.IsDead)
                {
                    CharacterController charController = collidee.GetComponent<CharacterController>();
                    charController.enabled = false;
                    gameController.ControlsDisabled = true;

                    AnimationManager animationManager = collidee.GetComponent<AnimationManager>();
                    animationManager.PlayDeathAnimation();

                    PlaySoundEffect soundEffectManager = collidee.GetComponent<PlaySoundEffect>();
                    soundEffectManager.PlayLifeLostSound();

                    deathCheck.IsDead = true;
                }
            }
        }
    }

    #region Accessors
    public bool UseCollision
    {
        get
        {
            return useCollision;
        }
    }
    #endregion
}

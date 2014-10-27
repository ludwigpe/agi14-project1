using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the collision related logic with Pellets (i.e. increase score).
/// </summary>
public class PelletCollision : MonoBehaviour
{
    // How many points one pellet is worth
    public int scoreValue = 10;

    // Links to gameobjects
	public GameObject pickupEffectPrefab;
    private GameController gameController;
    private PelletLight pelletLight;
    private Collider empCollider;

    // EMP related
    private bool empEncountered = false;
    private float empStartTime;                 // Time at which EMP was created
    private float empHitTime;                   // Time at which Pellet was hit by the EMP

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("Cannot find 'GameController' script");
        }
        gameController.IncrementPelletCounter();
        pelletLight = GetComponent<PelletLight>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Has the EMP died?
        if (empEncountered && empCollider == null)
        {
            float empDestroyedTime = Time.time;
            float timeAfterEMP = empHitTime - empStartTime;
            float turnOnLightTime = empDestroyedTime + timeAfterEMP;
            pelletLight.TurnOnLightAtTime(turnOnLightTime);
            empEncountered = false;
        }
    }

    /// <summary>
    /// Something has collided with the pellet.
    /// </summary>
    /// <param name="other">Collider object.</param>
    void OnTriggerEnter(Collider other)
    {
        // EMP
        if (other.gameObject.name.Equals("EMP_EFFECT"))
        {
            empCollider = other;
            pelletLight.TurnOffLight();
            
            empEncountered = true;
            empStartTime = empCollider.GetComponentInParent<EMPExplosion>().StartedAt;
            empHitTime = Time.time;
        }
        // Pac-Man
        else if (other.gameObject.CompareTag("Player"))
        {
            gameController.AddScore(scoreValue);
            gameController.DecrementPelletCounter();

            float comboValue = gameController.IncreaseComboCounter();
            PlaySoundEffect playSoundEffect = other.gameObject.GetComponent<PlaySoundEffect>();
            playSoundEffect.PlayEatPelletSound(comboValue);

			Instantiate(pickupEffectPrefab, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}

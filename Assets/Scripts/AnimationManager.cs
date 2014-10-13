using UnityEngine;
using System.Collections;

/// <summary>
/// AnimationManager is responsible for playing animations and 
/// making sure that certain effects related to said animation occur.
/// </summary>
public class AnimationManager : MonoBehaviour 
{
    // Connection to animation
    private Animation animations;
    private string currentlyPlaying = "";

	/// <summary>
	/// Initializes the animation manager.
	/// </summary>
	void Start () 
    {
	}
	
	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	void Update () 
    {
        animations = GetComponentInChildren<Animation>();
        if (currentlyPlaying.Equals("Move"))
        {
            if (!animations.isPlaying)
            {
                currentlyPlaying = "";
            }
        }
        else if (currentlyPlaying.Equals("Death"))
        {
            if (!animations.isPlaying)
            {
                currentlyPlaying = "";
                DeathCheck deathCheck = GetComponent<DeathCheck>();
                deathCheck.Kill();
            }
        }  
	}

    /// <summary>
    /// Plays the move animation.
    /// </summary>
    public void PlayMoveAnimation()
    {
        animations.Play("Move");
        currentlyPlaying = "Move";
    }

    /// <summary>
    /// Plays the death animation.
    /// When done, will kill the unit.
    /// </summary>
    public void PlayDeathAnimation()
    {
        animations.Play("Death");
        currentlyPlaying = "Death";
    }
}

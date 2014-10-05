using UnityEngine;
using System.Collections;

/// <summary>
/// PlaySoundEffect handles playing of sound effects (well duh..)
/// and tries to do this as autonomously as possible.
/// </summary>
public class PlaySoundEffect : MonoBehaviour
{
    // AudioSources
    private AudioSource audio_source_brake;
    private AudioSource audio_source_move;
    private AudioSource audio_source_jump;
    private AudioSource audio_source_life_lost;
    private AudioSource audio_source_eat_pellet;

    private bool brakeSoundStarted = false;
    private int framesWithoutBrakeSound = 0;
    private bool brakeSoundHitThisFrame = false;

    private const int MAX_FRAMES_WO_BRAKE = 1;  // After this amount of frames without brake sfx => stop it
    private const float MIN_VEL_MAG = 0.1f;     // Minimum velocity magnitude to play brake sound effect

    // Use this for initialization
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 5)
        {
            Debug.Log("Could not find all AudioSources.");
        }
        audio_source_brake = audioSources[0];
        audio_source_move = audioSources[1];
        audio_source_jump = audioSources[2];
        audio_source_life_lost = audioSources[3];
        audio_source_eat_pellet = audioSources[4];
    }

    /// <summary>
    ///  Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (brakeSoundStarted && framesWithoutBrakeSound > MAX_FRAMES_WO_BRAKE)
        {
            audio_source_brake.Stop();
            brakeSoundStarted = false;
        }
        else if (brakeSoundStarted && !brakeSoundHitThisFrame)
        {
            framesWithoutBrakeSound++;
        }
        brakeSoundHitThisFrame = false;
    }

    /// <summary>
    /// Plays the jump sound effect.
    /// </summary>
    public void PlayJumpSound()
    {
        audio_source_jump.Play();
    }

    /// <summary>
    /// Tries to play the move sound effect.
    /// </summary>
    /// <returns>True if successful.</returns>
    public bool PlayMoveSound()
    {
        bool result = false;
        if (!audio_source_move.isPlaying)
        {
            audio_source_move.Play();
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Tries to play the brake sound effect.
    /// </summary>
    /// <param name="velMag">Magnitude of current velocity.</param>
    /// <returns>True if successful.</returns>
    public bool PlayBrakeSound(float velMag)
    {
        bool result = false;
        if (!audio_source_brake.isPlaying && !brakeSoundStarted && velMag > MIN_VEL_MAG)
        {
            audio_source_brake.Play();
            brakeSoundStarted = true;
            result = true;
        }
        framesWithoutBrakeSound = 0;
        brakeSoundHitThisFrame = true;
        return result;
    }

    /// <summary>
    /// Tries to play the life lost sound effect.
    /// </summary>
    /// <returns>True if successful.</returns>
    public bool PlayLifeLostSound()
    {
        bool result = false;
        if (!audio_source_life_lost.isPlaying)
        {
            audio_source_life_lost.Play();
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Tries to play the eat pellet sound effect.
    /// </summary>
    /// <param name="pelletsPerSecond">Number of pellets consumed per second.</param>
    /// <returns>True if successful.</returns>
    public bool PlayEatPellet(float pelletsPerSecond)
    {
        bool result = false;
        if (!audio_source_eat_pellet.isPlaying)
        {
            //float pitch = pelletsPerSecond / 5
            //audio_source_eat_pellet.pitch = ;
            audio_source_eat_pellet.Play();
            result = true;
        }
        return result;
    }
}

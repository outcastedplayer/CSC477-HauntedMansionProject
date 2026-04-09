using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds; // Array to hold multiple footstep sounds

    [Header("Pacing")]
    [Tooltip("How fast the footsteps play (lower = faster)")]
    public float stepDelay = 0.5f;
    private float stepTimer;

    void Update()
    {
        // Check if the player is pressing WASD or Arrow Keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // If the player is moving
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            // Count down the timer
            stepTimer -= Time.deltaTime;

            // When the timer hits zero, play a step!
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepDelay; // Reset the timer for the next step
            }
        }
        else
        {
            // If the player stops, reset the timer so the next step is immediate when they start moving again
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        // Make sure we actually assigned sounds in the inspector
        if (footstepSounds.Length > 0)
        {
            // Pick a random footstep sound from the list
            int index = Random.Range(0, footstepSounds.Length);
            footstepSource.clip = footstepSounds[index];

            // Slightly randomize the pitch so it doesn't sound like a machine gun
            footstepSource.pitch = Random.Range(0.9f, 1.1f);

            footstepSource.Play();
        }
    }
}
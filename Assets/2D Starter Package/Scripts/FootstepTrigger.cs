using UnityEngine;

public class FootstepTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepSounds; // Drag multiple footstep clips here
    [SerializeField] private float stepInterval = 0.4f; // Time between steps
    [SerializeField] private float minSpeedForFootsteps = 0.1f;
    
    private Rigidbody2D rb;
    private float stepTimer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (rb.linearVelocity.magnitude > minSpeedForFootsteps)
        {
            stepTimer += Time.deltaTime;
            
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
    
    void PlayFootstep()
    {
        if (footstepSounds.Length > 0)
        {
            // Play random footstep for variety
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}
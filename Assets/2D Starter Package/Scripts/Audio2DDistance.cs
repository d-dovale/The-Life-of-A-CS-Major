using UnityEngine;

public class Audio2DDistance : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform player;
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float maxVolume = 0.3f;
    
    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }
    
    private void Update()
    {
        if (player == null || audioSource == null) return;
        
        // Calculate 2D distance (ignoring Z axis)
        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(player.position.x, player.position.y)
        );
        
        // Calculate volume based on distance
        float volume;
        if (distance <= minDistance)
        {
            volume = maxVolume;
        }
        else if (distance >= maxDistance)
        {
            volume = 0f;
        }
        else
        {
            // Linear falloff between min and max distance
            float t = (distance - minDistance) / (maxDistance - minDistance);
            volume = Mathf.Lerp(maxVolume, 0f, t);
        }
        
        audioSource.volume = volume;
    }
}
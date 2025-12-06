using UnityEngine;
using System.Collections;

/// <summary>
/// Add this to your projectile prefab to delay collider activation.
/// This prevents the projectile from immediately colliding with nearby objects when spawned.
/// </summary>
public class ProjectileColliderDelay : MonoBehaviour
{
    [Tooltip("How long to wait before enabling the collider (in seconds)")]
    [SerializeField] private float colliderEnableDelay = 0.1f;
    
    private Collider2D[] colliders;
    
    private void Awake()
    {
        // Get all colliders on this object
        colliders = GetComponents<Collider2D>();
        
        // Disable them all immediately
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        // Re-enable after delay
        StartCoroutine(EnableCollidersAfterDelay());
    }
    
    private IEnumerator EnableCollidersAfterDelay()
    {
        yield return new WaitForSeconds(colliderEnableDelay);
        
        // Re-enable all colliders
        foreach (Collider2D col in colliders)
        {
            col.enabled = true;
        }
    }
}
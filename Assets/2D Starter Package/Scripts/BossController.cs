using UnityEngine;
using DigitalWorlds.StarterPackage2D;

public class BossController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string shootTriggerName = "Shoot";
    
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab; // Use GameObject instead of Projectile2D
    [SerializeField] private Transform launchTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float animationDelay = 0.3f;
    
    private void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    
    public void ShootAtPlayer()
    {
        Debug.Log("ShootAtPlayer called!");
        
        if (animator != null)
        {
            animator.SetTrigger(shootTriggerName);
        }
        
        Invoke(nameof(FireProjectile), animationDelay);
    }
    
    private void FireProjectile()
    {
        Debug.Log("FireProjectile called!");
        
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is NULL!");
            return;
        }
        
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is NULL!");
            return;
        }
        
        Vector3 spawnPosition = launchTransform != null ? launchTransform.position : transform.position;
        Vector2 direction = (playerTransform.position - spawnPosition).normalized;
        
        Debug.Log($"Spawning projectile at {spawnPosition} going toward {direction}");
        
        // Instantiate the projectile
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        if (newProjectile == null)
        {
            Debug.LogError("Failed to instantiate projectile!");
            return;
        }
        
        Debug.Log($"Projectile instantiated: {newProjectile.name}");
        
        // Get the Rigidbody2D and apply velocity directly
        Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
            Debug.Log($"Applied velocity: {rb.linearVelocity}");
        }
        else
        {
            Debug.LogError("Projectile has no Rigidbody2D!");
        }
        
        // Rotate the projectile to face the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        newProjectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
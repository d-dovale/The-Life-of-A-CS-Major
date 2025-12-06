using UnityEngine;

public class PulsingStinkRadius : MonoBehaviour
{
    [Header("Pulse Settings")]
    [Tooltip("Minimum scale multiplier for the stink radius")]
    [SerializeField] private float minScale = 0.7f;
    
    [Tooltip("Maximum scale multiplier for the stink radius")]
    [SerializeField] private float maxScale = 1.3f;
    
    [Tooltip("How fast the radius pulses (cycles per second)")]
    [SerializeField] private float pulseSpeed = 1f;
    
    [Header("References")]
    [Tooltip("The View Cone sprite that shows the stink radius")]
    [SerializeField] private Transform viewCone;
    
    [Tooltip("The collider that triggers damage (optional - will be found automatically if not assigned)")]
    [SerializeField] private CapsuleCollider2D stinkCollider;
    
    private Vector3 originalViewConeScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    
    private void Start()
    {
        // Find the view cone if not assigned
        if (viewCone == null)
        {
            viewCone = transform.Find("View Cone");
            if (viewCone == null)
            {
                Debug.LogWarning("View Cone not found! Please assign it in the inspector.");
                enabled = false;
                return;
            }
        }
        
        // Find the collider if not assigned
        if (stinkCollider == null)
        {
            stinkCollider = GetComponentInChildren<CapsuleCollider2D>();
        }
        
        // Store original values
        originalViewConeScale = viewCone.localScale;
        if (stinkCollider != null)
        {
            originalColliderSize = stinkCollider.size;
            originalColliderOffset = stinkCollider.offset;
        }
    }
    
    private void Update()
    {
        // Calculate pulsing scale using a sine wave
        float pulseValue = Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f);
        // Map from [-1, 1] to [minScale, maxScale]
        float currentScale = Mathf.Lerp(minScale, maxScale, (pulseValue + 1f) / 2f);
        
        // Apply scale to view cone
        viewCone.localScale = originalViewConeScale * currentScale;
        
        // Apply scale to collider if it exists
        if (stinkCollider != null)
        {
            stinkCollider.size = originalColliderSize * currentScale;
            stinkCollider.offset = originalColliderOffset * currentScale;
        }
    }
}
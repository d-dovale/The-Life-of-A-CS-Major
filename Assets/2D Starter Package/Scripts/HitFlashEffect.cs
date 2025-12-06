using UnityEngine;
using System.Collections;

public class HitFlashEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    [Tooltip("Color to flash when hit")]
    [SerializeField] private Color flashColor = Color.green;
    
    [Tooltip("Color to flash when healed")]
    [SerializeField] private Color healFlashColor = Color.green;
    
    [Tooltip("Duration of the flash in seconds")]
    [SerializeField] private float flashDuration = 0.2f;
    
    private SpriteRenderer spriteRenderer;
    private Material flashMaterial;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private bool isFlashing = false;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Create a unique material instance so we don't affect other sprites
            flashMaterial = spriteRenderer.material;
            originalColor = spriteRenderer.color;
        }
    }
    
    public void TriggerHitFlash()
    {
        // If already flashing, restart the flash
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine(flashColor));
    }
    
    public void TriggerHealFlash()
    {
        // If already flashing, restart the flash
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine(healFlashColor));
    }
    
    private IEnumerator FlashRoutine(Color colorToFlash)
    {
        if (spriteRenderer == null) yield break;
        
        isFlashing = true;
        spriteRenderer.color = colorToFlash;
        
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Only reset color if object still exists and hasn't been disabled
        if (spriteRenderer != null && gameObject.activeInHierarchy)
        {
            spriteRenderer.color = originalColor;
        }
        
        isFlashing = false;
    }
    
    private void OnDisable()
    {
        // Reset color when disabled to prevent it staying flashed
        if (spriteRenderer != null && !isFlashing)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    private void OnDestroy()
    {
        // Clean up material instance
        if (flashMaterial != null)
        {
            Destroy(flashMaterial);
        }
    }
}
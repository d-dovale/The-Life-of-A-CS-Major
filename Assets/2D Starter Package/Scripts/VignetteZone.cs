using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteZone : MonoBehaviour
{
    [Header("Vignette Settings")]
    [SerializeField] private float targetIntensity = 0.6f;
    [SerializeField] private float transitionSpeed = 2f;
    
    private Volume postProcessVolume;
    private Vignette vignette;
    private float originalIntensity;

    void Start()
    {
        // Find your post-processing volume
        postProcessVolume = FindObjectOfType<Volume>();
        
        if (postProcessVolume.profile.TryGet(out vignette))
        {
            originalIntensity = vignette.intensity.value;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeVignette(targetIntensity));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeVignette(originalIntensity));
        }
    }

    System.Collections.IEnumerator ChangeVignette(float target)
    {
        float current = vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            vignette.intensity.value = Mathf.Lerp(current, target, elapsed);
            yield return null;
        }
    }
}
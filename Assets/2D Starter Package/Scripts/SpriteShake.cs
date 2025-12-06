using System.Collections;
using UnityEngine;

public class SpriteShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeIntensity = 0.1f;
    
    [Header("Continuous Shake")]
    [SerializeField] private bool shakeOnStart = false;
    [SerializeField] private bool continuousShake = false;
    [SerializeField] private float delayBetweenShakes = 0.1f;
    
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        // Store the original local position
        originalPosition = transform.localPosition;
    }

    void Start()
    {
        if (shakeOnStart && !continuousShake)
        {
            Shake();
        }
        else if (continuousShake)
        {
            StartContinuousShake();
        }
    }

    // Call this method to trigger a single shake
    public void Shake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    // Overload to customize shake on the fly
    public void Shake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeIntensity = intensity;
        Shake();
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Random offset for shake
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to original position
        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }

    // Start continuous shaking
    public void StartContinuousShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ContinuousShakeRoutine());
    }

    // Stop continuous shaking
    public void StopContinuousShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
        transform.localPosition = originalPosition;
    }

    IEnumerator ContinuousShakeRoutine()
    {
        while (true)
        {
            // Shake
            float elapsed = 0f;
            while (elapsed < shakeDuration)
            {
                float x = Random.Range(-1f, 1f) * shakeIntensity;
                float y = Random.Range(-1f, 1f) * shakeIntensity;

                transform.localPosition = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Brief pause between shakes
            transform.localPosition = originalPosition;
            yield return new WaitForSeconds(delayBetweenShakes);
        }
    }

    // Optional: Reset position if something goes wrong
    public void ResetPosition()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
        transform.localPosition = originalPosition;
    }
}
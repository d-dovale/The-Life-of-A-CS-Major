using UnityEngine;
using System.Collections;

public class AudioFade : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float fadeDuration = 2f;
    
    private float originalVolume;
    
    void Start()
    {
        if (audioSource != null)
        {
            originalVolume = audioSource.volume;
        }
    }
    
    public void FadeOut()
    {
        if (audioSource != null)
        {
            StartCoroutine(FadeOutCoroutine());
        }
    }
    
    public void FadeIn()
    {
        if (audioSource != null)
        {
            StartCoroutine(FadeInCoroutine());
        }
    }
    
    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }
        
        audioSource.volume = 0f;
    }
    
    private IEnumerator FadeInCoroutine()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, originalVolume, elapsed / fadeDuration);
            yield return null;
        }
        
        audioSource.volume = originalVolume;
    }
}
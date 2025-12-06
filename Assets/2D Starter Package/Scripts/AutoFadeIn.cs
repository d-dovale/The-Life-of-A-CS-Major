using UnityEngine;

public class AutoFadeIn : MonoBehaviour
{
    public AudioFade audioFade;
    public AudioSource audioSource;
    public float targetVolume = 0.5f; // Set desired final volume
    
    void Start()
    {
        if (audioSource != null)
        {
            audioSource.volume = targetVolume; // Set target first so AudioFade saves it
        }
        
        // Wait one frame for AudioFade to save the volume
        StartCoroutine(StartFade());
    }
    
    System.Collections.IEnumerator StartFade()
    {
        yield return null; // Wait one frame
        audioSource.volume = 0f; // Now set to 0
        audioFade.FadeIn(); // Fade from 0 to targetVolume
    }
}
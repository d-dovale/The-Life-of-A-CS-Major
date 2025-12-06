using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlitchEffect : MonoBehaviour
{
    public Image glitchOverlay;
    public float minInterval = 3f;
    public float maxInterval = 8f;
    
    void Start()
    {
        StartCoroutine(RandomGlitch());
    }
    
    IEnumerator RandomGlitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            
            glitchOverlay.enabled = true;
            yield return new WaitForSeconds(0.1f);
            glitchOverlay.enabled = false;
        }
    }
}
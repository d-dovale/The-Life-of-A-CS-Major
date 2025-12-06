using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VideoFadeIn : MonoBehaviour
{
    public Image blackScreen;
    public float fadeInDuration = 1f;
    
    void Start()
    {
        StartCoroutine(FadeIn());
    }
    
    IEnumerator FadeIn()
    {
        float elapsed = 0;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1 - (elapsed / fadeInDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0);
    }
}
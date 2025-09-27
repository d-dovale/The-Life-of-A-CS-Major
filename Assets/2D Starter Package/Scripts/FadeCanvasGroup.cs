using System.Collections;
using UnityEngine;

public class FadeCanvasGroup : MonoBehaviour
{
    [Tooltip("CanvasGroup to control. If null, one will be added/used on this GameObject.")]
    public CanvasGroup canvasGroup;

    [Tooltip("Fade duration in seconds.")]
    public float duration = 0.5f;

    [Tooltip("Use unscaled time so fade still works when game is paused.")]
    public bool useUnscaledTime = true;

    [Tooltip("When fully hidden, optionally disable the GameObject.")]
    public bool disableWhenHidden = true;

    [Tooltip("Alpha value when hidden.")]
    public float hiddenAlpha = 0f;

    [Tooltip("Alpha value when shown.")]
    public float shownAlpha = 1f;

    Coroutine fadeCoroutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    public void ShowImmediate()
    {
        StopFade();
        gameObject.SetActive(true);
        canvasGroup.alpha = shownAlpha;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideImmediate()
    {
        StopFade();
        canvasGroup.alpha = hiddenAlpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if (disableWhenHidden) gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        StartFade(shownAlpha);
    }

    public void FadeOut()
    {
        StartFade(hiddenAlpha);
    }

    void StartFade(float targetAlpha)
    {
        StopFade();
        // ensure visible so fade is seen
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;
        float d = Mathf.Max(0.0001f, duration);

        // If fading in, enable interactions once finished (or enable early if desired)
        canvasGroup.interactable = startAlpha > 0.01f;
        canvasGroup.blocksRaycasts = canvasGroup.interactable;

        while (t < d)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(t / d));
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        bool visible = targetAlpha > 0.001f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        if (!visible && disableWhenHidden)
            gameObject.SetActive(false);

        fadeCoroutine = null;
    }
}
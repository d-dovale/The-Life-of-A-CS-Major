// ...existing code...
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStartMessage : MonoBehaviour
{
    [Tooltip("The UI GameObject that displays the message. If null, this GameObject will be used.")]
    public GameObject messageObject;

    [Tooltip("How long (seconds) the message stays visible.")]
    public float displaySeconds = 3f;

    [Tooltip("Only show on the very first time this scene is entered.")]
    public bool onlyFirstTime = true;

    // If a FadeCanvasGroup is present on the messageObject, it will be used.
    [Tooltip("Optional FadeCanvasGroup on the message object")]
    public FadeCanvasGroup fade;

    string visitKey;
    Coroutine hideCoroutine;

    void Start()
    {
        if (messageObject == null) messageObject = gameObject;

        // Key is per-scene so each level can have its own first-visit flag
        visitKey = "Visited_" + SceneManager.GetActiveScene().name;

        if (onlyFirstTime && PlayerPrefs.GetInt(visitKey, 0) == 1)
        {
            // Already visited: make sure it's hidden immediately
            // If fade exists, use HideImmediate so alpha/state is correct
            fade = fade ?? messageObject.GetComponent<FadeCanvasGroup>();
            if (fade != null) fade.HideImmediate();
            else messageObject.SetActive(false);
            return;
        }

        // ensure fade reference
        fade = fade ?? messageObject.GetComponent<FadeCanvasGroup>();

        // Show now (use fade if available)
        if (fade != null)
            fade.FadeIn();
        else
            messageObject.SetActive(true);

        if (onlyFirstTime)
            PlayerPrefs.SetInt(visitKey, 1); // mark visited

        if (displaySeconds > 0f)
            hideCoroutine = StartCoroutine(HideAfter(displaySeconds));
    }

    IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Use fade if present so we get fade-out animation
        if (fade != null)
            fade.FadeOut();
        else
            messageObject.SetActive(false);
    }

    // Optional: allow other scripts (e.g., TriggerEvents2D) to force showing the message
    public void ShowNow()
    {
        // stop pending hide
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        fade = fade ?? messageObject.GetComponent<FadeCanvasGroup>();

        if (fade != null)
        {
            fade.FadeIn();
            if (displaySeconds > 0f)
                hideCoroutine = StartCoroutine(DelayHide(displaySeconds));
        }
        else
        {
            messageObject.SetActive(true);
            if (displaySeconds > 0f)
            {
                StopAllCoroutines();
                StartCoroutine(HideAfter(displaySeconds));
            }
        }
    }

    IEnumerator DelayHide(float s)
    {
        yield return new WaitForSeconds(s);
        fade.FadeOut();
    }
}
// ...existing code...
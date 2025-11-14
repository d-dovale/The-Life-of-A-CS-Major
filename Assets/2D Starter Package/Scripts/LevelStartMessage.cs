using System.Collections;
using UnityEngine;

public class LevelStartMessage : MonoBehaviour
{
    [Tooltip("The UI GameObject that displays the message. If null, this GameObject will be used.")]
    public GameObject messageObject;

    [Tooltip("How long (seconds) the message stays visible.")]
    public float displaySeconds = 3f;

    [Tooltip("If true, this message can only be triggered one time.")]
    public bool singleUse = true;

    [Tooltip("Enter the tag name that should register collisions. Leave blank for any tag.")]
    [SerializeField] private string tagName = "Player";

    [Tooltip("Optional FadeCanvasGroup on the message object")]
    public FadeCanvasGroup fade;

    [Tooltip("Unique identifier for this trigger zone (used for PlayerPrefs persistence)")]
    public string triggerID = "LevelMessage_1";

    [Tooltip("If true, uses PlayerPrefs to persist across game sessions.")]
    public bool persistAcrossSessions = true;

    [HideInInspector] public bool hasBeenUsed = false;

    private string visitKey;
    private Coroutine hideCoroutine;

    void Start()
    {
        if (messageObject == null) messageObject = gameObject;

        // Set up the visit key for persistence
        visitKey = "Visited_" + triggerID;

        // Check if already used in a previous session
        if (persistAcrossSessions && PlayerPrefs.GetInt(visitKey, 0) == 1)
        {
            hasBeenUsed = true;
        }

        // Get fade reference if it exists
        fade = fade ?? messageObject.GetComponent<FadeCanvasGroup>();

        // Make sure message is hidden at start
        if (fade != null)
            fade.HideImmediate();
        else
            messageObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the correct tag
        if (!string.IsNullOrEmpty(tagName) && !collision.CompareTag(tagName))
            return;

        // Check if already used (if singleUse is enabled)
        if (singleUse && hasBeenUsed)
            return;

        // Trigger the message
        ShowMessage();
    }

    void ShowMessage()
    {
        // Mark as used
        if (singleUse)
        {
            hasBeenUsed = true;
            
            // Persist to PlayerPrefs if enabled
            if (persistAcrossSessions)
            {
                PlayerPrefs.SetInt(visitKey, 1);
                PlayerPrefs.Save();
            }
        }

        // Stop any pending hide
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // Show the message
        if (fade != null)
            fade.FadeIn();
        else
            messageObject.SetActive(true);

        // Schedule hide if duration is set
        if (displaySeconds > 0f)
            hideCoroutine = StartCoroutine(HideAfter(displaySeconds));
    }

    IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Use fade if present
        if (fade != null)
            fade.FadeOut();
        else
            messageObject.SetActive(false);
    }

    // Optional: allow other scripts to manually trigger the message
    public void ShowNow()
    {
        // Don't show if already used and singleUse is enabled
        if (singleUse && hasBeenUsed)
            return;

        ShowMessage();
    }

    // Optional: reset the trigger so it can be shown again
    public void ResetTrigger()
    {
        hasBeenUsed = false;
        
        if (persistAcrossSessions)
        {
            PlayerPrefs.DeleteKey(visitKey);
            PlayerPrefs.Save();
        }
    }

    private void OnValidate()
    {
        // Clamp displaySeconds to 0 in the inspector
        displaySeconds = Mathf.Max(0, displaySeconds);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DigitalWorlds.Dialogue;
using DigitalWorlds.StarterPackage2D;

public class TriviaManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject triviaUI;
    [SerializeField] private TextMeshProUGUI answerAText;
    [SerializeField] private TextMeshProUGUI answerBText;
    [SerializeField] private TextMeshProUGUI answerCText;
    [SerializeField] private TextMeshProUGUI answerDText;
    [SerializeField] private GameObject additionalUIObject;
    
    [Header("Dialogue System")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueTrigger dummyTrigger;
    
    [Header("Boss References")]
    [SerializeField] private Animator bossAnimator;
    [Tooltip("The transform to flip (usually the boss GameObject itself, or the sprite child if separate)")]
    [SerializeField] private Transform bossTransform;
    [Tooltip("If true, boss will flip to face the player when firing projectiles")]
    [SerializeField] private bool flipBossToFacePlayer = true;
    
    [Header("Projectile Attack")]
    [Tooltip("Drag in the projectile prefab")]
    [SerializeField] private Projectile2D projectilePrefab;
    [Tooltip("Position to spawn projectile from (if null, uses boss transform)")]
    [SerializeField] private Transform projectileLaunchPoint;
    [Tooltip("Speed of the projectile")]
    [SerializeField] private float projectileVelocity = 8f;
    [Tooltip("Delay after showing wrong answer before starting attack sequence")]
    [SerializeField] private float attackSequenceDelay = 0.5f;
    [Tooltip("Delay after flip before playing attack animation")]
    [SerializeField] private float delayAfterFlip = 0.3f;
    [Tooltip("Delay after animation starts before firing projectile")]
    [SerializeField] private float delayBeforeProjectile = 0.4f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;
    [SerializeField] private AudioClip projectileFireSound;
    [SerializeField] private AudioClip askQuestionSound;
    [Tooltip("Delay before playing the ask question sound (in seconds)")]
    [SerializeField] private float askQuestionSoundDelay = 0.3f;
    [Tooltip("Volume for the ask question sound (0 to 1)")]
    [Range(0f, 1f)]
    [SerializeField] private float askQuestionSoundVolume = 0.5f;
    [Tooltip("Background music audio source (optional - will be stopped on death and restarted on trivia restart)")]
    [SerializeField] private AudioSource backgroundMusicSource;
    
    [Header("Timing")]
    [Tooltip("How long question dialogue stays on screen before auto-closing")]
    [SerializeField] private float questionDialogueDisplayTime = 4f;
    [Tooltip("Pause after question closes before answer labels (A,B,C,D) appear")]
    [SerializeField] private float pauseBeforeAnswerLabels = 1.5f;
    [Tooltip("How long correct/wrong dialogue stays on screen before auto-closing")]
    [SerializeField] private float feedbackDialogueDisplayTime = 2f;
    [Tooltip("Pause after feedback closes before next question starts")]
    [SerializeField] private float pauseBeforeNextQuestion = 1f;
    
    [Header("Feedback Dialogue")]
    [SerializeField] private string[] correctDialogues = new string[] { "Impressive... for a human.", "Lucky guess." };
    [SerializeField] private string[] wrongDialogues = new string[] { "Wrong! Prepare yourself!", "Pathetic attempt." };
    [SerializeField] private string[] playerDeathDialogues = new string[] { "Pathetic. Let's try that again.", "You'll need to do better than that, human." };
    [Tooltip("How long death dialogue stays on screen before restarting")]
    [SerializeField] private float deathDialogueDisplayTime = 3f;
    [Tooltip("Pause after death dialogue before restarting trivia")]
    [SerializeField] private float pauseBeforeRestart = 1f;
    
    [Header("Questions")]
    [SerializeField] private TriviaQuestion[] questions;
    
    [Header("End Trivia")]
    [SerializeField] private DialogueTrigger endDialogueTrigger;
    [SerializeField] private AudioSource bossDeathAudioSource;
    [SerializeField] private float deathAnimationDuration = 2f;
    [SerializeField] private Image fadeImage; // Black full-screen image
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private int nextSceneIndex = 1;
    
    private int currentQuestionIndex = 0;
    private bool waitingForAnswer = false;
    private TriviaQuestion currentQuestion;
    private Transform playerTransform;
    
    [System.Serializable]
    public class TriviaQuestion
    {
        [TextArea(3,6)]
        [Tooltip("Format: Question text | A: answer | B: answer | C: answer | D: answer")]
        public string questionData;
        [Tooltip("Correct answer: A, B, C, or D")]
        public string correctAnswer;
        
        public string[] GetSpokenQuestion()
        {
            if (string.IsNullOrEmpty(questionData)) return new string[] { "" };
            string[] parts = questionData.Split('|');
            if (parts.Length == 0) return new string[] { questionData };
            
            // First line: just the question
            string questionLine = parts[0].Trim();
            
            // Second line: all the answers
            string answersLine = "";
            for (int i = 1; i < parts.Length; i++)
            {
                answersLine += parts[i].Trim();
                if (i < parts.Length - 1) answersLine += " ";
            }
            
            return new string[] { questionLine, answersLine };
        }
    }
    
    private void Start()
    {
        if (triviaUI != null) triviaUI.SetActive(false);
        if (additionalUIObject != null) additionalUIObject.SetActive(false);
        if (dialogueManager == null) dialogueManager = FindAnyObjectByType<DialogueManager>();
        
        // Cache player transform for projectile targeting
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // If boss transform not assigned, try to get it from this GameObject
        if (bossTransform == null)
        {
            bossTransform = transform;
        }
    }
    
    private void Update()
    {
        // Debug: Press P to skip to end trivia
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Skipping to end trivia...");
            currentQuestionIndex = questions.Length;
            if (waitingForAnswer)
            {
                waitingForAnswer = false;
                if (triviaUI != null) triviaUI.SetActive(false);
                if (additionalUIObject != null) additionalUIObject.SetActive(false);
            }
            EndTrivia();
        }
    }
    
    public void StartTrivia()
    {
        Debug.Log("Starting trivia");
        
        // Start/restart background music
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
        
        currentQuestionIndex = 0;
        ShowNextQuestion();
    }
    
    private void ShowNextQuestion()
    {
        if (currentQuestionIndex >= questions.Length)
        {
            Debug.Log("All questions complete!");
            EndTrivia();
            return;
        }
        
        currentQuestion = questions[currentQuestionIndex];
        
        // Trigger boss animation for asking question
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Ask");
        }
        
        // Play question sound effect with delay and custom volume
        if (audioSource != null && askQuestionSound != null)
        {
            StartCoroutine(PlayAskQuestionSound());
        }
        
        // Show question via dialogue
        ShowDialogue(currentQuestion.GetSpokenQuestion());
        
        // Wait for dialogue to finish, then show answer labels
        StartCoroutine(WaitForDialogueThenShowAnswers());
    }
    
    private IEnumerator PlayAskQuestionSound()
    {
        yield return new WaitForSeconds(askQuestionSoundDelay);
        audioSource.PlayOneShot(askQuestionSound, askQuestionSoundVolume);
    }
    
    private IEnumerator WaitForDialogueThenShowAnswers()
    {
        // Wait for dialogue to end
        while (dialogueManager != null && dialogueManager.IsInDialogue)
        {
            yield return null;
        }
        
        // Re-enable movement
        EnablePlayerMovement(true);
        
        // Short pause
        yield return new WaitForSeconds(pauseBeforeAnswerLabels);
        
        // Show answer labels
        if (triviaUI != null) triviaUI.SetActive(true);
        if (answerAText != null) answerAText.text = "A";
        if (answerBText != null) answerBText.text = "B";
        if (answerCText != null) answerCText.text = "C";
        if (answerDText != null) answerDText.text = "D";
        
        // Enable additional UI object
        if (additionalUIObject != null) additionalUIObject.SetActive(true);
        
        waitingForAnswer = true;
    }
    
    public void SubmitAnswer(string answer)
    {
        if (!waitingForAnswer)
        {
            Debug.Log("Not waiting for answer");
            return;
        }
        
        waitingForAnswer = false;
        
        // Hide answer options
        if (triviaUI != null) triviaUI.SetActive(false);
        if (additionalUIObject != null) additionalUIObject.SetActive(false);
        
        bool correct = answer.ToUpper() == currentQuestion.correctAnswer.ToUpper();
        
        Debug.Log($"Answer: {answer}, Correct: {currentQuestion.correctAnswer}, Result: {correct}");
        
        StartCoroutine(HandleFeedback(correct));
    }
    
    private IEnumerator HandleFeedback(bool correct)
    {
        // Play sound effect
        if (audioSource != null)
        {
            AudioClip clipToPlay = correct ? correctAnswerSound : wrongAnswerSound;
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }
        }
        
        // Show feedback dialogue
        string dialogue;
        if (correct)
        {
            dialogue = correctDialogues[Random.Range(0, correctDialogues.Length)];
            
            // Show dialogue
            ShowDialogue(dialogue, isQuestion: false);
            
            // Trigger react animation for correct answer
            if (bossAnimator != null)
            {
                bossAnimator.SetTrigger("React");
            }
        }
        else
        {
            // Add correct answer reveal to wrong dialogue
            string wrongDialogue = wrongDialogues[Random.Range(0, wrongDialogues.Length)];
            dialogue = $"{wrongDialogue} The correct answer was {currentQuestion.correctAnswer}.";
            
            // Show dialogue
            ShowDialogue(dialogue, isQuestion: false);
            
            // Start the attack sequence (flip -> animation -> projectile)
            StartCoroutine(AttackSequence());
        }
        
        // Wait for dialogue
        while (dialogueManager != null && dialogueManager.IsInDialogue)
        {
            yield return null;
        }
        
        // Re-enable movement
        EnablePlayerMovement(true);
        
        // Brief pause
        yield return new WaitForSeconds(pauseBeforeNextQuestion);
        
        // Next question
        currentQuestionIndex++;
        ShowNextQuestion();
    }
    
    private IEnumerator AttackSequence()
    {
        // Initial delay before starting attack
        yield return new WaitForSeconds(attackSequenceDelay);
        
        // Step 1: Flip boss to face player
        if (flipBossToFacePlayer && bossTransform != null)
        {
            FlipBossToFacePlayer();
        }
        
        // Step 2: Wait after flip
        yield return new WaitForSeconds(delayAfterFlip);
        
        // Step 3: Trigger attack animation
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Attack");
        }
        
        // Step 4: Wait before firing projectile
        yield return new WaitForSeconds(delayBeforeProjectile);
        
        // Step 5: Fire projectile and play sound
        FireProjectileAtPlayer();
    }
    
    private void FireProjectileAtPlayer()
    {
        Debug.Log("=== FireProjectileAtPlayer CALLED ===");
        
        // Validation checks
        if (projectilePrefab == null)
        {
            Debug.LogWarning("No projectile prefab assigned to TriviaManager!");
            return;
        }
        
        if (playerTransform == null)
        {
            Debug.LogWarning("Player transform not found!");
            return;
        }
        
        // Determine spawn position
        Vector3 spawnPosition = projectileLaunchPoint != null ? projectileLaunchPoint.position : bossTransform.position;
        Debug.Log($"Spawn position: {spawnPosition}");
        
        // Calculate direction to player
        Vector2 direction = (playerTransform.position - spawnPosition).normalized;
        Debug.Log($"Direction to player: {direction}, Player position: {playerTransform.position}");
        
        // Instantiate and launch projectile
        Debug.Log("About to instantiate projectile...");
        Projectile2D newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Projectile instantiated: {newProjectile != null}, GameObject active: {newProjectile.gameObject.activeSelf}");
        
        newProjectile.Launch(direction, projectileVelocity, null); // Pass null so it doesn't ignore any collisions
        Debug.Log("Projectile Launch() called");
        
        // Ignore collision between projectile and boss
        Collider2D projectileCollider = newProjectile.GetComponent<Collider2D>();
        Collider2D bossCollider = bossTransform.GetComponent<Collider2D>();
        
        Debug.Log($"Projectile collider: {projectileCollider != null}, Boss collider: {bossCollider != null}");
        
        if (projectileCollider != null && bossCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, bossCollider, true);
            Debug.Log("Ignoring collision between boss and projectile");
        }
        
        // Check if projectile still exists after a frame
        StartCoroutine(CheckProjectileExistence(newProjectile));
        
        // Play projectile fire sound
        if (audioSource != null && projectileFireSound != null)
        {
            audioSource.PlayOneShot(projectileFireSound);
        }
        
        Debug.Log("Boss fired projectile at player!");
    }
    
    private IEnumerator CheckProjectileExistence(Projectile2D projectile)
    {
        yield return new WaitForSeconds(0.1f);
        if (projectile == null)
        {
            Debug.LogError("PROJECTILE WAS DESTROYED within 0.1 seconds!");
        }
        else
        {
            Debug.Log("Projectile still exists after 0.1 seconds");
        }
    }
    
    private void FlipBossToFacePlayer()
    {
        if (bossTransform == null || playerTransform == null)
        {
            return;
        }
        
        // Calculate horizontal direction to player
        float directionToPlayer = playerTransform.position.x - bossTransform.position.x;
        
        // Get current scale
        Vector3 scale = bossTransform.localScale;
        
        // Flip based on player position
        // If player is to the right (directionToPlayer > 0), face right (positive x scale)
        // If player is to the left (directionToPlayer < 0), face left (negative x scale)
        if (directionToPlayer > 0 && scale.x < 0)
        {
            // Player is right, boss is facing left - flip to face right
            scale.x = Mathf.Abs(scale.x);
            bossTransform.localScale = scale;
        }
        else if (directionToPlayer < 0 && scale.x > 0)
        {
            // Player is left, boss is facing right - flip to face left
            scale.x = -Mathf.Abs(scale.x);
            bossTransform.localScale = scale;
        }
    }
    
    public void RepeatQuestion()
    {
        if (currentQuestion == null || !waitingForAnswer)
        {
            Debug.Log("No active question to repeat");
            return;
        }
        
        Debug.Log("Repeating question");
        ShowDialogue(currentQuestion.GetSpokenQuestion());
    }
    
    private void ShowDialogue(string[] texts, bool isQuestion = true)
    {
        // Disable continue image for trivia dialogue
        if (dialogueManager != null)
        {
            dialogueManager.SetContinueImageEnabled(false);
        }
        
        Queue<string> dialogue = new Queue<string>();
        dialogue.Enqueue("[NAME=ChatGPT]");
        dialogue.Enqueue("[SPEAKERSPRITE=ChatGPT Boss]");
        
        // Add all text lines
        foreach (string text in texts)
        {
            dialogue.Enqueue(text);
        }
        
        dialogue.Enqueue("EndQueue");
        
        if (dialogueManager != null)
        {
            if (dummyTrigger != null)
            {
                dialogueManager.CurrentTrigger = dummyTrigger;
            }
            dialogueManager.StartDialogue(dialogue);
            StartCoroutine(AutoAdvanceDialogue(isQuestion, texts.Length));
        }
    }
    
    // Overload for single string (for feedback dialogue)
    private void ShowDialogue(string text, bool isQuestion = true)
    {
        ShowDialogue(new string[] { text }, isQuestion);
    }
    
    private IEnumerator AutoAdvanceDialogue(bool isQuestion, int lineCount)
    {
        // Use different timing based on dialogue type
        float displayTime = isQuestion ? questionDialogueDisplayTime : feedbackDialogueDisplayTime;
        
        // Advance through each line
        for (int i = 0; i < lineCount; i++)
        {
            yield return new WaitForSeconds(displayTime);
            
            if (dialogueManager != null && dialogueManager.IsInDialogue)
            {
                dialogueManager.AdvanceDialogue();
            }
        }
    }
    
    private void EnablePlayerMovement(bool enabled)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<DigitalWorlds.StarterPackage2D.PlayerMovementTopDown>();
            if (movement != null)
            {
                movement.EnableMovement(enabled);
            }
        }
    }
    
    private void EndTrivia()
    {
        if (triviaUI != null) triviaUI.SetActive(false);
        if (additionalUIObject != null) additionalUIObject.SetActive(false);
        
        // Re-enable continue image for normal dialogue
        if (dialogueManager != null)
        {
            dialogueManager.SetContinueImageEnabled(true);
        }
        
        Debug.Log("Trivia complete! Starting end sequence...");
        
        // Start end sequence
        if (endDialogueTrigger != null)
        {
            StartCoroutine(PlayEndSequence());
        }
        else
        {
            Debug.LogWarning("No end dialogue trigger assigned!");
        }
    }
    
    private IEnumerator PlayEndSequence()
    {
        // Wait a moment for systems to settle
        yield return new WaitForSeconds(0.5f);
        
        // Re-enable player movement
        EnablePlayerMovement(true);
        
        // Enable the dialogue trigger - player should already be on it
        // Death sequence will be triggered by DialogueTrigger's onDialogueEnded event
        endDialogueTrigger.gameObject.SetActive(true);
    }
    
    public void PlayDeathSequence()
    {
        StartCoroutine(DeathAndSceneTransition());
    }
    
    // Called when player dies during trivia - shows taunt dialogue then restarts
    public void OnPlayerDeathDuringTrivia()
    {
        Debug.Log("Player died during trivia - restarting...");
        
        // Stop ALL running coroutines to prevent old question from continuing
        StopAllCoroutines();
        
        // Start the restart sequence
        StartCoroutine(HandlePlayerDeathRestart());
    }
    
    private IEnumerator HandlePlayerDeathRestart()
    {
        // Stop waiting for answer if we were
        waitingForAnswer = false;
        
        // Stop background music
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
        
        // Hide trivia UI
        if (triviaUI != null) triviaUI.SetActive(false);
        if (additionalUIObject != null) additionalUIObject.SetActive(false);
        
        // Force close any active dialogue
        if (dialogueManager != null && dialogueManager.IsInDialogue)
        {
            dialogueManager.AdvanceDialogue();
            // Wait a frame to ensure dialogue is closed
            yield return null;
        }
        
        // Show death taunt dialogue
        string deathDialogue = playerDeathDialogues[Random.Range(0, playerDeathDialogues.Length)];
        ShowDialogue(deathDialogue, isQuestion: false);
        
        // Wait for dialogue to display
        yield return new WaitForSeconds(deathDialogueDisplayTime);
        
        // Close dialogue
        if (dialogueManager != null && dialogueManager.IsInDialogue)
        {
            dialogueManager.AdvanceDialogue();
        }
        
        // Brief pause
        yield return new WaitForSeconds(pauseBeforeRestart);
        
        // Restart trivia from beginning (this will also restart music)
        StartTrivia();
    }
    
    private IEnumerator DeathAndSceneTransition()
    {
        // Play death animation and sound
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Death");
        }
        
        if (bossDeathAudioSource != null)
        {
            bossDeathAudioSource.Play();
        }
        
        // Wait for death animation
        yield return new WaitForSeconds(deathAnimationDuration);
        
        // Fade out and load scene
        yield return StartCoroutine(FadeOut());
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
    }
    
    private IEnumerator FadeOut()
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("No fade image assigned, skipping fade");
            yield return new WaitForSeconds(1f);
            yield break;
        }
        
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float elapsed = 0f;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = 1f;
        fadeImage.color = color;
    }
}
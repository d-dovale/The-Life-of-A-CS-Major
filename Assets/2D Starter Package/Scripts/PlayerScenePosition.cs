using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DigitalWorlds.StarterPackage2D;

public class PlayerScenePosition : MonoBehaviour
{
    [Tooltip("How long after scene load before player can move (prevents immediate scene re-trigger)")]
    [SerializeField] private float movementBufferTime = 0.5f;
    
    [Tooltip("How long player must be in scene before position is saved (prevents glitchy transitions)")]
    [SerializeField] private float positionSaveCooldown = 1.0f;
    
    [Header("Initial Facing Direction (optional)")]
    [Tooltip("Set initial facing direction for this scene. Leave at (0,0) to use animator's default.")]
    [SerializeField] private Vector2 initialFacingDirection = Vector2.zero;
    
    private PlayerMovementTopDown topDownMovement;
    private PlayerMovement2D platformerMovement;
    private Animator animator;
    private bool canSavePosition = false;
    
    private void Start()
    {
        // Get components
        topDownMovement = GetComponent<PlayerMovementTopDown>();
        platformerMovement = GetComponent<PlayerMovement2D>();
        
        // Look for animator on this object or in children
        animator = GetComponentInChildren<Animator>();
        
        // Disable movement immediately
        DisableMovement();
        
        // Try to load saved position for current scene
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (PlayerPositionManager.TryGetPosition(currentScene, out Vector3 savedPosition))
        {
            transform.position = savedPosition;
        }
        
        // Set initial facing direction if specified
        if (initialFacingDirection != Vector2.zero && animator != null)
        {
            animator.SetFloat("LastInputX", initialFacingDirection.x);
            animator.SetFloat("LastInputY", initialFacingDirection.y);
            animator.SetFloat("InputX", initialFacingDirection.x);
            animator.SetFloat("InputY", initialFacingDirection.y);
        }
        
        // Re-enable movement after buffer time
        StartCoroutine(EnableMovementAfterDelay());
        
        // Allow position saving after cooldown
        StartCoroutine(EnablePositionSavingAfterDelay());
    }
    
    private IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(movementBufferTime);
        EnableMovement();
    }
    
    private IEnumerator EnablePositionSavingAfterDelay()
    {
        yield return new WaitForSeconds(positionSaveCooldown);
        canSavePosition = true;
    }
    
    private void DisableMovement()
    {
        if (topDownMovement != null)
            topDownMovement.EnableMovement(false);
        if (platformerMovement != null)
            platformerMovement.EnableMovement(false);
    }
    
    private void EnableMovement()
    {
        if (topDownMovement != null)
            topDownMovement.EnableMovement(true);
        if (platformerMovement != null)
            platformerMovement.EnableMovement(true);
    }
    
    private void OnDestroy()
    {
        // Only save position if enough time has passed in this scene
        if (Application.isPlaying && canSavePosition)
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            PlayerPositionManager.SavePosition(currentScene, transform.position);
        }
    }
}
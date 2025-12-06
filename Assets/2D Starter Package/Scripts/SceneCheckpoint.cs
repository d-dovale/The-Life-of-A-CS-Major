using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCheckpoint : MonoBehaviour
{
    [Tooltip("The scene index this checkpoint saves for (usually the PREVIOUS scene you came from)")]
    [SerializeField] private int sceneToSaveFor;
    
    [Tooltip("Optional: Specific position to save. Leave empty to use this checkpoint's position.")]
    [SerializeField] private Transform spawnPosition;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Use spawn position if provided, otherwise use checkpoint's position
            Vector3 positionToSave = spawnPosition != null ? spawnPosition.position : transform.position;
            PlayerPositionManager.SavePosition(sceneToSaveFor, positionToSave);
        }
    }
}
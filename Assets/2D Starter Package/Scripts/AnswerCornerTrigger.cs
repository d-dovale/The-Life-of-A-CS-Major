using UnityEngine;

public class AnswerCornerTrigger : MonoBehaviour
{
    [SerializeField] private string answer; // Set to "A", "B", "C", or "D" in inspector
    [SerializeField] private TriviaManager triviaManager;
    
    private void Start()
    {
        if (triviaManager == null)
        {
            triviaManager = FindAnyObjectByType<TriviaManager>();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Player entered corner {answer}");
            triviaManager.SubmitAnswer(answer);
        }
    }
}
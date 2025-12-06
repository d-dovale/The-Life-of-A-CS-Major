using UnityEngine;

public class RepeatQuestionTrigger : MonoBehaviour
{
    [SerializeField] private TriviaManager triviaManager;
    [SerializeField] private float cooldown = 1f;
    
    private float lastTriggerTime = 0f;
    
    private void Start()
    {
        if (triviaManager == null)
        {
            triviaManager = FindAnyObjectByType<TriviaManager>();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= lastTriggerTime + cooldown)
        {
            Debug.Log("Replaying question");
            triviaManager.RepeatQuestion();
            lastTriggerTime = Time.time;
        }
    }
}
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float countdownTime = 30f; // Set the total time for the countdown

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI gameTimerText;

    [SerializeField] private GameManager gameManager;

    private float currentTime; // To track the remaining time
    private bool isPaused = true; // Timer starts paused by default

    void Start()
    {
        currentTime = countdownTime; // Initialize the timer
        UpdateTimerDisplay(); // Update the timer display at the start

        if(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            Debug.Log("GameManager not set in the inspector. Trying to find one in the scene.");
        }
    }

    void Update()
    {
        // Only update the timer if it's not paused
        if (!isPaused && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                currentTime = 0;
                HandleTimeOut(); // Handle when the timer reaches zero
            }
        }
    }

    // Method to pause the timer
    public void PauseTimer()
    {
        isPaused = true;
    }

    // Method to resume the timer
    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void ResetTimer()
    {
        currentTime = countdownTime;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        // Format the time as seconds
        string timerText = Mathf.Ceil(currentTime).ToString("00");

        gameTimerText.text = timerText;
        //player2TimerText.text = timerText;
    }

    private void HandleTimeOut()
    {
        // Logic when the timer runs out
        Debug.Log("Time's Up!");
        gameManager.GameOver();
    }
}
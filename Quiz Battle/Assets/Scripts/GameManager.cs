using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private APIManager apiManager;
    [SerializeField] private Timer gameTimer;

    [Header("Player 1 UI Elements")]
    [SerializeField] private TextMeshProUGUI player1QuestionText;
    [SerializeField] private Button[] player1ChoiceButtons; // Assign buttons in the inspector for Player 1
    [SerializeField] private TextMeshProUGUI[] player1ChoiceTexts; // Choices for Player 1
    [SerializeField] private TextMeshProUGUI player1ScoreText;

    [Header("Player 2 UI Elements")]
    [SerializeField] private TextMeshProUGUI player2QuestionText;
    [SerializeField] private Button[] player2ChoiceButtons; // Assign buttons in the inspector for Player 2
    [SerializeField] private TextMeshProUGUI[] player2ChoiceTexts; // Choices for Player 2
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    

    [Header("Miscellaneous")]
    private string correctAnswer; // Store the correct answer
    private int player1Score = 0; // Player 1 score
    private int player2Score = 0; // Player 2 score
    private List<(string question, string[] choices, string correctAnswer)> questionQueue;
    private int currentQuestionIndex = 0;

    [SerializeField] private PlayerChoiceNavigator player1ChoiceNavigator;
    [SerializeField] private PlayerChoiceNavigator player2ChoiceNavigator;

    [Header("GameOver Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI player1FinalScoreText;
    [SerializeField] private TextMeshProUGUI player2FinalScoreText;
    [SerializeField] private Image[] resultIcons; // 0 for crown, 1 for trash bin
    [SerializeField] private Image player1ResultIcon;
    [SerializeField] private Image player2ResultIcon;

    /*
     * TODO: WHILE WAITING FOR THE API RESPONSE, LOADING SCREEN SHOULD BE DISPLAYED
     */ 

    private void Start()
    {
        if (apiManager == null)
        {
            apiManager = FindObjectOfType<APIManager>();
            Debug.Log("APIManager not set in the inspector. Trying to find one in the scene.");
        }

        if (gameTimer == null)
        {
            gameTimer = FindObjectOfType<Timer>();
            Debug.Log("Timer not set in the inspector. Trying to find one in the scene.");
        }

        if (player1ChoiceNavigator == null)
        {
            player1ChoiceNavigator = FindObjectOfType<PlayerChoiceNavigator>();
            Debug.Log("PlayerChoiceNavigator not set in the inspector. Trying to find one in the scene.");
        }

        if (player2ChoiceNavigator == null)
        {
            player2ChoiceNavigator = FindObjectOfType<PlayerChoiceNavigator>();
            Debug.Log("PlayerChoiceNavigator not set in the inspector. Trying to find one in the scene.");
        }

        questionQueue = new List<(string, string[], string)>(); // Initialize the question queue

        player1ScoreText.text = "Score: " + player1Score;
        player2ScoreText.text = "Score: " + player2Score;
    }

    public void GetQuestionsFromAPI(string prompt)
    {
        gameTimer.ResetTimer(); // Reset the timer
        gameTimer.PauseTimer(); // Pause timer during question fetch
        StartCoroutine(apiManager.GetQuestionsFromAPI(prompt, OnQuestionsSuccess, HandleError));
    }

    private void OnQuestionsSuccess(List<string> questions)
    {
        foreach (var qBlock in questions)
        {
            var (question, choices, correct) = ParseQuestionAndChoices(qBlock);
            questionQueue.Add((question, choices, correct));
        }

        currentQuestionIndex = 0;
        DisplayQuestion(currentQuestionIndex); // Display the first question
        gameTimer.ResumeTimer();
    }

    private (string question, string[] choices, string correctAnswer) ParseQuestionAndChoices(string content)
    {
        // Split the content into lines
        string[] lines = content.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        string question = null;
        var choices = new List<string>();
        string correctAnswer = "";

        foreach (var line in lines)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("Correct Answer:", StringComparison.OrdinalIgnoreCase))
            {
                correctAnswer = trimmedLine.Substring("Correct Answer:".Length).Trim();
            }
            else if (trimmedLine.StartsWith("Question:", StringComparison.OrdinalIgnoreCase))
            {
                question = trimmedLine.Substring("Question:".Length).Trim(); // Extract text after "Question:"
            }
            else{
                choices.Add(trimmedLine);
            }
        }

        return (question, choices.ToArray(), correctAnswer);
    }

    private void DisplayQuestion(int index)
    {
        if (index >= questionQueue.Count) return;

        var currentQuestion = questionQueue[index];
        correctAnswer = currentQuestion.correctAnswer; // Store the correct answer

        // Assign the question and choices to Player 1's UI
        player1QuestionText.text = currentQuestion.question;
        AssignChoicesToUI(player1ChoiceTexts, currentQuestion.choices);

        // Assign the same question and choices to Player 2's UI
        player2QuestionText.text = currentQuestion.question;
        AssignChoicesToUI(player2ChoiceTexts, currentQuestion.choices);
    }

    private void AssignChoicesToUI(TextMeshProUGUI[] choiceTexts, string[] choices)
    {
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceTexts[i].text = choices[i];
            }
            else
            {
                choiceTexts[i].text = ""; // Clear unused choice slots
            }
        }
    }

    private void HandleError(string errorMessage)
    {
        Debug.LogError(errorMessage);

        // Handle errors for Player 1
        player1QuestionText.text = errorMessage;
        foreach (var choiceText in player1ChoiceTexts)
        {
            choiceText.text = "";
        }

        // Handle errors for Player 2
        player2QuestionText.text = errorMessage;
        foreach (var choiceText in player2ChoiceTexts)
        {
            choiceText.text = "";
        }
    }

    public void EnablePlayerButtons() // Call this when the next question is loaded
    {
        player1ChoiceNavigator.enabled = true;
        player2ChoiceNavigator.enabled = true;

        foreach (var button in player1ChoiceButtons)
        {
            button.interactable = true;
        }
        foreach (var button in player2ChoiceButtons)
        {
            button.interactable = true;
        }

        // Reset the answer flags in the AnswerValidator scripts
        foreach (var validator in FindObjectsOfType<AnswerValidator>())
        {
            validator.ResetAnswerFlag();
        }
    }

    public void DisablePlayerButtons()
    {
        player1ChoiceNavigator.enabled = false;
        player2ChoiceNavigator.enabled = false;

        foreach (var button in player1ChoiceButtons)
        {
            button.interactable = false;
        }
        foreach (var button in player2ChoiceButtons)
        {
            button.interactable = false;
        }
    }

    public void UpdatePlayerScore(int playerNumber, int score)
    {
        if (playerNumber == 1)
        {
            player1Score += score;
            player1ScoreText.text = "Score: " + player1Score;
            Debug.Log("Player 1 Score: " + player1Score);
        }
        else if (playerNumber == 2)
        {
            player2Score += score;
            player2ScoreText.text = "Score: " + player2Score;
            Debug.Log("Player 2 Score: " + player2Score);
        }
    }

    public void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < questionQueue.Count)
        {
            DisplayQuestion(currentQuestionIndex); // Display the next question
            EnablePlayerButtons(); // Enable buttons for both players
        }
        else
        {
            // UI FOR FINAL SCORES
            DisablePlayerButtons(); // Disable buttons for both players
           // Time.timeScale = 0; // Pause the game
            Debug.Log("Quiz Finished!");
            // Logic to end the quiz or show final scores
        }
    }

    public string GetCorrectAnswer() // Method to get the correct answer
    {
        return correctAnswer;
    }

    public void PauseGame()
    {
        gameTimer.PauseTimer();
        DisablePlayerButtons();
    }

    public void ResumeGame()
    {
        gameTimer.ResumeTimer();
        EnablePlayerButtons();
    }

    public void ResetGame()
    {
        player1Score = 0;
        player2Score = 0;
        player1ScoreText.text = "Score: " + player1Score;
        player2ScoreText.text = "Score: " + player2Score;

        questionQueue.Clear();
        currentQuestionIndex = 0;
    }

    public void GameOver()
    {
        PauseGame();
        gameOverPanel.SetActive(true);
        player1FinalScoreText.text = "Player 1\nScore: " + player1Score;
        player2FinalScoreText.text = "Player 2\nScore: " + player2Score;

        if (player1Score > player2Score)
        {
            player1ResultIcon.sprite = resultIcons[0].sprite;
            player2ResultIcon.sprite = resultIcons[1].sprite;
        }
        else if (player1Score < player2Score)
        {
            player1ResultIcon.sprite = resultIcons[1].sprite;
            player2ResultIcon.sprite = resultIcons[0].sprite;
        }
        else
        {
            player1ResultIcon.sprite = resultIcons[0].sprite;
            player2ResultIcon.sprite = resultIcons[0].sprite;
        }
    }

    /*private void OnQuestionSuccess(string jsonResponse)
    {
        Debug.Log("API Response: " + jsonResponse);

        var chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);
        if (chatResponse.choices != null && chatResponse.choices.Length > 0)
        {
            string content = chatResponse.choices[0].message.content.Trim();
            var (question, choices, correct) = ParseQuestionAndChoices(content);

            correctAnswer = correct; // Store the correct answer

            // Assign the question and choices to Player 1's UI
            player1QuestionText.text = question;
            AssignChoicesToUI(player1ChoiceTexts, choices);

            // Assign the same question and choices to Player 2's UI
            player2QuestionText.text = question;
            AssignChoicesToUI(player2ChoiceTexts, choices);

            // Resume the timer after updating the UI
            gameTimer.ResumeTimer();
        }
        else
        {
            HandleError("Failed to load question.");
        }
    }

    public void GetQuestionFromAPI(string prompt)
    {
        gameTimer.PauseTimer(); // Pause timer during question fetch
        StartCoroutine(apiManager.GetQuestionFromAPI(prompt, OnQuestionSuccess, HandleError));
    }*/
}
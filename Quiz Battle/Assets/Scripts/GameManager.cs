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
    [SerializeField] private Button[] player1OptionButtons; // Assign buttons in the inspector for Player 1
    [SerializeField] private TextMeshProUGUI[] player1OptionTexts; // Options for Player 1
    [SerializeField] private TextMeshProUGUI player1ScoreText;

    [Header("Player 2 UI Elements")]
    [SerializeField] private TextMeshProUGUI player2QuestionText;
    [SerializeField] private Button[] player2OptionButtons; // Assign buttons in the inspector for Player 2
    [SerializeField] private TextMeshProUGUI[] player2OptionTexts; // Options for Player 2
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    

    [Header("Miscellaneous")]
    private string correctAnswer; // Store the correct answer
    private int player1Score = 0; // Player 1 score
    private int player2Score = 0; // Player 2 score
    private List<(string question, string[] options, string correctAnswer)> questionQueue;
    private int currentQuestionIndex = 0;
    [SerializeField] private GameObject loadingPanel;

    [SerializeField] private PlayerChoiceNavigator player1OptionNavigator;
    [SerializeField] private PlayerChoiceNavigator player2OptionNavigator;

    [Header("GameOver Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI player1FinalScoreText;
    [SerializeField] private TextMeshProUGUI player2FinalScoreText;
    [SerializeField] private Image[] resultIcons; // 0 for crown, 1 for trash bin
    [SerializeField] private Image player1ResultIcon;
    [SerializeField] private Image player2ResultIcon;

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

        if (player1OptionNavigator == null)
        {
            player1OptionNavigator = FindObjectOfType<PlayerChoiceNavigator>();
            Debug.Log("PlayerChoiceNavigator not set in the inspector. Trying to find one in the scene.");
        }

        if (player2OptionNavigator == null)
        {
            player2OptionNavigator = FindObjectOfType<PlayerChoiceNavigator>();
            Debug.Log("PlayerChoiceNavigator not set in the inspector. Trying to find one in the scene.");
        }

        loadingPanel.SetActive(false); // Hide the loading panel

        questionQueue = new List<(string, string[], string)>(); // Initialize the question queue

        player1ScoreText.text = "Score: " + player1Score;
        player2ScoreText.text = "Score: " + player2Score;
    }

    public void GetQuestionsFromAPI(string prompt)
    {
        loadingPanel.SetActive(true); // Show the loading panel
        gameTimer.ResetTimer(); // Reset the timer
        gameTimer.PauseTimer(); // Pause timer during question fetch
        StartCoroutine(apiManager.GetQuestionsFromAPI(prompt, OnQuestionsSuccess, HandleError));
    }

    private void OnQuestionsSuccess(List<string> questions)
    {
        foreach (var qBlock in questions)
        {
            var (question, options, correct) = ParseQuestionAndOptions(qBlock);
            questionQueue.Add((question, options, correct));
        }

        loadingPanel.SetActive(false); // Hide the loading panel
        currentQuestionIndex = 0;
        DisplayQuestion(currentQuestionIndex); // Display the first question
        gameTimer.ResumeTimer();
        EnablePlayerButtons(); // Enable buttons for both players
    }

    private (string question, string[] options, string correctAnswer) ParseQuestionAndOptions(string content)
    {
        Debug.Log("Raw Content: " + content);  // Log the content to see what is received

        string[] lines = content.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        string question = null;
        var options = new List<string>();
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
                question = trimmedLine.Substring("Question:".Length).Trim();
            }
            else if (trimmedLine.StartsWith("A)") || trimmedLine.StartsWith("B)") || trimmedLine.StartsWith("C)") || trimmedLine.StartsWith("D)"))
            {
                options.Add(trimmedLine);  // Only add lines starting with options (A, B, C, D)
            }
        }

        Debug.Log($"Parsed Question: {question}");
        foreach (var choice in options)
        {
            Debug.Log($"Choice: {choice}");
        }
        Debug.Log($"Correct Answer: {correctAnswer}");

        return (question, options.ToArray(), correctAnswer);
    }



    private void DisplayQuestion(int index)
    {
        if (index >= questionQueue.Count) return;

        var currentQuestion = questionQueue[index];
        correctAnswer = currentQuestion.correctAnswer; // Store the correct answer

        // Assign the question and options to Player 1's UI
        player1QuestionText.text = currentQuestion.question;
        AssignOptionsToUI(player1OptionTexts, currentQuestion.options);

        // Assign the same question and options to Player 2's UI
        player2QuestionText.text = currentQuestion.question;
        AssignOptionsToUI(player2OptionTexts, currentQuestion.options);
    }

    private void AssignOptionsToUI(TextMeshProUGUI[] choiceTexts, string[] options)
    {
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < options.Length)
            {
                choiceTexts[i].text = options[i];
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
        foreach (var choiceText in player1OptionTexts)
        {
            choiceText.text = "";
        }

        // Handle errors for Player 2
        player2QuestionText.text = errorMessage;
        foreach (var choiceText in player2OptionTexts)
        {
            choiceText.text = "";
        }
    }

    public void EnablePlayerButtons() // Call this when the next question is loaded
    {
        player1OptionNavigator.enabled = true;
        player2OptionNavigator.enabled = true;

        foreach (var button in player1OptionButtons)
        {
            button.interactable = true;
        }
        foreach (var button in player2OptionButtons)
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
        player1OptionNavigator.enabled = false;
        player2OptionNavigator.enabled = false;

        foreach (var button in player1OptionButtons)
        {
            button.interactable = false;
        }
        foreach (var button in player2OptionButtons)
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
            ResetButtonColors();
            DisplayQuestion(currentQuestionIndex); // Display the next question
            ResumeGame(); // Resume the game
        }
        else
        {
            // UI FOR FINAL SCORES
          //  DisablePlayerButtons(); // Disable buttons for both players
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
        // Reset scores
        player1Score = 0;
        player2Score = 0;
        player1ScoreText.text = "Score: " + player1Score;
        player2ScoreText.text = "Score: " + player2Score;

        // Clear the question queue and reset the index
        questionQueue.Clear();
        currentQuestionIndex = 0;

        // Clear question texts
        player1QuestionText.text = "";
        player2QuestionText.text = "";

        // Clear choice texts for both players
        ClearOptionTexts(player1OptionTexts);
        ClearOptionTexts(player2OptionTexts);

        // Disable player navigation until a new question is loaded
        DisablePlayerButtons();
        ResetButtonColors();
    }

    // Helper method to clear choice texts
    private void ClearOptionTexts(TextMeshProUGUI[] choiceTexts)
    {
        foreach (var choiceText in choiceTexts)
        {
            choiceText.text = ""; // Set each choice text to an empty string
        }
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

    public void HighlightCorrectAnswer(bool isPlayer1)
    {
        Button[] playerButtons = isPlayer1 ? player1OptionButtons : player2OptionButtons;

        foreach (var button in playerButtons)
        {
            string buttonText = button.GetComponentInChildren<TextMeshProUGUI>().text;
            string optionLetter = buttonText.Substring(0, 1);  // Extract the option letter (A, B, C, or D)

            // Highlight the correct button if it matches the correct answer
            if (optionLetter.Equals(correctAnswer.Substring(0, 1), StringComparison.OrdinalIgnoreCase))
            {
                SetButtonColor(button, Color.green);  // Highlight correct answer in green
                break;  // Stop after highlighting the correct button
            }
        }
    }

    private void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color; // Set the normal color
        colors.disabledColor = color; // Ensure disabled buttons retain the color
        button.colors = colors;
    }

    public void ResetButtonColors()
    {
        ResetButtons(player1OptionButtons);
        ResetButtons(player2OptionButtons);
    }

    private void ResetButtons(Button[] buttons)
    {
        foreach (var button in buttons)
        {
            SetButtonColor(button, Color.white); // Reset to default color
        }
    }
}
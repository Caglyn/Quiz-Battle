using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class AnswerValidator : MonoBehaviour
{
    [Header("Player Configuration")]
    [SerializeField] private bool isPlayer1;
    [SerializeField] private GameManager gameManager;

    private bool hasAnswered = false; // Flag to track if the player has already answered
    
    [SerializeField] private int correctAnswerPoint = 10;
    [SerializeField] private int wrongAnswerPoint = -10;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            Debug.Log("GameManager not set in the inspector. Trying to find one in the scene.");
        }
    }

    public void ValidateAnswer(Button clickedButton)
    {
        if (hasAnswered)
            return;

        hasAnswered = true;

        // Clean up the selected answer (extract the text after the option letter)
        string selectedAnswer = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text.Trim();
        string selectedOption = selectedAnswer.Substring(0, 1);  // Extract 'A', 'B', etc.

        // Clean up the correct answer for comparison
        string correctOption = gameManager.GetCorrectAnswer().Substring(0, 1); // Extract 'B' from "B)"

       // Debug.Log("Player selected: " + selectedOption);
       // Debug.Log("Correct answer: " + correctOption);

        if (selectedOption.Equals(correctOption, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Correct answer!");
            if (isPlayer1)
                gameManager.UpdatePlayerScore(1, correctAnswerPoint);
            else
                gameManager.UpdatePlayerScore(2, correctAnswerPoint);
        }
        else
        {
            Debug.Log("Wrong answer.");
            SetButtonColor(clickedButton, Color.red);  // Highlight wrong answer in red
            if (isPlayer1)
                gameManager.UpdatePlayerScore(1, wrongAnswerPoint);
            else
                gameManager.UpdatePlayerScore(2, wrongAnswerPoint);
        }

        // Highlight the correct answer (whether the answer was correct or wrong)
        gameManager.HighlightCorrectAnswer(isPlayer1);
        
        StartCoroutine(DelayNextQuestion());
    }

    // Helper method to set button color
    private void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color; // Set the normal color
        colors.disabledColor = color; // Ensure disabled buttons retain the color
        button.colors = colors;
    }

    private IEnumerator DelayNextQuestion()
    {
        //gameManager.PauseGame(); // Keep the game paused for a short time
        gameManager.DisablePlayerButtons();
        yield return new WaitForSeconds(0.5f);
        gameManager.NextQuestion();
    }

    public void ResetAnswerFlag()
    {
        hasAnswered = false;
    }
}
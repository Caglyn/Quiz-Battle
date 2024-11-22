using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AnswerValidator : MonoBehaviour
{
    [Header("Player Configuration")]
    [SerializeField] private bool isPlayer1; // Check for Player 1, uncheck for Player 2
    [SerializeField] private GameManager gameManager; // Reference to the GameManager script

    private bool hasAnswered = false; // Flag to track if the player has already answered
    
    [SerializeField] private int correctAnswerPoint = 10;
    [SerializeField] private int wrongAnswerPoint = -10;

    public void ValidateAnswer(Button clickedButton)
    {
        if (hasAnswered)
            return;

        hasAnswered = true;

        string selectedAnswer = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        Debug.Log("Player " + (isPlayer1 ? "1" : "2") + " selected: " + selectedAnswer);

        if (selectedAnswer == gameManager.GetCorrectAnswer())
        {
            Debug.Log("Correct answer!");

            if (isPlayer1)
                gameManager.UpdatePlayerScore(1, correctAnswerPoint);
            else
                gameManager.UpdatePlayerScore(2, correctAnswerPoint);
        }
        else
        {
            if (isPlayer1)
                gameManager.UpdatePlayerScore(1, wrongAnswerPoint);
            else
                gameManager.UpdatePlayerScore(2, wrongAnswerPoint);
        }

        // FOR SOME REASON, AFTER THE LAST QUESTION, PLAYERS CAN STILL ANSWER, NEEDS FIXING!!!
        // SOMETHING TO DO WITH GAMEMANAGER'S NEXT QUESTIN METHOD AND 48-49 LINES OF THIS

        // Optionally, disable buttons after an answer is selected
        gameManager.DisablePlayerButtons();
        gameManager.NextQuestion();
    }

    public void ResetAnswerFlag() // Method to reset the flag for the next question
    {
        hasAnswered = false;
    }
}

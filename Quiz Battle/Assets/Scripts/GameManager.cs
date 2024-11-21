using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private APIManager apiManager;

    [Header("Player 1 UI Elements")]
    [SerializeField] private TextMeshProUGUI player1QuestionText;
    [SerializeField] private TextMeshProUGUI[] player1ChoiceTexts; // Choices for Player 1

    [Header("Player 2 UI Elements")]
    [SerializeField] private TextMeshProUGUI player2QuestionText;
    [SerializeField] private TextMeshProUGUI[] player2ChoiceTexts; // Choices for Player 2
    
    private void Start()
    {
        if (apiManager == null)
        {
            apiManager = FindObjectOfType<APIManager>();
            Debug.Log("APIManager not set in the inspector. Trying to find one in the scene.");
        }
    }

    private void OnQuestionSuccess(string jsonResponse)
    {
        Debug.Log("API Response: " + jsonResponse);

        var chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);
        if (chatResponse.choices != null && chatResponse.choices.Length > 0)
        {
            string content = chatResponse.choices[0].message.content.Trim();
            var (question, choices) = ParseQuestionAndChoices(content);

            // Assign the question and choices to Player 1's UI
            player1QuestionText.text = question;
            AssignChoicesToUI(player1ChoiceTexts, choices);

            // Assign the same question and choices to Player 2's UI
            player2QuestionText.text = question;
            AssignChoicesToUI(player2ChoiceTexts, choices);
        }
        else
        {
            HandleError("Failed to load question.");
        }
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

    private (string question, string[] choices) ParseQuestionAndChoices(string content)
    {
        // Split the content into lines
        string[] lines = content.Split(new[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        string question = null;
        var choices = new List<string>();

        foreach (var line in lines)
        {
            string trimmedLine = line.Trim();

            // Skip "Options:" line
            if (trimmedLine.Equals("Options:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Handle "Question:" prefix
            if (trimmedLine.StartsWith("Question:", StringComparison.OrdinalIgnoreCase))
            {
                question = trimmedLine.Substring("Question:".Length).Trim(); // Extract text after "Question:"
            }
            else
            {
                // Add remaining lines as choices
                choices.Add(trimmedLine);
            }
        }

        // Ensure we have a valid question
        question ??= "Failed to parse question.";

        return (question, choices.ToArray());
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

    public void GetQuestionFromAPI(string prompt)
    {
        StartCoroutine(apiManager.GetQuestionFromAPI(prompt, OnQuestionSuccess, HandleError));
    }

    [System.Serializable]
    private class ChatResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class APIManager : MonoBehaviour
{
    private string apiKey = "api_key";
    private const string apiUrl = "https://api.openai.com/v1/chat/completions";

    public IEnumerator GetQuestionsFromAPI(string prompt, Action<List<string>> onSuccess, Action<string> onError)
    {
       // string requestBody = "{\"model\":\"gpt-4\",\"messages\":[{\"role\":\"system\",\"content\":\"You are a helpful assistant for a quiz game.\"},{\"role\":\"user\",\"content\":\"" + prompt + "\"}],\"max_tokens\":1500,\"temperature\":0.7}";
        string requestBody = JsonConvert.SerializeObject(new
        {
            model = "gpt-3.5-turbo-0125",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant for a quiz game." },
                new { role = "user", content = prompt }
            },
            max_tokens = 1000,
            temperature = 0.7
        });

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the entire response into a list of question blocks
                List<string> questions = ParseQuestions(request.downloadHandler.text);
                onSuccess?.Invoke(questions);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);  // Detailed response log
                onError?.Invoke(request.error);
            }
        }
    }

    // Method to split the fetched response into individual questions
    private List<string> ParseQuestions(string jsonResponse)
    {
        List<string> questionList = new List<string>();
        var chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);
        if (chatResponse.choices != null && chatResponse.choices.Length > 0)
        {
            string content = chatResponse.choices[0].message.content.Trim();
            string[] questionBlocks = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            questionList.AddRange(questionBlocks);
        }
        return questionList;
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

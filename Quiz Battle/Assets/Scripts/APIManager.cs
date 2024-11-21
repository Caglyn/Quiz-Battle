using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class APIManager : MonoBehaviour
{
    private string apiKey = "api_key";
    private const string apiUrl = "https://api.openai.com/v1/chat/completions";

    public IEnumerator GetQuestionFromAPI(string prompt, Action<string> onSuccess, Action<string> onError)
    {
        string requestBody = "{\"model\":\"gpt-4\",\"messages\":[{\"role\":\"system\",\"content\":\"You are a helpful assistant for a quiz game.\"},{\"role\":\"user\",\"content\":\"" + prompt + "\"}],\"max_tokens\":150,\"temperature\":0.7}";

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
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}

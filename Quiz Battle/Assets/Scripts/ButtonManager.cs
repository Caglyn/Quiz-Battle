using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    private Scene scene;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TextMeshProUGUI questionText;

    private string prompt = "Generate a multiple-choice question about science with 4 options and specify the correct answer.";
    private string apiKey = "API_KEY";

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        tutorialPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        questionText.gameObject.SetActive(false);
    }

    public void OnClickPlayButton()
    {
        Debug.Log("Play Button Clicked");
        mainMenuPanel.SetActive(false);
        questionText.gameObject.SetActive(true);
        StartCoroutine(GetQuestionFromAPI(prompt));
    }

    public void OnClickExitButton()
    {
        Debug.Log("Exit Button Clicked");
        Application.Quit();
    }

    public void OnClickTutorialButton()
    {
        Debug.Log("Tutorial Button Clicked");
        tutorialPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void OnClickBackButton()
    {
        Debug.Log("Back Button Clicked");
        tutorialPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private IEnumerator GetQuestionFromAPI(string prompt)
    {
        string url = "https://api.openai.com/v1/chat/completions";
        string requestBody = "{\"model\":\"gpt-4\",\"messages\":[{\"role\":\"system\",\"content\":\"You are a helpful assistant for a quiz game.\"},{\"role\":\"user\",\"content\":\"" + prompt + "\"}],\"max_tokens\":150,\"temperature\":0.7}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            string response = ExtractResponse(request.downloadHandler.text);
            questionText.text = response; // Display the generated question
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Details: " + request.downloadHandler.text);
            questionText.text = "Failed to load question.";
        }
    }


    private string ExtractResponse(string jsonResponse)
    {
        // Deserialize the JSON response
        var chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);
        if (chatResponse.choices != null && chatResponse.choices.Length > 0)
        {
            return chatResponse.choices[0].message.content.Trim();
        }
        return "No response from API.";
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

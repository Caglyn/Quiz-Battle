using UnityEngine;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject categoryPanel;
    [SerializeField] private GameObject gamePanel;
    
    [SerializeField] private APIManager apiManager;
    [SerializeField] private GameManager gameManager;
    
    private string prompt;

    private void Start()
    {
        categoryPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        if (apiManager == null)
        {
            apiManager = FindObjectOfType<APIManager>();
            Debug.Log("APIManager not set in the inspector. Trying to find one in the scene.");
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            Debug.Log("GameManager not set in the inspector. Trying to find one in the scene.");
        }
    }

    public void OnClickPlayButton()
    {
        Debug.Log("Play Button Clicked");
        mainMenuPanel.SetActive(false);
        categoryPanel.SetActive(true);
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
        if (categoryPanel.activeSelf)
        {
            categoryPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else if (tutorialPanel.activeSelf)
        {
            tutorialPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }

    public void OnClickCategoryButton(string category)
    {
        Debug.Log("Category Button Clicked: " + category);
        categoryPanel.SetActive(false);

        prompt = "Generate a multiple-choice question about " + category + " with 4 options and specify the correct answer.";
        gameManager.GetQuestionFromAPI(prompt);
        gamePanel.SetActive(true);
    }
}
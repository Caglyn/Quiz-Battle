using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject categoryPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject controlsPanel;
    
    [Header("Manager References")]
    [SerializeField] private APIManager apiManager;
    [SerializeField] private GameManager gameManager;
    
    [Header("Prompt Settings")]
    [SerializeField] private string prompt;
    [SerializeField] private int desiredQuestionCount = 6;

    bool isGameActive;

    private void Start()
    {
        categoryPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        controlsPanel.SetActive(false);
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
    
    private void Update()
    {
        isGameActive = pausePanel.activeSelf;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGameActive = !isGameActive;
            
            if (isGameActive == true)
            {
                OnClickPauseButton();
            }
            else if (isGameActive == false)
            {
                OnClickResumeButton();
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (gamePanel.activeSelf || pausePanel.activeSelf)
            {
                OnClickRetryButton();
            }
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
        else if (gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(false);
            gamePanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else if (controlsPanel.activeSelf)
        {
            controlsPanel.SetActive(false);
            tutorialPanel.SetActive(true);
        }
        else if (pausePanel.activeSelf)
        {
            pausePanel.SetActive(false);
            gamePanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }

    public void OnClickForwardButton()
    {
        Debug.Log("Forward Button Clicked");
        tutorialPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void OnClickCategoryButton(string category)
    {
        Debug.Log("Category Button Clicked: " + category);
        categoryPanel.SetActive(false);

        prompt = $"Generate {desiredQuestionCount} multiple-choice questions about {category}. Each question should be formatted as follows:\n" +
         "Question: [Your question here]\n" +
         "A) [Option 1]\n" +
         "B) [Option 2]\n" +
         "C) [Option 3]\n" +
         "D) [Option 4]\n" +
         "Correct Answer: [Correct option letter]\n\n" +
         "Ensure each question follows this structure exactly, with no additional text or explanations.";

        gameManager.GetQuestionsFromAPI(prompt);
        gamePanel.SetActive(true);
        gameManager.ResetGame();
    }

    public void OnClickPauseButton()
    {
        Debug.Log("Pause Button Clicked");
        gameManager.PauseGame();
        pausePanel.SetActive(true);
    }

    public void OnClickResumeButton()
    {
        Debug.Log("Resume Button Clicked");
        gameManager.ResumeGame();
        pausePanel.SetActive(false);
    }

    public void OnClickRetryButton()
    {
        Debug.Log("Retry Button Clicked");
        pausePanel.SetActive(false);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        categoryPanel.SetActive(true);
    }
}
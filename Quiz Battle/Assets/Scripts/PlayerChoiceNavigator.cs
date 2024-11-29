using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerChoiceNavigator : MonoBehaviour
{
    [Header("Player Configuration")]
    [SerializeField] private bool isPlayer1; // Check for Player 1, uncheck for Player 2
    [SerializeField] private Button[] choiceButtons; // Assign the choice buttons in the inspector for each player

    private int currentIndex = 0; // Current selected button index
    private GameInputActions inputActions; // Input Actions object

    private Color defaultColor = Color.white;  // Default button color
    private Color selectedColor = Color.yellow; // Highlighted color for selection

    private AudioManager audioManager;

    private void Awake()
    {
        inputActions = new GameInputActions();

        if (isPlayer1)
        {
            inputActions.Player1.Move.performed += OnMove;
            inputActions.Player1.Select.performed += OnSelect;
        }
        else
        {
            inputActions.Player2.Move.performed += OnMove;
            inputActions.Player2.Select.performed += OnSelect;
        }

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnEnable()
    {
        if (isPlayer1)
        {
            inputActions.Player1.Enable();
        }
        else
        {
            inputActions.Player2.Enable();
        }

        HighlightButton(currentIndex); // Highlight the initial button
    }

    private void OnDisable()
    {
        if (isPlayer1)
        {
            inputActions.Player1.Disable();
        }
        else
        {
            inputActions.Player2.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Read the raw input as a float to detect the direction
        float inputY = context.ReadValue<Vector2>().y;

        // Only trigger movement when the key is pressed down, not held
        if (Mathf.Approximately(inputY, 0)) return; // Ignore zero input (no movement)

        // Determine direction and move selection
        int direction = inputY > 0 ? -1 : 1;
        MoveSelection(direction);

        // Play sound effect once per button press
        audioManager.PlayNavigateSound();
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        choiceButtons[currentIndex].onClick.Invoke(); // Trigger the button click
        audioManager.PlaySelectSound(); // Play selection sound
    }

    private void MoveSelection(int direction)
    {
        // Remove highlight from the current button
        SetButtonColor(choiceButtons[currentIndex], defaultColor);

        // Update the index with wrap-around logic
        currentIndex = (currentIndex + direction + choiceButtons.Length) % choiceButtons.Length;

        // Highlight the new button
        HighlightButton(currentIndex);
    }

    private void HighlightButton(int index)
    {
        SetButtonColor(choiceButtons[index], selectedColor);
    }

    private void SetButtonColor(Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        button.colors = colors;
    }
}
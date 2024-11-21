using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerChoiceNavigator : MonoBehaviour
{
    [Header("Player Configuration")]
    [SerializeField] private bool isPlayer1; // Check for Player 1, uncheck for Player 2
    [SerializeField] private Button[] choiceButtons; // Assign the choice buttons in the inspector

    [Header("Input Settings")]
    private GameInputActions inputActions; // Input Actions

    private int currentIndex = 0; // Current selected button index

    private void Awake()
    {
        inputActions = new GameInputActions(); // Initialize the input actions
Debug.Log("OnSelect callback registered for Player " + (isPlayer1 ? "1" : "2"));

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
        Vector2 input = context.ReadValue<Vector2>();

        if (input.y > 0) // Up
        {
            MoveSelection(-1);
        }
        else if (input.y < 0) // Down
        {
            MoveSelection(1);
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        // Trigger the currently selected button's click event
        choiceButtons[currentIndex].onClick.Invoke();
    }

    private void MoveSelection(int direction)
    {
        // Deselect the current button
        choiceButtons[currentIndex].OnDeselect(null);

        // Update the index
        currentIndex += direction;

        // Wrap around the index
        if (currentIndex < 0)
        {
            currentIndex = choiceButtons.Length - 1;
        }
        else if (currentIndex >= choiceButtons.Length)
        {
            currentIndex = 0;
        }

        // Highlight the new button
        choiceButtons[currentIndex].Select();
    }
}

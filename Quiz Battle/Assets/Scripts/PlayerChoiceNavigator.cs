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
        float inputY = context.ReadValue<Vector2>().y;

        if (inputY > 0)
        {
            MoveSelection(-1); // Move up
        }
        else if (inputY < 0)
        {
            MoveSelection(1); // Move down
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        choiceButtons[currentIndex].onClick.Invoke(); // Trigger the button click
    }

    private void MoveSelection(int direction)
    {
        choiceButtons[currentIndex].OnDeselect(null); // Deselect current button
        currentIndex = (currentIndex + direction + choiceButtons.Length) % choiceButtons.Length; // Update index
        choiceButtons[currentIndex].Select(); // Select new button
    }
}

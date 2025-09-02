using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public PlayerInput Inputs { get; private set; }
 
    public const string INPUT_GAME_MOUSE_POSITION = "Look";
    // Left mouse button
    public const string INPUT_GAME_LMB_Click = "LMB Click";
    public const string INPUT_GAME_LMB_HOLD = "LMB Hold";
    public const string INPUT_GAME_LMB_Double_Click = "LMB Double Click";
    // Right mouse button
    public const string INPUT_GAME_RMB_Click = "RMB Click";
    // Keyboard
    public const string INPUT_GAME_SHIFT = "Shift";
    public const string INPUT_GAME_MOVE = "Move";
    public const string INPUT_TOGGLE_AGGRESSIVE_APPROACH_COMMAND = "Toggle Aggressive Approach Command";

    public bool isMouseOverGameObject;
    private void Awake()
    {
        Instance = this;
        Inputs = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            isMouseOverGameObject = true;
        else isMouseOverGameObject = false;
    }
}

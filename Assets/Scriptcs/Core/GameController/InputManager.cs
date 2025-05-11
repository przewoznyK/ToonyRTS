using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private TeamColorEnum myTeamColor;

    public PlayerInput Inputs { get; private set; }
 
    public const string INPUT_GAME_MOUSE_POSITION = "Look";
    // Left mouse button
    public const string INPUT_GAME_LPM_Click = "LPM Click";
    public const string INPUT_GAME_LPM_HOLD = "LPM Hold";
    public const string INPUT_GAME_LPM_Double_Click = "LPM Double Click";
    // Right mouse button
    public const string INPUT_GAME_RPM_Click = "RPM Click";
    // Keyboard
    public const string INPUT_GAME_SHIFT = "Shift";

    private void Awake()
    {
        Inputs = GetComponent<PlayerInput>();
    }
}

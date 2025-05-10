using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private TeamColorEnum myTeamColor;

    public PlayerInput Inputs { get; private set; }

    public const string INPUT_GAME_MOUSE_POSITION = "Look";
    public const string INPUT_GAME_LPM_Click = "LPM Click";
    public const string INPUT_GAME_LPM_Move = "LPM Move";
    public const string INPUT_GAME_LPM_Double_Click = "LPM Double Click";

    private void Awake()
    {
        Debug.Log("Input");

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Inputs = GetComponent<PlayerInput>();
    }
}

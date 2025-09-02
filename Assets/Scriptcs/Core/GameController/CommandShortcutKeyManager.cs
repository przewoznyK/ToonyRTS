using UnityEngine;
using UnityEngine.InputSystem;

public class CommandShortcutKeyManager : MonoBehaviour
{
    CommandPanelUI commandPanelUI;

    InputAction toggleAggressiveApproachCommandAction;

    private bool isInitialized;
    public void Init(InputManager inputManager, CommandPanelUI commandPanelUI)
    {
        this.commandPanelUI = commandPanelUI;

        toggleAggressiveApproachCommandAction = inputManager.Inputs.actions[InputManager.INPUT_TOGGLE_AGGRESSIVE_APPROACH_COMMAND];
        toggleAggressiveApproachCommandAction.performed += ToggleAggressiveApproachCommandAction;
        isInitialized = true;
    }

    private void OnEnable()
    {
        if(isInitialized)
            toggleAggressiveApproachCommandAction.performed += ToggleAggressiveApproachCommandAction;
    }

    private void OnDisable()
    {
        toggleAggressiveApproachCommandAction.performed -= ToggleAggressiveApproachCommandAction;
    }
    void ToggleAggressiveApproachCommandAction(InputAction.CallbackContext contex)
    {
        commandPanelUI.ToggleAggresiveApproachButton();
    }
}

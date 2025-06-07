using UnityEngine;
using UnityEngine.InputSystem;

public class ManageSelectionUnits : MonoBehaviour
{
    InputManager inputManager;
    ControlledUnits controlledUnits;

    private InputAction RMPClickAction;

    private bool initialized = false;
    internal void Init(InputManager inputManager, ControlledUnits controlledUnits)
    {
        this.inputManager = inputManager;
        this.controlledUnits = controlledUnits;

        RMPClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];
        RMPClickAction.performed += OnRpmClick;

        initialized = true;
    }

    private void OnRpmClick(InputAction.CallbackContext ctx)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            foreach (var unit in controlledUnits.selectedUnits)
            {
                unit.PlayerRightMouseButtonCommand(hit);
            }  
        }   
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class ManageSelectionUnits : MonoBehaviour
{
    InputManager inputManager;
    ControlledUnits controlledUnits;

    private InputAction RMBClickAction;
    private InputAction ShiftClickAction;
    internal void Init(InputManager inputManager, ControlledUnits controlledUnits)
    {
        this.inputManager = inputManager;
        this.controlledUnits = controlledUnits;

        RMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];
        ShiftClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_SHIFT];
        RMBClickAction.performed += OnRpmClick;
    }

    private void OnRpmClick(InputAction.CallbackContext ctx)
    {
        bool isShiftPressed = ShiftClickAction.ReadValue<float>() > 0f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            foreach (var unit in controlledUnits.selectedUnits)
            {
                unit.PlayerRightMouseButtonCommand(hit, isShiftPressed);
            }  
        }   
    }
}

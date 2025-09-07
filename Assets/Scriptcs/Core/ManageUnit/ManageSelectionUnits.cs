using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ManageSelectionUnits : MonoBehaviour
{
    InputManager inputManager;
    ControlledUnits controlledUnits;

    private InputAction RMBClickAction;
    private InputAction LMBClickAction;
    private InputAction ShiftClickAction;

    [SerializeField] private LayerMask ignoreLayerMask;
    private bool initialized;

    //internal void Init(InputManager inputManager, ControlledUnits controlledUnits)
    //{
    //    this.inputManager = inputManager;
    //    this.controlledUnits = controlledUnits;


    //    RMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];
    //    LMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_LMB_Click];
    //    ShiftClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_SHIFT];

    //    RMBClickAction.performed += OnRMBClick;
    //    LMBClickAction.performed += OnLMBClick;

    //    initialized = true;
    //}
    //private void OnEnable()
    //{
    //    if(initialized)
    //    {
    //        RMBClickAction.performed += OnRMBClick;
    //        LMBClickAction.performed += OnLMBClick;
    //    }
    //}

    //private void OnDisable()
    //{
    //    RMBClickAction.performed -= OnRMBClick;
    //    LMBClickAction.performed -= OnLMBClick;
    //}
    //private void OnRMBClick(InputAction.CallbackContext ctx)
    //{
    //    if (InputManager.Instance.isMouseOverGameObject)
    //        return;

    //    bool isShiftPressed = ShiftClickAction.ReadValue<float>() > 0f;

    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreLayerMask))
    //    {
    //        foreach (var unit in controlledUnits.selectedUnits)
    //        {
    //            unit.PlayerRightMouseButtonCommand(hit, isShiftPressed);
    //        }  
    //    }   
    //}

    //private void OnLMBClick(InputAction.CallbackContext ctx)
    //{
    //    if (InputManager.Instance.isMouseOverGameObject)
    //        return;

    //    bool isShiftPressed = ShiftClickAction.ReadValue<float>() > 0f;

    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out RaycastHit hit, 100f))
    //    {
    //        foreach (var unit in controlledUnits.selectedUnits)
    //        {
    //            unit.PlayerLeftMouseButtonCommand(hit, isShiftPressed);
    //        }
    //    }
    //}
}

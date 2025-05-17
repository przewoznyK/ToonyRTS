using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManageSelectionUnits : MonoBehaviour
{
    InputManager inputManager;
    ActiveUnits activeUnits;

    private InputAction rmbClickAction;

    private bool initialized = false;
    internal void Init(InputManager inputManager, ActiveUnits activeUnits)
    {
        this.inputManager = inputManager;
        this.activeUnits = activeUnits;

        rmbClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RPM_Click];


        rmbClickAction.performed += OnRpmClick;

        initialized = true;
    }

    private void OnRpmClick(InputAction.CallbackContext ctx)
    {
        
        Debug.Log(activeUnits);
    }
}

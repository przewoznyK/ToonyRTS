using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManageSelectionUnits : MonoBehaviour
{
    InputManager inputManager;
    ActiveUnits activeUnits;

    private InputAction rpmClick;

    private bool initialized = false;
    internal void Init(InputManager inputManager, ActiveUnits activeUnits)
    {
        this.inputManager = inputManager;
        this.activeUnits = activeUnits;

        rpmClick = inputManager.Inputs.actions[InputManager.INPUT_GAME_RPM_Click];


        rpmClick.performed += OnRpmClick;

        initialized = true;
    }

    private void OnRpmClick(InputAction.CallbackContext ctx)
    {
        
        Debug.Log(activeUnits);
    }
}

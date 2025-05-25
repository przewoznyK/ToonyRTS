using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManageSelectionUnits : MonoBehaviour
{
    InputManager inputManager;
    ActiveUnits activeUnits;

    private InputAction RMPClickAction;

    private bool initialized = false;
    internal void Init(InputManager inputManager, ActiveUnits activeUnits)
    {
        this.inputManager = inputManager;
        this.activeUnits = activeUnits;

        RMPClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];


        RMPClickAction.performed += OnRpmClick;

        initialized = true;
    }

    private void OnRpmClick(InputAction.CallbackContext ctx)
    {
        
        Debug.Log(activeUnits);
    }
}

using System;
using TMPro;
using UnityEngine;

public class SelectionInfoUI : MonoBehaviour
{
    ControlledUnits controlledUnits;
    TextMeshProUGUI unitCountInfo;

    private bool initialized = false;
    internal void Init(ControlledUnits controlledUnits)
    {
        this.controlledUnits = controlledUnits;
        unitCountInfo = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        initialized = true;
        this.gameObject.SetActive(false);
        
    }

    private void OnEnable()
    {
        if (initialized == false)
            return;
        unitCountInfo.text = controlledUnits.GetUnitsSelectedCount().ToString();
    }
}

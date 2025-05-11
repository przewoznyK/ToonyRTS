using System;
using TMPro;
using UnityEngine;

public class SelectionInfoUI : MonoBehaviour
{
    ActiveUnits activeUnits;
    TextMeshProUGUI unitCountInfo;

    private bool initialized = false;
    internal void Init(ActiveUnits activeUnits)
    {
        this.activeUnits = activeUnits;
        unitCountInfo = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        initialized = true;
        this.gameObject.SetActive(false);
        
    }

    private void OnEnable()
    {
        if (initialized == false)
            return;
        unitCountInfo.text = activeUnits.GetUnitsCount().ToString();
    }
}

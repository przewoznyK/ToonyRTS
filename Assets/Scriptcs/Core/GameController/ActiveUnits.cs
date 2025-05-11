using System.Collections.Generic;
using UnityEngine;

public class ActiveUnits
{
    public List<Unit> unitsSelected = new();

    public void AddUnit(Unit unit)
    {
        unitsSelected.Add(unit);
        Debug.Log(unitsSelected.Count);
    }

    public List<Unit> TakeUnitList()
    {
        return unitsSelected;
    }

    public int GetUnitsCount()
    {
        return unitsSelected.Count;
    }

    public void ClearUnitsList()
    {
        unitsSelected.Clear();
    }
}

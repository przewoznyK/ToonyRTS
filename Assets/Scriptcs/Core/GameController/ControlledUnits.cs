using System.Collections.Generic;
using System.Diagnostics;

public class ControlledUnits
{
    public List<Unit> allUnits = new();
    public List<Unit> selectedUnits = new();

    // ALL PLAYER UNITS 
    public void AddToAllUnits(Unit unit)
    {
        allUnits.Add(unit);
    }

    public void DeleteFromAllUnits(Unit unit)
    {
        if (unit == null) return;

        allUnits.Remove(unit);
    }
    public List<Unit> GetAllUnitList()
    {
        return allUnits;
    }

    public int GetAllUnitsCount()
    {
        return allUnits.Count;
    }


    // ACTIVE UNITS
    public void AddToSelectedUnits(Unit unit)
    {
        selectedUnits.Add(unit);
    }
    public void DeleteFromSelectedUnits(Unit unit)
    {
        if (unit == null) return;

        selectedUnits.Remove(unit);
    }
    public List<Unit> TakeSelectedUnitList()
    {
        return selectedUnits;
    }

    public int GetUnitsSelectedCount()
    {
        return selectedUnits.Count;
    }

    public void ClearSelectedUnitsList()
    {
        selectedUnits.Clear();
    }

    public void RemoveUnit(Unit unit)
    {
        DeleteFromAllUnits(unit);
        DeleteFromSelectedUnits(unit);
        EntitiesOnMapDatabase.Instance.RemoveUnitFromList(unit.teamColor, unit);
    }
}
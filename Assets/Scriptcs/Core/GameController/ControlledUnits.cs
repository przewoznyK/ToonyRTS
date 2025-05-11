using System.Collections.Generic;

public class ControlledUnits
{
    public List<Unit> allUnits = new();

    public void AddUnit(Unit unit)
    {
        allUnits.Add(unit);
    }

    public List<Unit> TakeUnitList()
    {
        return allUnits;
    }

    public int GetUnitsCount()
    {
        return allUnits.Count;
    }

    public void ClearUnitsList()
    {
        allUnits.Clear();
    }
}
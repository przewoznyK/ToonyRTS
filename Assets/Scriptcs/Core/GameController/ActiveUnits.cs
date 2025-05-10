using System.Collections.Generic;
using UnityEngine;

public class ActiveUnits
{
    public List<Unit> unitsSelected = new();
    public ActiveUnits()
    {
        
    }

    public void AddUnit(Unit unit)
    {
        unitsSelected.Add(unit);
        Debug.Log("DODA£EM UNITA");
    }
}

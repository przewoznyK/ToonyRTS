using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledBuildings : MonoBehaviour
{
    public List<Building> allBuildings = new();
    public List<Building> selectedBuildings = new();

    // ALL PLAYER BUILDINGS
    public void AddToAllBuildings(Building building)
    {
        allBuildings.Add(building);
    }

    public void DeleteFromAllBuildings(Building building)
    {
        if (building == null) return;

        allBuildings.Remove(building);
    }
    public List<Building> GetAllBuildingsList()
    {
        return allBuildings;
    }

    public int GetAllBuildingsCount()
    {
        return allBuildings.Count;
    }


    // ACTIVE UNITS
    public void AddToSelectedBuildings(Building building)
    {
        selectedBuildings.Add(building);
    }
    public void DeleteFromSelectedBuildings(Building building)
    {
        if (building == null) return;

        selectedBuildings.Remove(building);
    }
    public List<Building> TakeSelectedBuildingsList()
    {
        return selectedBuildings;
    }

    public int GetBuildingsSelectedCount()
    {
        return selectedBuildings.Count;
    }

    public void ClearSelectedBuildingsList()
    {
        selectedBuildings.Clear();
    }

    public void RemoveUnit(Building building)
    {
        DeleteFromAllBuildings(building);
        DeleteFromSelectedBuildings(building);
    }
}

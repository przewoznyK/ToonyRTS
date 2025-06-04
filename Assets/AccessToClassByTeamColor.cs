using System.Collections.Generic;
using UnityEngine;

public class AccessToClassByTeamColor : MonoBehaviour
{
    public static AccessToClassByTeamColor instance;
    Dictionary<TeamColorEnum, PlayerResources> PlayersResourcesManagerGlobalList = new();
    Dictionary<TeamColorEnum, ControlledUnits> ControlledUnitsManagerGlobalList = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void AddPlayerResourceManagerToGlobalList(TeamColorEnum teamColorEnum,PlayerResources playerResources)
    {
        PlayersResourcesManagerGlobalList.Add(teamColorEnum, playerResources);
    }

    public PlayerResources GetPlayerResourcesManagerByTeamColor(TeamColorEnum teamColorEnum)
    {
        return PlayersResourcesManagerGlobalList[teamColorEnum];
    }
    public void AddControlledUnitsManagerToGlobalList(TeamColorEnum teamColorEnum, ControlledUnits controlledUnits)
    {
        ControlledUnitsManagerGlobalList.Add(teamColorEnum, controlledUnits);
    }

    public ControlledUnits GetControlledUnitsByTeamColor(TeamColorEnum teamColorEnum)
    {
        return ControlledUnitsManagerGlobalList[teamColorEnum];
    }
}

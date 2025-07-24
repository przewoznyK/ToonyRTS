using UnityEngine;

public class PlayerController
{

    public PlayerController(TeamColorEnum teamColor, AccessToClassByTeamColor accessToClassByTeamColor)
    {
        var teamProfile = new CreateTeamProfile(teamColor);

        
        var controlledUnits = new ControlledUnits();
        var playerControlledBuildings = new PlayerControlledBuildings();

        var playerResources = new PlayerResources(summaryPanelUI, commandPanelUI, 3000, 3000, 2000, 1000);

        accessToClassByTeamColor.AddPlayerResourceManagerToGlobalList(teamColor, playerResources);
        accessToClassByTeamColor.AddControlledUnitsManagerToGlobalList(teamColor, controlledUnits);
        accessToClassByTeamColor.AddControlledBuildingsManagerToGlobalList(teamColor, playerControlledBuildings);
        Debug.Log(teamProfile.tagName);
        

    }
}

using UnityEngine;

public class PlayerController
{

    public PlayerController(TeamColorEnum teamColor, AccessToClassByTeamColor accessToClassByTeamColor)
    {
        var teamProfile = new CreateTeamProfile(teamColor);
        var controlledUnits = new ControlledUnits();

        accessToClassByTeamColor.AddControlledUnitsManagerToGlobalList(teamColor, controlledUnits);

        Debug.Log(teamProfile.tagName);
        

    }
}

using UnityEngine;

public class PlayerController
{

    public PlayerController(TeamColorEnum teamColorEnum)
    {
        var teamProfile = new CreateTeamProfile(teamColorEnum);
        
        var activeUnits = new ActiveUnits();
        var activeClickableObject = new ActiveClickableObject(activeUnits);
        
        Debug.Log(teamProfile.tagName);
        

    }
}

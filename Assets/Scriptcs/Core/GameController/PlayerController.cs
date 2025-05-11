using UnityEngine;

public class PlayerController
{

    public PlayerController(TeamColorEnum teamColorEnum)
    {
        var teamProfile = new CreateTeamProfile(teamColorEnum);
        

        
        Debug.Log(teamProfile.tagName);
        

    }
}

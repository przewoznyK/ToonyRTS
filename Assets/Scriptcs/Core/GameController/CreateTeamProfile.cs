using UnityEngine;

public class CreateTeamProfile
{
    public Material teamMaterial;
    public string tagName;
    public CreateTeamProfile(TeamColorEnum teamColorEnum)
    {
        tagName = teamColorEnum.ToString();
        teamMaterial = TeamColorDatabase.Instance.GetTeamMaterialColor(teamColorEnum);     
        
    }
}

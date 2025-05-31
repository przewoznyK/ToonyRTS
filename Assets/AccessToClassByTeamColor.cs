using System.Collections.Generic;
using UnityEngine;

public class AccessToClassByTeamColor : MonoBehaviour
{
    public static AccessToClassByTeamColor instance;
    Dictionary<TeamColorEnum, PlayerResources> PlayersResourcesManagerGlobalList = new();

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
}

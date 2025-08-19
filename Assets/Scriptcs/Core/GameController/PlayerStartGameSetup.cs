using System;
using UnityEngine;

public class PlayerStartGameSetup : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    internal void Init(PlayerResources playerResources, ConstructionPlacerSystem constructionPlacerSystem, GridDataNetwork gridData, TeamColorEnum teamColor, int xPosition, int zPosition)
    {
        ConstructionData currentConstructionData = new (buildingData, xPosition, zPosition, teamColor);
        constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);

        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(0, teamColor, xPosition -3, zPosition -3);
    }

}

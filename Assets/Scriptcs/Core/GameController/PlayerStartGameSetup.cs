using System;
using UnityEngine;

public class PlayerStartGameSetup : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    internal void Init(PlayerResources playerResources, ConstructionPlacerSystem constructionPlacerSystem, GridDataNetwork gridData, TeamColorEnum teamColor, int xPosition, int zPosition)
    {
        ConstructionData currentConstructionData = new(buildingData, xPosition, zPosition, teamColor);
        currentConstructionData.buildingData.buildingID = 0;
        constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
        
        //currentConstructionData.positionToOccupy = new Vector3Int(xPosition + 10, 0, zPosition + 10);
        //currentConstructionData.buildingData.buildingID = 1;
        //constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);

       // PlayerController.LocalPlayer.CmdSpawnUnitOnStart(0, teamColor, xPosition -3, zPosition -3);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -3, zPosition -4);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -3, zPosition -8);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -3, zPosition -12);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -6, zPosition -4);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -6, zPosition -8);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -6, zPosition -12);
     //   PlayerController.LocalPlayer.CmdSpawnUnitOnStart(2, teamColor, xPosition - 3, zPosition - 5);
      //  PlayerController.LocalPlayer.CmdSpawnUnitOnStart(3, teamColor, xPosition - 3, zPosition - 6);
    }

}

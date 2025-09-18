using Mirror;
using System;
using UnityEngine;

public class PlayerStartGameSetup : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    [SerializeField] private BuildingData treeResourceData;
    [SerializeField] private BuildingData stoneResourceData;
    [SerializeField] private BuildingData goldResourceData;
 //   [SerializeField] private NetworkIdentity treeResourcePrefab;
    internal void Init(PlayerResources playerResources, ConstructionPlacerSystem constructionPlacerSystem, GridDataNetwork gridData, TeamColorEnum teamColor, int xPosition, int zPosition)
    {
    
            ConstructionData currentConstructionData = new(buildingData, xPosition, zPosition, teamColor);
            currentConstructionData.buildingData.buildingID = 0;
           constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);

    //    ConstructionData currentConstructionData1 = new(buildingData, xPosition, zPosition, teamColor);
   //     currentConstructionData1.buildingData.buildingID = 0;
      //  constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData1);

        //currentConstructionData.positionToOccupy = new Vector3Int(xPosition + 10, 0, zPosition + 10);
        //currentConstructionData.buildingData.buildingID = 1;
        //constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
        //if(PlayerController.LocalPlayer.isServer)
        //{
        //  PlayerController.LocalPlayer.CmdSpawnUnitOnStart(0, teamColor, xPosition + 5, zPosition + 5);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition + 5, zPosition + 2);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition + 5, zPosition + 1);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition + 5, zPosition);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition + 5, zPosition - 1);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition + 5, zPosition - 2);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(0, teamColor, xPosition + 5, zPosition - 10);

        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition + 2);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition + 1);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition - 1);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition - 2);

        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 10, zPosition + 1);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 10, zPosition);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 10, zPosition - 1);

        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition + 2);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition + 1);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition - 1);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, xPosition + 15, zPosition - 2);

        GameManager.Instance.rtsCameraController.gameObject.transform.position = new Vector3(xPosition, 30, -20);

        int sizeX = 29;
        int sizeY = 29;
        for (int i = -sizeX; i < sizeY; i += 3)
        {
            for (int j = -sizeX; j < sizeY; j += 3)
            {
                if (i >= -5 && i <= 5)
                    continue;
                if (j >= -5 && j <= 5)
                    continue;
                    int borderThickness = 3;

                bool isBorder =
                    (i <= -30 + borderThickness || i >= 30 - borderThickness || 
                     j <= -30 + borderThickness || j >= 30 - borderThickness ||   
                     Mathf.Abs(i) <= 5 + borderThickness ||                       
                     Mathf.Abs(j) <= 5 + borderThickness);

                if (isBorder)
                {
                    currentConstructionData = new(treeResourceData, i + xPosition, j + zPosition, teamColor);
                    constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
                }
            }
        }
        currentConstructionData = new(stoneResourceData, (-sizeX / 2 - 5) + xPosition, (-sizeY / 2 - 5) + zPosition, teamColor);
        constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
        currentConstructionData = new(stoneResourceData, (-sizeX / 2 + 32) + xPosition, (-sizeY / 2 + 32) + zPosition, teamColor);
        constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
        currentConstructionData = new(goldResourceData, (-sizeX / 2 - 5) + xPosition, (-sizeY / 2 + 32) + zPosition, teamColor);
        constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
        currentConstructionData = new(goldResourceData, (-sizeX / 2 + 32) + xPosition, (-sizeY / 2 - 5) + zPosition, teamColor);
        constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
        //PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((-sizeX / 2 - 5) + xPosition, 0, (-sizeY / 2 - 5) + zPosition)); 
        //PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((sizeX / 2 + 5) + xPosition, 0, (-sizeY / 2 - 5) + zPosition)); 
        //PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((-sizeX / 2 - 5) + xPosition, 0, (sizeY / 2 + 5) + zPosition)); 
        //PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((sizeX / 2 + 5) + xPosition, 0, (sizeY / 2 + 5) + zPosition));


    }

}

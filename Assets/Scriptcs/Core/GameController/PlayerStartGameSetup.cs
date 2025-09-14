using Mirror;
using System;
using UnityEngine;

public class PlayerStartGameSetup : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
 //   [SerializeField] private NetworkIdentity treeResourcePrefab;
    internal void Init(PlayerResources playerResources, ConstructionPlacerSystem constructionPlacerSystem, GridDataNetwork gridData, TeamColorEnum teamColor, int xPosition, int zPosition)
    {
    
            ConstructionData currentConstructionData = new(buildingData, xPosition, zPosition, teamColor);
            currentConstructionData.buildingData.buildingID = 0;
            constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);

        //currentConstructionData.positionToOccupy = new Vector3Int(xPosition + 10, 0, zPosition + 10);
        //currentConstructionData.buildingData.buildingID = 1;
        //constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
        //if(PlayerController.LocalPlayer.isServer)
        //{
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(0, teamColor, xPosition + 5, zPosition + 5);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition + 2, zPosition);
        PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition + 4, zPosition);

        GameManager.Instance.rtsCameraController.gameObject.transform.position = new Vector3(xPosition, 30, -20);

        int sizeX = 29;
        int sizeY = 29;
        for (int i = -sizeX; i < sizeY; i += 3)
        {
            for (int j = -sizeX; j < sizeY; j += 3)
            {
                // pomijamy centralny obszar (zostaj¹ 4 kwadraty)
                if (i >= -5 && i <= 5)
                    continue;
                if (j >= -5 && j <= 5)
                    continue;
                    // szerokoœæ ramki (np. 3–6 jednostek gruboœci)
                    int borderThickness = 3;

                // sprawdzamy czy jesteœmy na krawêdzi kwadratu
                bool isBorder =
                    (i <= -30 + borderThickness || i >= 30 - borderThickness ||   // lewa/prawa granica ca³ej siatki
                     j <= -30 + borderThickness || j >= 30 - borderThickness ||   // dó³/góra ca³ej siatki
                     Mathf.Abs(i) <= 5 + borderThickness ||                       // wewnêtrzna granica przy œrodku
                     Mathf.Abs(j) <= 5 + borderThickness);

                if (isBorder)
                {
                    PlayerController.LocalPlayer.CmdSpawnResource(1, new Vector3(i + xPosition, 0, j + zPosition));
              
                }
            }
        }

        PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((-sizeX / 2 - 5) + xPosition, 0, (-sizeY / 2 - 5) + zPosition)); // lewy dó³
        PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((sizeX / 2 + 5) + xPosition, 0, (-sizeY / 2 - 5) + zPosition));  // prawy dó³
        PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((-sizeX / 2 - 5) + xPosition, 0, (sizeY / 2 + 5) + zPosition));  // lewy góra
        PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3((sizeX / 2 + 5) + xPosition, 0, (sizeY / 2 + 5) + zPosition));   // p
        //    PlayerController.LocalPlayer.CmdSpawnUnitOnStart(0, teamColor, xPosition - 3, zPosition - 6);
        //    PlayerController.LocalPlayer.CmdSpawnUnitOnStart(0, teamColor, xPosition - 3, zPosition - 9);
        //    PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition - 3, zPosition - 12);
        //    PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3(10, 0, 10));
        //}

        //currentConstructionData = new(buildingData, 10, 10, TeamColorEnum.Red);
        //currentConstructionData.buildingData.buildingID = 0;
        //constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);

        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, 12, 0);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, 72, 2);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, 72, 4);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, 72, 6);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, 72, 0);
        //PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, TeamColorEnum.Red, 72, 8);

        // PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3(12, 0, 10));
        //    PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3(14, 0, 10));
        //  PlayerController.LocalPlayer.CmdSpawnResource(0, new Vector3(16, 0, 10));
        //  GameManager.Instance.defaultMapGenerator.Init();
        //   PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -3, zPosition -8);
        //   PlayerController.LocalPlayer.CmdSpawnUnitOnStart(1, teamColor, xPosition -3, zPosition -12);
        //   PlayerController.LocalPlayer.CmdSpawnUnitOnStart(2, teamColor, xPosition -6, zPosition -4);
        //   PlayerController.LocalPlayer.CmdSpawnUnitOnStart(2, teamColor, xPosition -6, zPosition -8);
        //   PlayerController.LocalPlayer.CmdSpawnUnitOnStart(2, teamColor, xPosition -6, zPosition -12);
        //   PlayerController.LocalPlayer.CmdSpawnUnitOnStart(2, teamColor, xPosition - 3, zPosition - 5);
        //  PlayerController.LocalPlayer.CmdSpawnUnitOnStart(3, teamColor, xPosition - 3, zPosition - 6);




    }

}

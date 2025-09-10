using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPlacerSystem : NetworkBehaviour
{
    PlayerResources playerResources;
    ActiveClickableObject activeClickableObject;
    [SerializeField] private GameObject constructionRepresentationPrefab;
    [SerializeField] private InConstructionBuildingRepresentation constructionRepresentation;
    public void Init(PlayerResources playerResources, ActiveClickableObject activeClickableObject)
    {
        this.playerResources = playerResources;
        this.activeClickableObject = activeClickableObject;
    }
    internal void PlaceConstruction(PlayerResources playerResources, GridDataNetwork gridData, ConstructionData currentConstructionData)
    {
        List<Vector3Int> positionsToOccupy = gridData.UpdateGridDataInLocal(currentConstructionData.positionToOccupy, currentConstructionData.buildingData.size, 1, 1, currentConstructionData);
        if(currentConstructionData.createFromPreviewSystem == true)
        {
            // Pay Cost
            playerResources.SpendResources(currentConstructionData.buildingData.objectPrices);

            Vector3 position = currentConstructionData.positionToOccupy;
            Vector2 size = currentConstructionData.buildingData.size;

            List<Unit> selectedUnits = activeClickableObject.controlledUnits.TakeSelectedUnitList();
            PlayerController.LocalPlayer.CmdSpawnConstructionRepresentation(currentConstructionData.buildingData.buildingID, position, size, selectedUnits, currentConstructionData.teamColor, positionsToOccupy);
        }
        else
            PlayerController.LocalPlayer.CmdSpawnBuilding(currentConstructionData.buildingData.buildingID ,currentConstructionData.positionToOccupy, currentConstructionData.teamColor, positionsToOccupy);
    }
}

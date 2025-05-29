using System;
using UnityEngine;

public class ConstructionPlacerSystem : MonoBehaviour
{
    PlayerResources playerResources;
    ActiveClickableObject activeClickableObject;
    public void Init(PlayerResources playerResources, ActiveClickableObject activeClickableObject)
    {
        this.playerResources = playerResources;
        this.activeClickableObject = activeClickableObject;
    }
    internal void PlaceConstruction(GridData gridData, ConstructionData currentConstructionData)
    {
        Instantiate(currentConstructionData.buildingData.buildingPrefab, currentConstructionData.positionToOccupy, Quaternion.identity);
        gridData.AddObjectAt(currentConstructionData.positionToOccupy, currentConstructionData.buildingData.size, 1 ,1 );
        if(currentConstructionData.createFromPreviewSystem == true)
        {
            currentConstructionData.previewObjectToPlaceMeshRenderer.material = currentConstructionData.originalMaterial;
            playerResources.SpendResources(currentConstructionData.buildingData.objectPrices);

        }
    }
}

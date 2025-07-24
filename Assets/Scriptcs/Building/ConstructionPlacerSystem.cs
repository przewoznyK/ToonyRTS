using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPlacerSystem : MonoBehaviour
{
    ActiveClickableObject activeClickableObject;
    [SerializeField] private GameObject constructionRepresentationPrefab;
    [SerializeField] private InConstructionBuildingRepresentation constructionRepresentation;
    public void Init(ActiveClickableObject activeClickableObject)
    {
        this.activeClickableObject = activeClickableObject;
    }
    internal void PlaceConstruction(PlayerResources playerResources, GridData gridData, ConstructionData currentConstructionData)
    {
        
        gridData.AddObjectAt(currentConstructionData.positionToOccupy, currentConstructionData.buildingData.size, 1 ,1, currentConstructionData);
        if(currentConstructionData.createFromPreviewSystem == true)
        {
            // Set Material
            currentConstructionData.previewObjectToPlaceMeshRenderer.material = currentConstructionData.originalMaterial;
            // Pay Cost
            playerResources.SpendResources(currentConstructionData.buildingData.objectPrices);

            constructionRepresentation.SetFinishBuilding(currentConstructionData.buildingData.buildingPrefab, currentConstructionData.buildingData.positionToOccupy);
            GameObject constructionInstantiate = Instantiate(constructionRepresentationPrefab, currentConstructionData.positionToOccupy, Quaternion.identity);
           
            constructionInstantiate.transform.rotation = Quaternion.Euler(90, 0, 0);
            constructionInstantiate.transform.localScale = new Vector3(currentConstructionData.buildingData.size.x, currentConstructionData.buildingData.size.y, 1);
            constructionInstantiate.transform.localScale = new Vector3(currentConstructionData.buildingData.size.x, currentConstructionData.buildingData.size.y, 1);
           

            List<Unit> selectedUnits = activeClickableObject.controlledUnits.TakeSelectedUnitList();
            foreach (var unit in selectedUnits)
            {
                if(unit is GathererNew)
                {
                    GathererNew gatherer = unit as GathererNew;
                    gatherer.BuildConstruction(constructionInstantiate);
                }
            }
        }
        else
        {
            GameObject constructionInstantiate = Instantiate(currentConstructionData.buildingData.buildingPrefab, currentConstructionData.positionToOccupy, Quaternion.identity);
            constructionInstantiate.GetComponent<Building>().SetTeamColor(currentConstructionData.teamColor);
        }
    }
}

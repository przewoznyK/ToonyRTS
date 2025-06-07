using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPlacerSystem : MonoBehaviour
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
    internal void PlaceConstruction(GridData gridData, ConstructionData currentConstructionData)
    {
        
        gridData.AddObjectAt(currentConstructionData.positionToOccupy, currentConstructionData.buildingData.size, 1 ,1 );
        if(currentConstructionData.createFromPreviewSystem == true)
        {
            currentConstructionData.previewObjectToPlaceMeshRenderer.material = currentConstructionData.originalMaterial;
            playerResources.SpendResources(currentConstructionData.buildingData.objectPrices);

            constructionRepresentation.SetFinishBuilding(currentConstructionData.buildingData.buildingPrefab);
            GameObject constructionInstantiate = Instantiate(constructionRepresentationPrefab, currentConstructionData.positionToOccupy, Quaternion.identity);
          //  constructionInstantiate.GetComponent<Building>().SetTeamColor(currentConstructionData.teamColor);

            constructionInstantiate.transform.rotation = Quaternion.Euler(90, 0, 0);
            constructionInstantiate.transform.localScale = new Vector3(currentConstructionData.buildingData.size.x, currentConstructionData.buildingData.size.y, 1);
            constructionInstantiate.transform.localScale = new Vector3(currentConstructionData.buildingData.size.x, currentConstructionData.buildingData.size.y, 1);

            List<Unit> selectedUnits = activeClickableObject.controlledUnits.TakeSelectedUnitList();
            foreach (var unit in selectedUnits)
            {
                if(unit is Gatherer)
                {
                    Gatherer gatherer = unit as Gatherer;
                    gatherer.SetBuildingToBuild(constructionInstantiate);
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

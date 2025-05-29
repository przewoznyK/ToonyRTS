using System.Collections.Generic;
using UnityEngine;

public class ConstructionData
{
    public List<Unit> unitToBuildingConstruction;
    public BuildingData buildingData;
    public MeshRenderer previewObjectToPlaceMeshRenderer;
    public Material originalMaterial;
    public Vector3Int positionToOccupy;
    public bool createFromPreviewSystem;
    public ConstructionData(BuildingData buildingData, int xPosition, int zPosition)
    {
        this.buildingData = buildingData;
        this.createFromPreviewSystem = false;
        this.positionToOccupy = new Vector3Int(xPosition, 0, zPosition);
    }
    public ConstructionData(List<Unit> unitToBuildingConstruction, BuildingData buildingData, MeshRenderer previewObjectToPlaceMeshRenderer, Material originalMaterial)
    {
        this.unitToBuildingConstruction = unitToBuildingConstruction;
        this.buildingData = buildingData;
        this.previewObjectToPlaceMeshRenderer = previewObjectToPlaceMeshRenderer;
        this.originalMaterial = originalMaterial;
        this.createFromPreviewSystem = true;

    }

    public void SetPositionToOccupy(Vector3Int positionToOccupy)
    {
        this.positionToOccupy = positionToOccupy;
    }
}

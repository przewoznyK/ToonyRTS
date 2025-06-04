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
    public TeamColorEnum teamColor;
    public ConstructionData(BuildingData buildingData, int xPosition, int zPosition, TeamColorEnum teamColor)
    {
        this.buildingData = buildingData;
        this.createFromPreviewSystem = false;
        this.positionToOccupy = new Vector3Int(xPosition, 0, zPosition);
        this.teamColor = teamColor;
    }
    public ConstructionData(List<Unit> unitToBuildingConstruction, BuildingData buildingData, MeshRenderer previewObjectToPlaceMeshRenderer, Material originalMaterial, TeamColorEnum teamColor)
    {
        this.unitToBuildingConstruction = unitToBuildingConstruction;
        this.buildingData = buildingData;
        this.previewObjectToPlaceMeshRenderer = previewObjectToPlaceMeshRenderer;
        this.originalMaterial = originalMaterial;
        this.createFromPreviewSystem = true;
        this.teamColor = teamColor;
    }

    public void SetPositionToOccupy(Vector3Int positionToOccupy)
    {
        this.positionToOccupy = new Vector3Int(positionToOccupy.x, 1, positionToOccupy.z);
       
    }
}

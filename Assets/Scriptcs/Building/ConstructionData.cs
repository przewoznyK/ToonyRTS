using System.Collections.Generic;
using UnityEngine;

public class ConstructionData
{
    public List<Unit> unitToBuildingConstruction;
    public BuildingData buildingData;
    public Vector3Int positionToOccupy;
    public bool createFromPreviewSystem;
    public TeamColorEnum teamColor;

    public ConstructionData() { }
    public ConstructionData(BuildingData buildingData, int xPosition, int zPosition, TeamColorEnum teamColor)
    {
        this.buildingData = buildingData;
        this.createFromPreviewSystem = false;
        this.positionToOccupy = new Vector3Int(xPosition, 0, zPosition);
        this.teamColor = teamColor;
    }
    public ConstructionData(List<Unit> unitToBuildingConstruction, BuildingData buildingData, TeamColorEnum teamColor)
    {
        this.unitToBuildingConstruction = unitToBuildingConstruction;
        this.buildingData = buildingData;
        this.createFromPreviewSystem = true;
        this.teamColor = teamColor;
    }

    public void SetPositionToOccupy(Vector3Int positionToOccupy)
    {
        this.positionToOccupy = new Vector3Int(positionToOccupy.x, 0, positionToOccupy.z);
    }
}

using UnityEngine;

public class BuildConstructionTask : UnitTask
{
    public InConstructionBuildingRepresentation constructionBuildingRepresentation;
    public Vector3 constructionPosition { private set; get; }

    public BuildConstructionTask(GameObject construction)
    {
        unitTaskType = UnitTaskTypeEnum.BuildingConstruction;
        this.constructionPosition = construction.transform.position;
        this.constructionBuildingRepresentation = construction.GetComponent<InConstructionBuildingRepresentation>();
    }
}

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
        this.taskPosition = construction.transform.position;
    }
    public override void TakeVisualizationTask(GameObject flagGameObject)
    {
        this.flagGameObject = flagGameObject;
        this.taskPosition = flagGameObject.transform.position;
    }
    public override void EndTask()
    {
        if (flagGameObject != null)
        {
            flagGameObject.SetActive(false);
            flagGameObject = null;
        }
    }
}

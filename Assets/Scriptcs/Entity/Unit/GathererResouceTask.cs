using UnityEngine;

public class GathererResourceTask : UnitTask
{
    public GatherableResource gatherableResource;
    public ResourceTypesEnum currentResourceTypeGathering;

    public GathererResourceTask(GatherableResource resource)
    {
        unitTaskType = UnitTaskTypeEnum.GatherResource;
        this.targetTransform = resource.transform;
        this.taskPosition = resource.transform.position;
        gatherableResource = resource;
        currentResourceTypeGathering = resource.resourceType;
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
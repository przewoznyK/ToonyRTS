using UnityEngine;

public class GathererResourceTask : UnitTask
{
    public GatherableResource gatherableResource;
    public Transform targetTransform { private set; get; }
    public ResourceTypesEnum currentResourceTypeGathering;

    public GathererResourceTask(GatherableResource resource)
    {
        gatherableResource = resource;
        unitTaskType = UnitTaskTypeEnum.GatherResource;
        this.targetTransform = resource.transform;
        currentResourceTypeGathering = resource.resourceType;
    }
}
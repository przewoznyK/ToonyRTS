using UnityEngine;

public class GatherResourceTask : UnitTask
{
    public GatherableResource gatherableResource;
    public Transform targetTransform { private set; get; }
    public ResourceTypesEnum currentResourceTypeGathering;

    public GatherResourceTask(GatherableResource resource)
    {
        gatherableResource = resource;
        unitTaskType = UnitTaskTypeEnum.GatherResource;
        this.targetTransform = resource.transform;
        currentResourceTypeGathering = resource.resourceType;
    }
}

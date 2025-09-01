using UnityEngine;

public class AggressiveApproachTask : UnitTask
{
    public AggressiveApproachTask(Vector3 destinatedPosition)
    {
        unitTaskType = UnitTaskTypeEnum.AggressiveApproach;
        this.taskPosition = destinatedPosition;
    }
    public override void TakeVisualizationTask(GameObject flagGameObject)
    {
        this.flagGameObject = flagGameObject;
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

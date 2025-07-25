using UnityEngine;

public class GoToPositionTask : UnitTask
{
    public GoToPositionTask(Vector3 destinatedPosition)
    {
        unitTaskType = UnitTaskTypeEnum.GoToPosition;
        this.taskPosition = destinatedPosition;
    }
    public override void TakeVisualizationTask(GameObject flagGameObject)
    {
        this.flagGameObject = flagGameObject;
    }
    public override void EndTask()
    {
        if(flagGameObject != null)
        {
            flagGameObject.SetActive(false);
            flagGameObject = null;
        }
    }
}

using UnityEngine;

public class ReturnToStockpileTask : UnitTask
{
    public ReturnToStockpileTask(Vector3 destinatedPosition)
    {
        unitTaskType = UnitTaskTypeEnum.ReturnToStockpile;
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

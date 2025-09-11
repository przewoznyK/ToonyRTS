using UnityEngine;

public class ReturnToStockpileTask : UnitTask
{
    public Vector2 buildingSize;
    public ReturnToStockpileTask(Vector3 destinatedPosition, Vector2 buildingSize)
    {
        unitTaskType = UnitTaskTypeEnum.ReturnToStockpile;
        this.taskPosition = destinatedPosition;
        this.buildingSize = buildingSize;
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

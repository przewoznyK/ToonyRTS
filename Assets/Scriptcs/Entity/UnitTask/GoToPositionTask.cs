using UnityEngine;

public class GoToPositionTask : UnitTask
{
    public GoToPositionTask(Vector3 destinatedPosition)
    {
        unitTaskType = UnitTaskTypeEnum.GoToPosition;
        this.taskPosition = destinatedPosition;
    }
    public override void TakeVisulazationTask(GameObject flagGameObject)
    {
        Debug.Log("PRZYPISUJE" + flagGameObject == null);
        this.flagGameObject = flagGameObject;
        this.taskPosition = flagGameObject.transform.position;
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

using UnityEngine;

public class GoToPositionTask : UnitTask
{
    GameObject flagGameObject;
    public Vector3 destinatedPosition { private set; get; }

    public GoToPositionTask(Vector3 destinatedPosition)
    {
        unitTaskType = UnitTaskTypeEnum.GoToPosition;
        this.destinatedPosition = destinatedPosition;

    }
    public override void TakeVisulazationTask(GameObject flagGameObject)
    {
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

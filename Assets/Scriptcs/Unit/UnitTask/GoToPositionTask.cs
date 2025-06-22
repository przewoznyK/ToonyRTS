using UnityEngine;

public class GoToPositionTask : UnitTask
{
    public Vector3 destinatedPosition { private set; get; }

    public GoToPositionTask(Vector3 destinatedPosition)
    {
        unitTaskType = UnitTaskTypeEnum.GoToPosition;
        this.destinatedPosition = destinatedPosition;

    }

}

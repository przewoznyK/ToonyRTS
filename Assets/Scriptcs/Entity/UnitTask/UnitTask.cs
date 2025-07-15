using UnityEngine;

public class UnitTask
{
    public UnitTaskTypeEnum unitTaskType { protected set; get; }
    public Vector3 taskPosition;
    public virtual void TakeVisulazationTask(GameObject flagGameObject) { }
    public virtual void EndTask() { }
}

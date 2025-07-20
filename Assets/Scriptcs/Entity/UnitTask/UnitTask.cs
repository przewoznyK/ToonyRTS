using UnityEngine;

public class UnitTask
{
    public UnitTaskTypeEnum unitTaskType { protected set; get; }
    public GameObject flagGameObject;
    public Vector3 taskPosition;
    public Transform targetTransform;
    public virtual void TakeVisualizationTask(GameObject flagGameObject) { }
    public virtual void EndTask() { }
}

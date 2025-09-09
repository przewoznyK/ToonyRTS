using UnityEngine;

public class UnitTask
{
    public UnitTaskTypeEnum unitTaskType;
    public GameObject flagGameObject;
    public Vector3 taskPosition;
    public Transform targetTransform;
    public virtual void TakeVisualizationTask(GameObject flagGameObject) { }
    public virtual void EndTask() { }
}

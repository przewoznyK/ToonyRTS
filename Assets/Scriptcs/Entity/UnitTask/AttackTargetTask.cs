using UnityEngine;

public class AttackTargetTask : UnitTask
{
    public Transform targetTransform { private set; get; }
    public AttackTargetTask(Transform targetTransform)
    {
        unitTaskType = UnitTaskTypeEnum.AttackTarget;
        this.targetTransform = targetTransform;
    }
}


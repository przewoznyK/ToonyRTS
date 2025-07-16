using UnityEngine;

public class AttackTargetTask : UnitTask
{
    public AttackTargetTask(Transform targetTransform)
    {
        unitTaskType = UnitTaskTypeEnum.AttackTarget;
        this.targetTransform = targetTransform;
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


using System;
using UnityEngine;

public class MeleeWarrior : Unit
{

    [Header("Ranged Properties")]
    public bool isRanged;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletForce;
    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
            unitTaskManager.ResetTasks();  

        if (hit.collider.CompareTag("Ground"))
        {
            unitTaskManager.GoToPosition(hit.point);

        }
        else if(hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() & teamColor) != teamColor)
            {
                unitTaskManager.AttackTarget(component.GetProperties<Transform>(), component.GetTeam());
            }
        }
       
    }

    internal void AttackDetectionTarget(IGetTeamAndProperties component)
    {
        unitTaskManager.AttackTarget(component.GetProperties<Transform>(), component.GetTeam());
    }
}

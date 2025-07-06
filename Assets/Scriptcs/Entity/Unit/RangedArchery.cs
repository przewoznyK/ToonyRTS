using UnityEngine;

public class RangedArchery : Unit
{
    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
            unitTaskManager.ResetTasks();

        if (hit.collider.CompareTag("Ground"))
        {
            unitTaskManager.GoToPosition(hit.point);
            Debug.Log("GROUND");

        }
        else if (hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() & teamColor) != teamColor)
            {
                unitTaskManager.AttackTarget(component.GetProperties<Transform>(), component.GetTeam());
            }
        }

    }
}

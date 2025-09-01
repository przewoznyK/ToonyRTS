using UnityEngine;

public class MeleeWarrior : Unit
{
    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false && unitTaskManager.requestedTasks.Count > 0)
            unitTaskManager.RequestToServerToResetTasks();

        if (hit.collider.CompareTag("Ground"))
            unitTaskManager.RequestToServerToCreateGoToPositionTask(hit.point);

        else if(hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
            if (component.GetTeam() != teamColor)
                unitTaskManager.RequestToServerToCreateAttackEntityTask(component.GetTeam(), component.GetProperties<Transform>());
    }

    public override void PlayerLeftMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false && unitTaskManager.requestedTasks.Count > 0)
            unitTaskManager.RequestToServerToResetTasks();

        if (aggressiveApproach)
        {
            Debug.Log("LEFT PLAYER REQUEST");
            unitTaskManager.RequestToServerToCreateAggressiveApproachTask(hit.point);
        }

    }
}

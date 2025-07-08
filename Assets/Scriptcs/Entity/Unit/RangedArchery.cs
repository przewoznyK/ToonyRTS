using UnityEngine;

public class RangedArchery : Unit
{
    public RangedTaskManager rangedTaskManager;
    [Header("Ranged Properties")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletForce;
    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
            rangedTaskManager.ResetTasks();

        if (hit.collider.CompareTag("Ground"))
        {
            rangedTaskManager.GoToPosition(hit.point);

        }
        else if (hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() & teamColor) != teamColor)
            {
                rangedTaskManager.AttackTarget(component.GetProperties<Transform>(), component.GetTeam());
            }
        }

    }
}

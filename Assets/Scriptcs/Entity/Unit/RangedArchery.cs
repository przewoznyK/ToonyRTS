using UnityEngine;

public class RangedArchery : Unit
{
    [Header("Ranged Properties")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletForce;
    private void Start()
    {
        InitUniversalFunction();
    }

    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
            unitTaskManager.ResetTasks();

        if (hit.collider.CompareTag("Ground"))
        {
            unitTaskManager.GoToPosition(hit.point);
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

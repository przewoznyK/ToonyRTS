using UnityEngine;

public class DetectionCollider : MonoBehaviour
{
    [SerializeField] private MeleeWarrior meleeWarrior;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() != meleeWarrior.teamColor) && (component.GetEntityType() == EntityTypeEnum.unit))
            {
                meleeWarrior.AttackDetectionTarget(component);
            }
        }
    }
}

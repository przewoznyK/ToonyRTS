using System.Collections;
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
                StartCoroutine(DelayedAttack(component));
            }
        }
    }

    private IEnumerator DelayedAttack(IGetTeamAndProperties target)
    {
        yield return new WaitForSeconds(1f);
        meleeWarrior.AttackDetectionTarget(target);
    }
}

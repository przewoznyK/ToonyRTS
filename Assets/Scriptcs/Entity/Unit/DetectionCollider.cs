using System.Collections;
using UnityEngine;

public class DetectionCollider : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() != unit.teamColor) && (component.GetEntityType() == EntityTypeEnum.unit))
            {
                GetComponent<SphereCollider>().enabled = false;
                StartCoroutine(DelayedAttack(component));
            }
        }
    }

    private IEnumerator DelayedAttack(IGetTeamAndProperties target)
    {
        yield return new WaitForSeconds(2f);
        if(target != null)
            unit.AttackDetectionTarget(target);
    }
}

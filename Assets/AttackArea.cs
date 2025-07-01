using System;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private TeamColorEnum teamColor;
    Unit unit;
    internal void SetTeamColor(Unit unit)
    {
        this.unit = unit;
        teamColor = unit.teamColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EntityHealth>(out EntityHealth entityHealth))
        {
            if (entityHealth.teamColor != teamColor)
            {
                entityHealth.TakeDamage(unit, 3);
            }
        }
    }
}

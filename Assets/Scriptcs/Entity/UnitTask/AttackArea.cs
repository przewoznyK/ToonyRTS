using System;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    Unit unit;
    internal void Init(Unit unit)
    {
        this.unit = unit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EntityHealth>(out EntityHealth entityHealth))
        {
            if (entityHealth.teamColor != unit.teamColor)
            {
                entityHealth.TakeDamageFromUnit(unit);
            }
        }
    }
}

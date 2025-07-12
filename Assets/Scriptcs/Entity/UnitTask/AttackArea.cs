using System;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    MeleeWarrior meleeWarrior;
    internal void Init(MeleeWarrior meleeWarrior)
    {
        this.meleeWarrior = meleeWarrior;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EntityHealth>(out EntityHealth entityHealth))
        {
            if (entityHealth.teamColor != meleeWarrior.teamColor)
            {
                entityHealth.TakeDamage(meleeWarrior, 3);
            }
        }
    }
}

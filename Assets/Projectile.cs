using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Unit unit;

    private int damage;
    internal void SetStartProperties(Unit unit)
    {
        this.unit = unit;
        damage = this.unit.damage;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        EntityHealth entity = other.GetComponent<EntityHealth>();
        if(entity)
        {
            if (entity.teamColor != unit.teamColor)
            {
                entity.TakeDamage(unit, unit.damage);
                Destroy(gameObject);
            }
        }
    }
}

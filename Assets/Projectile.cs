using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Unit unit;
    internal void SetStartProperties(Unit unit)
    {
        this.unit = unit;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        EntityHealth entity = other.GetComponent<EntityHealth>();
        if(entity)
        {
            if (entity.teamColor != unit.teamColor)
            {
                entity.TakeDamageFromUnit(unit);
                Destroy(gameObject);
            }
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

public class UnitAnimationFunctions : MonoBehaviour
{
    Unit unit;
    UnitTaskManager unitTaskManager;
    [SerializeField] protected AttackArea attackArea;

    internal virtual void Init(Unit unit, UnitTaskManager unitTaskManager)
    {
        this.unit = unit;
        this.unitTaskManager = unitTaskManager;
        attackArea.Init(unit);
    }
    public void MeleeAttackSingleTarget()
    {
        if (unitTaskManager.taskTransform != null)
            unitTaskManager.taskTransform.GetComponent<EntityHealth>().TakeDamageFromUnit(unit);
    }
    public void MeleeColliderAttack(float duration)
    {
        StartCoroutine(ActiveAttackArea(duration));
    }
    IEnumerator ActiveAttackArea(float duration)
    {
        attackArea.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        attackArea.gameObject.SetActive(false);
    }

    public void ShootBullet()
    {
        GameObject bullet = Instantiate(unit.bulletPrefab, unit.shootPoint.position, unit.shootPoint.rotation);
        bullet.GetComponent<Projectile>().SetStartProperties(unit);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(unit.shootPoint.forward * unit.bulletForce, ForceMode.Impulse);
    }
}

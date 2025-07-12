using System;
using System.Collections;
using UnityEngine;

public class UnitAnimationFunctions : MonoBehaviour
{
    MeleeWarrior meleeWarrior;
    [SerializeField] protected AttackArea attackArea;

    internal virtual void Init(MeleeWarrior meleeWarrior)
    {
        this.meleeWarrior = meleeWarrior;
        attackArea.Init(meleeWarrior);
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
        GameObject bullet = Instantiate(meleeWarrior.bulletPrefab, meleeWarrior.shootPoint.position, meleeWarrior.shootPoint.rotation);
        bullet.GetComponent<Projectile>().SetStartProperties((Unit)meleeWarrior);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(meleeWarrior.shootPoint.forward * meleeWarrior.bulletForce, ForceMode.Impulse);
    }
}

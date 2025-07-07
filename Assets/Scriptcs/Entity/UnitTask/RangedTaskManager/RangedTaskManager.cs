using System.Collections;
using UnityEngine;

public class RangedTaskManager : UnitTaskManager
{
    RangedArchery rangedArchery;
    private void Start()
    {
        rangedArchery = GetComponent<RangedArchery>();
    }
    private void Update()
    {
        if (isOnTask)
        {
            if (currentTask.unitTaskType == UnitTaskTypeEnum.GoToPosition)
            {
                if (Vector3.Distance(taskVector, transform.position) <= 2f)
                {
                    GoToNextTask();
                }
            }

            if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget)
            {
                rotateToTaskTransform = true;
                rangedArchery.agent.SetDestination(taskTransform.position);
                rangedArchery.animator.SetFloat(Unit.Speed, 1f);

                if (Vector3.Distance(taskTransform.position, transform.position) <= rangedArchery.attackRange)
                {
                    StartCoroutine(AttackCycle());
                    isOnTask = false;
                }

            }
        }

        if (rotateToTaskTransform)
        {
            if (taskTransform != null)
            {
                var direction = taskTransform.position - transform.position;
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rangedArchery.rotationSpeed * Time.deltaTime);
            }
            else
                rotateToTaskTransform = false;
        }
    }



    public override void DoTask()
    {
        if (requestedTasks.Count > 0 && isOnTask == false)
        {
            currentTask = requestedTasks.First.Value;
            requestedTasks.RemoveFirst();
            if (currentTask is GoToPositionTask goToPositionTask)
            {
                Vector3 pos = goToPositionTask.destinatedPosition;
                rangedArchery.agent.SetDestination(pos);
                taskVector = pos;
                rangedArchery.animator.SetFloat(Unit.Speed, 1f);
            }
            else if (currentTask is AttackTargetTask attackTarget)
            {
                taskTransform = attackTarget.targetTransform;

            }
            isOnTask = true;
        }
    }
    void GoToNextTask()
    {
        isOnTask = false;
        rangedArchery.animator.SetFloat(Unit.Speed, 0f);
        DoTask();

    }

    IEnumerator AttackCycle()
    {
        rangedArchery.animator.SetFloat(Unit.Speed, 0f);
        rangedArchery.animator.SetTrigger("Shoot");
        //yield return new WaitForSeconds(0.2f);
        //attackArea.gameObject.SetActive(true);


        //yield return new WaitForSeconds(0.25f);
        //attackArea.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        if (taskTransform == null)
        {
            Transform nearestEnemy = FindNearestEnemy(enemyTeamTarget);
            if (nearestEnemy != null)
            {
                taskTransform = nearestEnemy;
                rotateToTaskTransform = true;
            }
                
            else
                yield break;
        }
        if (Vector3.Distance(taskTransform.position, transform.position) > rangedArchery.attackRange)
        {
            AttackTargetTask newTask = new(taskTransform);
            requestedTasks.AddFirst(newTask);
            DoTask();
            yield break;
        }
        yield return new WaitForSeconds(rangedArchery.attackCooldown);
        StartCoroutine(AttackCycle());
    }

    public override void DefenseFromAttack(Unit fromUnit)
    {
        if (fromUnit == null) return;
        if (isOnTask == false && taskTransform == null)
        {
            AttackTargetTask newTask = new(fromUnit.transform);
            enemyTeamTarget = fromUnit.GetTeam();
            rotateToTaskTransform = true;
            requestedTasks.AddFirst(newTask);
        //    unit.animator.SetFloat(Unit.Speed, 1f);
            DoTask();
        }
    }

    public void ShootBullet()
    {
        GameObject bullet = Instantiate(rangedArchery.bulletPrefab, rangedArchery.shootPoint.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().SetStartProperties((Unit)rangedArchery);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(rangedArchery.shootPoint.forward * rangedArchery.bulletForce, ForceMode.Impulse);
    }

    public override Transform FindNearestEnemy(TeamColorEnum teamColor)
    {
        Transform nearestEnemy = AccessToClassByTeamColor.instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, rangedArchery.maxEnemySearchingDistance);
        return nearestEnemy;
    }
}

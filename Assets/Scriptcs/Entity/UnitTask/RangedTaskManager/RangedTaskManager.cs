using System.Collections;
using UnityEngine;

public class RangedTaskManager : UnitTaskManager
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected float bulletForce;
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
                unit.agent.SetDestination(taskTransform.position);
                unit.animator.SetFloat(Unit.Speed, 1f);

                if (Vector3.Distance(taskTransform.position, transform.position) <= unit.attackRange)
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
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unit.rotationSpeed * Time.deltaTime);
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

                unit.agent.SetDestination(pos);
                taskVector = pos;
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
        unit.animator.SetFloat(Unit.Speed, 0f);
        DoTask();

    }

    IEnumerator AttackCycle()
    {
        unit.animator.SetFloat(Unit.Speed, 0f);
        unit.animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(0.2f);
        CreateBullet();
        //yield return new WaitForSeconds(0.2f);
        //attackArea.gameObject.SetActive(true);


        //yield return new WaitForSeconds(0.25f);
        //attackArea.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        if (taskTransform == null)
        {
            Transform nearestEnemy = FindNearestEnemy(enemyTeamTarget);
            if (nearestEnemy != null)
                taskTransform = nearestEnemy;
            else
                yield break;
        }
        if (Vector3.Distance(taskTransform.position, transform.position) > unit.attackRange)
        {
            AttackTargetTask newTask = new(taskTransform);
            requestedTasks.AddFirst(newTask);
            DoTask();
            yield break;
        }
        yield return new WaitForSeconds(unit.attackCooldown);
        StartCoroutine(AttackCycle());
    }

    public Transform FindNearestEnemy(TeamColorEnum teamColor)
    {
        Transform nearestEnemy = AccessToClassByTeamColor.instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, unit.maxEnemySearchingDistance);
        return nearestEnemy;
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

    void CreateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(shootPoint.forward * bulletForce, ForceMode.Impulse);
    }
}

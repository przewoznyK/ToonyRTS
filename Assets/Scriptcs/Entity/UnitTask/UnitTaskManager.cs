using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskManager : MonoBehaviour
{
    protected Unit unit;
    protected MeleeWarrior meleeWarrior;
    protected bool isOnTask;
    protected bool rotateToTaskTransform;
    public LinkedList<UnitTask> requestedTasks = new();
    protected UnitTask currentTask;
    protected Transform taskTransform;
    protected Vector3 taskVector;
    protected TeamColorEnum enemyTeamTarget;
    private void Start()
    {
        unit = GetComponent<Unit>();
        meleeWarrior = GetComponent<MeleeWarrior>();
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
                unit.agent.stoppingDistance = unit.attackRange;
                if (taskTransform != null)
                {
                    unit.agent.SetDestination(taskTransform.position);

                    if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance)
                    {
                        if (meleeWarrior.isRanged)
                            StartCoroutine(RangeAttackCycle());
                        else
                            StartCoroutine(MeleeAttackCycle());
                        isOnTask = false;
                    }
                }
                else
                {
                    unit.agent.ResetPath();
                    unit.animator.SetFloat(Unit.Speed, 0f);
                    isOnTask = false;

                }
            }
        }

        if(rotateToTaskTransform)
        {
            if (taskTransform != null)
            {
                var direction = taskTransform.position - transform.position;
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, meleeWarrior.rotationSpeed * Time.deltaTime);
            }
            else
                rotateToTaskTransform = false;
        }
    }



    public virtual void DoTask()
    {
        if (requestedTasks.Count > 0 && isOnTask == false)
        {
            currentTask = requestedTasks.First.Value;
            requestedTasks.RemoveFirst();
            if (currentTask is GoToPositionTask goToPositionTask)
            {
                unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                taskTransform = null;
                Vector3 pos = goToPositionTask.destinatedPosition;

                unit.agent.SetDestination(pos);
                taskVector = pos;
                unit.animator.SetFloat(Unit.Speed, 1f);
            }
            else if (currentTask is AttackTargetTask attackTarget)
            {
                unit.agent.stoppingDistance = unit.attackRange;
                taskTransform = attackTarget.targetTransform;
                unit.animator.SetFloat(Unit.Speed, 1f);
            }
      
            isOnTask = true;
        }
    }
    public void GoToNextTask()
    {
        isOnTask = false;
        unit.animator.SetFloat(Unit.Speed, 0f);
        DoTask();

    }

    public void ResetTasks()
    {
 
        requestedTasks.Clear();
        isOnTask = false;
        currentTask = null;
        StopAllCoroutines();
    }

    IEnumerator MeleeAttackCycle()
    {
        unit.animator.SetFloat(Unit.Speed, 0f);

        yield return new WaitForSeconds(0.5f);
        unit.animator.SetTrigger(Unit.AttackAnimationTrigger);
      
        if (CanUnitContinueAttackTheSameTarget() == false) yield break;
        yield return new WaitForSeconds(unit.attackCooldown);
        StartCoroutine(MeleeAttackCycle());
    }

    IEnumerator RangeAttackCycle()
    {
        meleeWarrior.animator.SetFloat(Unit.Speed, 0f);
        yield return new WaitForSeconds(0.5f);
        meleeWarrior.animator.SetTrigger("Shoot");

        if (CanUnitContinueAttackTheSameTarget() == false) yield break;
        yield return new WaitForSeconds(meleeWarrior.attackCooldown);
        StartCoroutine(RangeAttackCycle());
    }

    bool CanUnitContinueAttackTheSameTarget()
    {
        if (taskTransform == null && requestedTasks.Count == 0)
        {
            Transform nearestEnemy = FindNearestEnemy(enemyTeamTarget);
            if (nearestEnemy != null)
            {
                taskTransform = nearestEnemy;
                rotateToTaskTransform = true;
            }
            else return false;

            if (Vector3.Distance(taskTransform.position, transform.position) > meleeWarrior.attackRange)
            {
                AttackTargetTask newTask = new(taskTransform);
                requestedTasks.AddFirst(newTask);
                DoTask();
                return false;
            }
        }
        return true;
    }
    public Transform FindNearestEnemy(TeamColorEnum teamColor)
    {
        Transform nearestEnemy = AccessToClassByTeamColor.instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, unit.maxEnemySearchingDistance);
        return nearestEnemy;
    }

    public virtual void DefenseFromAttack(Unit fromUnit)
    {
        if (fromUnit == null) return;
        if(isOnTask == false && taskTransform == null)
        {
            AttackTargetTask newTask = new(fromUnit.transform);
            enemyTeamTarget = fromUnit.GetTeam();
            rotateToTaskTransform = true;
            requestedTasks.AddFirst(newTask);
            unit.animator.SetFloat(Unit.Speed, 1f);
            DoTask();
        }
    }

    public void ShootBullet()
    {
        GameObject bullet = Instantiate(meleeWarrior.bulletPrefab, meleeWarrior.shootPoint.position, meleeWarrior.shootPoint.rotation);
        bullet.GetComponent<Projectile>().SetStartProperties((Unit)meleeWarrior);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(meleeWarrior.shootPoint.forward * meleeWarrior.bulletForce, ForceMode.Impulse);
    }

    internal void GoToPosition(Vector3 point)
    {
        GoToPositionTask newTask = new(point);
        requestedTasks.AddLast(newTask);
        DoTask();
    }

    internal void AttackTarget(Transform target, TeamColorEnum targetTeam)
    {
  
        AttackTargetTask newTask = new(target);
        enemyTeamTarget = targetTeam;
        requestedTasks.AddLast(newTask);
        DoTask();
    }

    public void AttackFunctionInAnimation(float duration)
    {
        StartCoroutine(ActiveAttackArea(duration));
    }

    IEnumerator ActiveAttackArea(float duration)
    {
        unit.attackArea.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        unit.attackArea.gameObject.SetActive(false);
    }
}

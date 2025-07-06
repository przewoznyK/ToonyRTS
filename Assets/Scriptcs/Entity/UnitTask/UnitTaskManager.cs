using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskManager : MonoBehaviour
{
    protected Unit unit;
    public GameObject attackArea;

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
                unit.agent.SetDestination(taskTransform.position);
                unit.animator.SetFloat(Unit.Speed, 1f);

                if (Vector3.Distance(taskTransform.position, transform.position) <= unit.attackRange)
                {
                    StartCoroutine(AttackCycle());
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
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unit.rotationSpeed * Time.deltaTime);
                if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
                    rotateToTaskTransform = false;
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
                Vector3 pos = goToPositionTask.destinatedPosition;

                unit.agent.SetDestination(pos);
                taskVector = pos;
            }
            else if (currentTask is AttackTargetTask attackTarget)
            {
                taskTransform = attackTarget.targetTransform;
            }
         //   unit.animator.SetFloat(Unit.Speed, 1f);
            isOnTask = true;
        }
    }
    void GoToNextTask()
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

    IEnumerator AttackCycle()
    {
        unit.animator.SetFloat(Unit.Speed, 0f);
        unit.animator.SetTrigger(Unit.AttackAnimationTrigger);
      

        yield return new WaitForSeconds(0.2f);
        attackArea.gameObject.SetActive(true);

     
        yield return new WaitForSeconds(0.25f);
        attackArea.gameObject.SetActive(false);
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
}

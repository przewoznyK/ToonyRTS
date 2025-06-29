using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitTaskManager : MonoBehaviour
{
    Unit unit;
    public GameObject attackArea;

    bool isOnTask;
    public LinkedList<UnitTask> requestedTasks = new();
    UnitTask currentTask;
    Transform taskTransform;
    Vector3 taskVector;

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
                if (Vector3.Distance(taskTransform.position, transform.position) <= unit.attackRange)
                {
                    StartCoroutine(AttackCycle());
                    isOnTask = false;
                }
            }
        }
    }

    internal void GoToPosition(Vector3 point)
    {
        GoToPositionTask newTask = new(point);
        requestedTasks.AddLast(newTask);
    }

    internal void AttackTarget(Transform transform)
    {
        AttackTargetTask newTask = new(transform);
        requestedTasks.AddLast(newTask);

    }

    public void DoTask()
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
            unit.animator.SetFloat(Unit.Speed, 1f);
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
            Transform nearestEnemy = FindNearestEnemy(TeamColorEnum.Red);
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
        ControlledUnits enemies = AccessToClassByTeamColor.instance.GetControlledUnitsByTeamColor(teamColor);
        Transform nearestEnemy = AccessToClassByTeamColor.instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, unit.maxEnemySearchingDistance);
        return nearestEnemy;
    }
}

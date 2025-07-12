using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class UnitTaskManager : MonoBehaviour
{
    protected Unit unit;
    protected MeleeWarrior meleeWarrior;
    protected bool isOnTask;
    protected bool rotateToTaskTransform;
    public LinkedList<UnitTask> requestedTasks = new();
    protected UnitTask currentTask;
    public Transform taskTransform;
    protected Vector3 taskVector;
    protected TeamColorEnum enemyTeamTarget;
    public bool attackCycleActivated;
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

                    if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance && attackCycleActivated == false)
                    {
                        if (meleeWarrior.isRanged)
                            StartCoroutine(AttackCycle("Shoot"));
                        else
                            StartCoroutine(AttackCycle("Attack"));
                        attackCycleActivated = true;
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


    IEnumerator AttackCycle(string animationTriggerName)
    {
        meleeWarrior.animator.SetFloat(Unit.Speed, 0f);
        yield return new WaitForSeconds(0.5f);

        if (AttackAndCheckIfCanContinueAttackOrSearchNewEnemy(animationTriggerName) == false) yield break;
        yield return new WaitForSeconds(meleeWarrior.attackCooldown);
        StartCoroutine(AttackCycle(animationTriggerName));
    }

    bool AttackAndCheckIfCanContinueAttackOrSearchNewEnemy(string animationTriggerName)
    {
        if (taskTransform)
        {
            if(Vector3.Distance(taskTransform.position, transform.position) < meleeWarrior.attackRange)
            {
                unit.animator.SetTrigger(animationTriggerName);
                return true;
            }
            else
            {
                AttackTargetTask newTask = new(taskTransform);
                requestedTasks.AddFirst(newTask);
                DoTask();
                attackCycleActivated = false;
                return false;
            }
           
        }
        if (taskTransform == null && requestedTasks.Count == 0)
        {
            Transform nearestEnemy = FindNearestEnemy(enemyTeamTarget);
            if (nearestEnemy != null)
            {
                if (Vector3.Distance(nearestEnemy.position, transform.position) > meleeWarrior.attackRange)
                {
                    AttackTargetTask newTask = new(nearestEnemy);
                    requestedTasks.AddFirst(newTask);
                    DoTask();
                    attackCycleActivated = false;
                }
            }
        }
        return false;
    }
    public Transform FindNearestEnemy(TeamColorEnum teamColor)
    {
        Transform nearestEnemy = AccessToClassByTeamColor.instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, unit.maxEnemySearchingDistance);
        return nearestEnemy;
    }

    internal void GoToPosition(Vector3 point)
    {
        attackCycleActivated = false;
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

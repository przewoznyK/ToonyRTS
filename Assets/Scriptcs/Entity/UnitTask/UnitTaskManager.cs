using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
public class UnitTaskManager : MonoBehaviour
{
    protected Unit unit;
    protected TaskVisualization taskVisualization;
    public LineRenderer lineRenderer;
    protected bool isOnTask;
    protected bool rotateToTaskTransform;
    public LinkedList<UnitTask> requestedTasks = new();
    protected UnitTask currentTask;
    public Transform taskTransform { get; protected set; }
    protected Vector3 taskVector;
    protected TeamColorEnum enemyTeamTarget;
    protected bool attackCycleActivated;
    private int renderLineIndex;
    private void Start()
    {
        unit = GetComponent<Unit>();
        taskVisualization = GetComponent<TaskVisualization>();
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
                if (unit.teamColor == TeamColorEnum.Blue) Debug.Log("ON TASK ATTACK TARGET");
                rotateToTaskTransform = true;
                unit.agent.stoppingDistance = unit.attackRange;
                if (taskTransform != null)
                {
                    unit.agent.SetDestination(taskTransform.position);
                    
                    if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance && attackCycleActivated == false)
                    {
                        if (unit.isRanged)
                            StartCoroutine(AttackCycle("Shoot"));
                        else
                            StartCoroutine(AttackCycle("Attack"));
                        attackCycleActivated = true;
                        isOnTask = false;
                        if (unit.teamColor == TeamColorEnum.Blue) Debug.Log("ZMIANA SPEEED NA 0");

                        unit.animator.SetFloat(Unit.Speed, 0f);
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
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unit.rotationSpeed * Time.deltaTime);
            }
            else
                rotateToTaskTransform = false;
        }

        if (requestedTasks.Count >= 1)
        {
            int taskCount = requestedTasks.Count;
            lineRenderer.positionCount = taskCount + 2;

            if (taskVector != Vector3.zero)
                lineRenderer.SetPosition(0, transform.position);

            int index = 1;
            foreach (var task in requestedTasks)
            {
                if (taskVector != Vector3.zero)
                {
                    lineRenderer.SetPosition(index, task.taskPosition);
                    index++;
                }

            }
        }
        else
            lineRenderer.positionCount = 0;

    }



    public virtual void DoTask()
    {
        if (requestedTasks.Count > 0 && isOnTask == false)
        {
            currentTask = requestedTasks.First.Value;
   
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
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();

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


    public IEnumerator AttackCycle(string animationTriggerName)
    {
        yield return new WaitForSeconds(0.5f);

        if (AttackAndCheckIfCanContinueAttackOrSearchNewEnemy(animationTriggerName) == false) yield break;
        yield return new WaitForSeconds(unit.attackCooldown);
        StartCoroutine(AttackCycle(animationTriggerName));
    }

    bool AttackAndCheckIfCanContinueAttackOrSearchNewEnemy(string animationTriggerName)
    {
        if (taskTransform)
        {
            if(Vector3.Distance(taskTransform.position, transform.position) < unit.attackRange)
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
                if (Vector3.Distance(nearestEnemy.position, transform.position) > unit.attackRange)
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

    internal virtual void GoToPosition(Vector3 point)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        GoToPositionTask newTask = new(point);
        requestedTasks.AddLast(newTask);
        DoTask();
        if(requestedTasks.Count > 1) newTask.TakeVisulazationTask(taskVisualization.VisualizeTask(point));
        DrawLineRenderer(point);

    }

    internal void AttackTarget(Transform target, TeamColorEnum targetTeam)
    {
        unit.SetActiveEnemyDetector(false);
        AttackTargetTask newTask = new(target);
        enemyTeamTarget = targetTeam;
        requestedTasks.AddLast(newTask);
        DoTask();
    }

    public void DrawLineRenderer(Vector3 point)
    {
        //if (requestedTasks.Count > 1)
        //{
        //    renderLineIndex++;
        //    lineRenderer.positionCount = renderLineIndex + 1;
        //    lineRenderer.SetPosition(0, transform.position);
        //    lineRenderer.SetPosition(renderLineIndex, point);
        //}

    }
    public virtual void GatherResource(GatherableResource resource) { }
    public virtual void BuildConstructionTask(GameObject construction) { }
}

using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class UnitTaskManager : NetworkBehaviour
{
    [SerializeField] protected Unit unit;

    [SerializeField] public LineRenderer lineRenderer;
    protected bool isOnTask;
    protected bool rotateToTaskTransform;
    public LinkedList<UnitTask> requestedTasks = new();
    public SyncList<TaskDataForVisualization> taskDataForVisualizationList = new();
    protected UnitTask currentTask;
    public Transform taskTransform;
    protected Vector3 taskVector;
    public TeamColorEnum enemyTeamTarget;
    protected bool attackCycleActivated;
    public int animatorSpeedValue;
    public float detectionSphereEnemyAggressionApproachRadius = 10f;


    private void Update()
    {
        WorkingTask();
        RotateToTaskTransform();
    }
    public virtual void DoTask()
    {
        attackCycleActivated = false;
        if (requestedTasks.Count > 0 && isOnTask == false)
        {
            currentTask = requestedTasks.First.Value;
            if (currentTask is GoToPositionTask goToPositionTask)
            {
                unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                taskTransform = null;
                Vector3 pos = goToPositionTask.taskPosition;
                RespondFromServerToMoveUnit(pos);
                RespondFromServerToUpdateAnimatorSpeedValue(1);
                taskVector = pos;
            }
            else if (currentTask is AttackTargetTask attackTarget)
            {
                if(attackTarget.targetTransform != null)
                {
                    taskVector = Vector3.zero;
                    if (attackTarget.targetTransform.TryGetComponent<Building>(out Building building))
                    {
                        Collider buildingCollider = building.GetComponent<Collider>();
                        Vector3 closest = buildingCollider.ClosestPoint(unit.transform.position);
                 
                        taskVector = closest;
                    }
           
                    RespondFromServerToAttackEntity(attackTarget.targetTransform);
                    RespondFromServerToUpdateAnimatorSpeedValue(1);
                    rotateToTaskTransform = true;
                    unit.agent.stoppingDistance = unit.attackRange;
                }

            }
            else if (currentTask is AggressiveApproachTask aggressiveApproach)
            {

                unit.agent.stoppingDistance = unit.attackRange;
                Unit closetEnemyUnit = DetectUnits(aggressiveApproach.taskPosition);

                if (closetEnemyUnit != null)
                {
                      enemyTeamTarget = closetEnemyUnit.teamColor;
                      RespondFromServerToAttackEntity(closetEnemyUnit.transform);
                      currentTask.unitTaskType = UnitTaskTypeEnum.AttackTarget;
                }
                else
                {

                    unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                    taskTransform = null;
                    Vector3 pos = aggressiveApproach.taskPosition;
                    RespondFromServerToMoveUnit(pos);
                    taskVector = pos;
                    currentTask.unitTaskType = UnitTaskTypeEnum.GoToPosition;
                }
                RespondFromServerToUpdateAnimatorSpeedValue(1);
            }
            isOnTask = true;
        }
    }

    public virtual void WorkingTask()
    {
        if (isOnTask)
        {
            if (currentTask.unitTaskType == UnitTaskTypeEnum.GoToPosition)
            {
                if (Vector3.Distance(taskVector, transform.position) <= unit.agent.stoppingDistance)
                    GoToNextTask();
            }
            else if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget && taskTransform != null)
            {
                if (taskTransform != null)
                {
                    // Working Task Unitl Unit Reach Enemy 
                    if(taskVector != Vector3.zero)
                        RespondFromServerToMoveUnit(taskVector);
                    else
                        RespondFromServerToMoveUnit(taskTransform.position);

                    if (!unit.agent.pathPending && unit.agent.remainingDistance <= unit.agent.stoppingDistance && attackCycleActivated == false)
                    {
                        if (unit.isRanged)
                            StartCoroutine(AttackCycle("Shoot"));
                        else
                            StartCoroutine(AttackCycle("Attack"));

                        attackCycleActivated = true;
                        RespondFromServerToUpdateAnimatorSpeedValue(0);
                        isOnTask = false;
                    }
                }
            }
            else if(currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget && taskTransform == null)
            {
                GoToNextTask();
                AttackNearestEnemyByTeamColor();
            }
        }
    }
    public void RotateToTaskTransform()
    {
        if (rotateToTaskTransform)
        {
            if (taskTransform != null)
            {
                var direction = taskTransform.position - transform.position;
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unit.rotationSpeed * UnityEngine.Time.deltaTime);
            }
            else
                rotateToTaskTransform = false;
        }
    }
    public void GoToNextTask()
    {
        if(requestedTasks.Count > 0)
        {
            isOnTask = false;
            requestedTasks.First.Value.EndTask();
            requestedTasks.RemoveFirst();
            if (taskDataForVisualizationList.Count > 0)
                taskDataForVisualizationList.Remove(taskDataForVisualizationList[0]);

            RespondFromServerToUpdateAnimatorSpeedValue(0);
            DoTask();
        }

        unit.SetActiveEnemyDetector(true);
    }

    public IEnumerator AttackCycle(string animationTriggerName)
    {
        RespondFromServerToUpdateAnimatorSpeedValue(0);
        yield return new WaitForSeconds(0.5f);

        if (AttackAndCheckIfCanContinueAttackOrSearchNewEnemy(animationTriggerName) == false) yield break;
        yield return new WaitForSeconds(unit.attackCooldown);
        StartCoroutine(AttackCycle(animationTriggerName));
    }
    bool AttackAndCheckIfCanContinueAttackOrSearchNewEnemy(string animationTriggerName)
    {
        if (taskTransform)
        {
            unit.agent.SetDestination(taskTransform.position);
            if (!unit.agent.pathPending && unit.agent.remainingDistance <= unit.agent.stoppingDistance)
            {
                unit.animator.SetTrigger(animationTriggerName);
                return true;
            }
            else
            {
                AttackTargetTask newTask = new(taskTransform);
                var firstNode = requestedTasks.First;
                requestedTasks.AddAfter(firstNode, newTask);

                GoToNextTask();
                attackCycleActivated = false;
                
                return false;
            }
        }
        else if (taskTransform == null)
        {
            AttackNearestEnemyByTeamColor();

        }
        return false;
    }
    internal void AttackNearestEnemyByTeamColor()
    {
        Unit closettEnemy = FindNearestEnemy(enemyTeamTarget);
        if (closettEnemy != null)
        {
            if (Vector3.Distance(closettEnemy.transform.position, transform.position) > unit.attackRange)
            {
                AttackTargetTask newTask = new(closettEnemy.transform);
                requestedTasks.AddFirst(newTask);
                DoTask();
            }
            else
                RespondFromServerToCreateAttackEntityTask(closettEnemy.teamColor, closettEnemy.transform);
        }
    }
    public Unit FindNearestEnemy(TeamColorEnum teamColor)
    {
        Unit closettEnemy = EntitiesOnMapDatabase.Instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, unit.maxEnemySearchingDistance * 3);
        return closettEnemy;
    }
    #region tasks

    public virtual void GatherResourceTask(GatherableResource resource) { }
    #endregion

    // Change Animator Speed
    public void RespondFromServerToUpdateAnimatorSpeedValue(int newValue)
    {
        unit.animator.SetFloat("Speed", 0);
    }

    public void RespondFromServerToSetBoolAnimation(string animationName, bool value)
    {
        unit.animator.SetBool(animationName, value);
    }

    public virtual void RespondFromServerToResetTasks()
    {
        requestedTasks.Clear();
        isOnTask = false;
        currentTask = null;
        unit.SetActiveEnemyDetector(true);
        StopAllCoroutines();
        taskDataForVisualizationList.Clear();
    }

    // Go To Position Task
    public void RespondFromServerToCreateGoToPositionTask(Vector3 positionPoint)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        GoToPositionTask newTask = new(positionPoint);
        requestedTasks.AddLast(newTask);
        taskDataForVisualizationList.Add(new TaskDataForVisualization(positionPoint));
        DoTask();
    }
    // Attack Single Entity Task
    public void RespondFromServerToCreateAttackEntityTask(TeamColorEnum targetTeam, Transform targetEntity)
    {
        unit.SetActiveEnemyDetector(false);
        AttackTargetTask newTask = new(targetEntity);
        taskDataForVisualizationList.Add(new TaskDataForVisualization(targetEntity.GetComponent<NetworkIdentity>()));
        enemyTeamTarget = targetTeam;
        requestedTasks.AddLast(newTask);
        DoTask();
    }

    public void RespondFromServerToCreateAggressiveApproachTask(Vector3 positionPoint)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        AggressiveApproachTask newTask = new(positionPoint);
        requestedTasks.AddLast(newTask);
        DoTask();
    }


    public void RespondFromServerToMoveUnit(Vector3 targetPos)
    {
            unit.agent.SetDestination(targetPos);
    }

    public void RespondFromServerToAttackEntity(Transform targetTransform)
    {
        taskTransform = targetTransform;
        unit.SetActiveEnemyDetector(false);
    }
    public virtual void RequestToServerToBuildConstructionTask(GameObject constructionInstantiate) { }

    public virtual void StopBuildingThisConstruction()
    {
        throw new NotImplementedException();
    }


    private Collider[] overlapResults = new Collider[50];

    public Unit DetectUnits(Vector3 position)
    {
        int hits = Physics.OverlapSphereNonAlloc(
            position,
            detectionSphereEnemyAggressionApproachRadius,
            overlapResults
        );

        Vector3 myPosition = this.unit.transform.position;

        Unit closestUnit = null;
        float closestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hits; i++)
        {
            Unit unit = overlapResults[i].GetComponent<Unit>();
            if (unit != null && this.unit.teamColor != unit.teamColor)
            {
                float distanceSqr = (unit.transform.position - myPosition).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestUnit = unit;
                    closestDistanceSqr = distanceSqr;
                }
            }
        }
        return closestUnit;
    }

    public virtual void RespondFromServerToBuildConstructionTask(GameObject construction) { }


    public bool CanReachDestination(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        // "snap" punkt do najbli¿szego na NavMesh
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            if (unit.agent.CalculatePath(hit.position, path))
            {
                return path.status == NavMeshPathStatus.PathComplete;
            }
        }
     //   Debug.Log("CANT REACH");
        return false;
    }
}

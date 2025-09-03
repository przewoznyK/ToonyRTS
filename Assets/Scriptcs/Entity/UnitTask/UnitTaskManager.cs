using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitTaskManager : NetworkBehaviour
{
    [SerializeField] protected Unit unit;
    [SerializeField] public TaskVisualization taskVisualization;
    [SerializeField] public LineRenderer lineRenderer;
    protected bool isOnTask;
    protected bool rotateToTaskTransform;
    public LinkedList<UnitTask> requestedTasks = new();
    protected UnitTask currentTask;
    public Transform taskTransform { get; protected set; }
    protected Vector3 taskVector;
    protected TeamColorEnum enemyTeamTarget;
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
                RequestToServerToMoveUnit(pos);
                RequestToServerToChangeAnimatorSpeed(1);
                taskVector = pos;
            }
            else if (currentTask is AttackTargetTask attackTarget)
            {
                RequestToServerToAttackEntity(attackTarget.targetTransform);
                RequestToServerToChangeAnimatorSpeed(1);
                Debug.Log(unit.teamColor + " DO TASK ATTACK ");
            }
            else if (currentTask is AggressiveApproachTask aggressiveApproach)
            {
                unit.agent.stoppingDistance = unit.attackRange;
                Unit closetEnemyUnit = DetectUnits(aggressiveApproach.taskPosition);

                if (closetEnemyUnit != null)
                {
                    RequestToServerToCreateAttackEntityTask(closetEnemyUnit.teamColor, closetEnemyUnit.transform);

                }
                else
                    RequestToServerToCreateGoToPositionTask(aggressiveApproach.taskPosition);

                RequestToServerToChangeAnimatorSpeed(1);
                GoToNextTask();
                return;
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
                {
                    GoToNextTask();
                }
            }

            else if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget)
            {
                rotateToTaskTransform = true;
                unit.agent.stoppingDistance = unit.attackRange;
                if (taskTransform != null)
                {
                    // Working Task Unitl Unit Reach Enemy 
                    RequestToServerToMoveUnit(taskTransform.position);

                    if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance && attackCycleActivated == false)
                    {
                        if (unit.isRanged)
                            StartCoroutine(AttackCycle("Shoot"));
                        else
                            StartCoroutine(AttackCycle("Attack"));
                        attackCycleActivated = true;
                        RequestToServerToChangeAnimatorSpeed(0);
                        isOnTask = false;
                    }
                }
                else if (taskTransform == null && requestedTasks.Count == 1)
                {
                    GoToNextTask();
                    AttackNearestEnemyByTeamColor();

                }
            }
        }
    }
    public void RotateToTaskTransform()
    {
        if (rotateToTaskTransform)
        {
            if (taskTransform != null)
            {
             //   Debug.Log(unit.teamColor + "   " + taskTransform.gameObject.name + "   " + taskTransform.GetComponent<Unit>().teamColor);
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
        isOnTask = false;
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks);
        RequestToServerToChangeAnimatorSpeed(0);
        DoTask();
        unit.SetActiveEnemyDetector(true);
      //  Debug.Log(unit.teamColor + " GO TO NEXT TASK");
    }

    public IEnumerator AttackCycle(string animationTriggerName)
    {
        RequestToServerToChangeAnimatorSpeed(0);
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
        else if (taskTransform == null && requestedTasks.Count == 1)
        {
            GoToNextTask();
            AttackNearestEnemyByTeamColor();

        }
        Debug.Log(unit.teamColor + " AKTUALNIE MAM " + requestedTasks.Count + " TASKOW ");

        foreach (var item in requestedTasks)
        {
            Debug.Log("TASK " + item.unitTaskType);
        }
        //else
        //    GoToNextTask();


        return false;
    }
    internal void AttackNearestEnemyByTeamColor()
    {
        Debug.Log(unit.teamColor + " SZUKAM ENEMY " + enemyTeamTarget);
        Unit closettEnemy = FindNearestEnemy(enemyTeamTarget);
        if (closettEnemy != null)
        {
            Debug.Log(unit.teamColor + " Znalazlem enemy");
            if (Vector3.Distance(closettEnemy.transform.position, transform.position) > unit.attackRange)
            {
                AttackTargetTask newTask = new(closettEnemy.transform);
                requestedTasks.AddFirst(newTask);
                DoTask();
            }
        }
    }
    public Unit FindNearestEnemy(TeamColorEnum teamColor)
    {
        Unit closettEnemy = EntitiesOnMapDatabase.Instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, unit.maxEnemySearchingDistance);
        return closettEnemy;
    }
    #region tasks

    public virtual void GatherResourceTask(GatherableResource resource) { }
    #endregion

    // Change Animator Speed
    public void RequestToServerToChangeAnimatorSpeed(int speedValue)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdChangeAnimatorSpeedUnit(this.GetComponent<NetworkIdentity>(), speedValue);
    }
    public void RespondFromServerUpdateAnimatorSpeedValue(int newValue)
    {
        unit.animator.SetFloat("Speed", newValue);
    }

    public void RequestToServerToSetBoolAnimation(string animationName, bool value)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdSetBoolAnimation(this.GetComponent<NetworkIdentity>(), animationName, value);
    }
    public void RespondFromServerToSetBoolAnimation(string animationName, bool value)
    {
        unit.animator.SetBool(animationName, value);
    }
    // Reset Tasks
    public void RequestToServerToResetTasks()
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdResetTasksUnit(this.GetComponent<NetworkIdentity>());
    }
    public void RespondFromServerToResetTasks()
    {
        requestedTasks.Clear();
        isOnTask = false;
        currentTask = null;
        unit.SetActiveEnemyDetector(true);
        StopAllCoroutines();
    }

    // Go To Position Task
    public virtual void RequestToServerToCreateGoToPositionTask(Vector3 positionPoint)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdCreateGoToPositionTask(this.GetComponent<NetworkIdentity>(), positionPoint);
    }
    public void RespondFromServerToCreateGoToPositionTask(Vector3 positionPoint)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        GoToPositionTask newTask = new(positionPoint);
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
    // Attack Single Entity Task
    public virtual void RequestToServerToCreateAttackEntityTask(TeamColorEnum targetTeam, Transform targetEntity)
    {
      //  if (!isLocalPlayer) return;
        Debug.Log(unit.teamColor + "   REQUEST ATTACK  " + targetTeam + "  " + targetEntity.GetComponent<Unit>().teamColor);
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdCreateAttackEntityTask(this.GetComponent<NetworkIdentity>(), targetTeam, targetEntity);
    }
    public void RespondFromServerToCreateAttackEntityTask(TeamColorEnum targetTeam, Transform targetEntity)
    {
        Debug.Log(unit.teamColor + "   RESPOND ATTACK  " + targetTeam + "  " + targetEntity.GetComponent<Unit>().teamColor);
        unit.SetActiveEnemyDetector(false);
        AttackTargetTask newTask = new(targetEntity);
        enemyTeamTarget = targetTeam;
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
    public virtual void RequestToServerToCreateAggressiveApproachTask(Vector3 positionPoint)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdCreateAggressiveApproachTask(this.GetComponent<NetworkIdentity>(), positionPoint);
    }
    public void RespondFromServerToCreateAggressiveApproachTask(Vector3 positionPoint)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        AggressiveApproachTask newTask = new(positionPoint);
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }

    public void RequestToServerToMoveUnit(Vector3 targetPos)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdMoveUnit(this.GetComponent<NetworkIdentity>(), targetPos);
    }
    public void RespondFromServerToMoveUnit(Vector3 targetPos)
    {
        unit.agent.SetDestination(targetPos);
    }
    public void RequestToServerToAttackEntity(Transform targetTransform)
    {
        Debug.Log(unit.teamColor + " REQUEST FROM SERVER " + targetTransform.GetComponent<Unit>().teamColor);

        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdAttackEntity(this.GetComponent<NetworkIdentity>(), targetTransform);
    }
    public void RespondFromServerToAttackEntity(Transform targetTransform)
    {
        taskTransform = targetTransform;
        unit.SetActiveEnemyDetector(false);
   //     Debug.Log(unit.teamColor + " ReSPOND FROM SERVER " + targetTransform.GetComponent<Unit>().teamColor);
    }
    public virtual void RequestToServerToBuildConstructionTask(GameObject constructionInstantiate) { }

    public virtual void StopBuildingThisConstruction()
    {
        throw new NotImplementedException();
    }


    public Unit DetectUnits(Vector3 position)
    {
        List<Unit> unitsInSphere = new();
        Collider[] hitColliders = Physics.OverlapSphere(position, detectionSphereEnemyAggressionApproachRadius);

        foreach (var collider in hitColliders)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null)
            {
                if(this.unit.teamColor != unit.teamColor)
                    unitsInSphere.Add(unit);
            }   
        }

        if (unitsInSphere.Count == 0)
            return null;

        Unit closestUnit = unitsInSphere[0];
        float closestDistanceSqr = (closestUnit.transform.position - position).sqrMagnitude;

        for (int i = 1; i < unitsInSphere.Count; i++)
        {
            float distanceSqr = (unitsInSphere[i].transform.position - position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestUnit = unitsInSphere[i];
                closestDistanceSqr = distanceSqr;
            }
        }
      //  Debug.Log(closestUnit.name+ "CLOSET ENEMY");
        return closestUnit;
    }
}

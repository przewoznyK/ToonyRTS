using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
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
                RespondFromServerToAttackEntity(attackTarget.targetTransform);
                RespondFromServerUpdateAnimatorSpeedValue(1);

                if (isServer)
                    Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [SERVER] DoTask AttackTargetTask " + requestedTasks.Count);
                else
                    Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [CLIENT] DoTask AttackTargetTask " + requestedTasks.Count);
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
                    GoToNextTask();

            }
            else if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget && taskTransform != null)
            {
                    Debug.Log("WorkingTask ATtack Target " + taskTransform.gameObject);
                    rotateToTaskTransform = true;
                unit.agent.stoppingDistance = unit.attackRange;
                if (taskTransform != null)
                {
                        // Working Task Unitl Unit Reach Enemy 
                        PlayerController.LocalPlayer.CmdMoveUnit(this.netIdentity, taskTransform.position);
                        Debug.Log("IDZIE DO ENEMY");
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
 
            }
            else if(currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget && taskTransform == null)
            {
                Debug.Log("NEXT ENEMY");
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
        isOnTask = false;
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        Debug.Log("GO TO NEXT TASK");
        if(taskDataForVisualizationList.Count > 0)
            taskDataForVisualizationList.Remove(taskDataForVisualizationList[0]);

        RequestToServerToChangeAnimatorSpeed(0);
        DoTask();
        unit.SetActiveEnemyDetector(true);
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
                var firstNode = requestedTasks.First;
                requestedTasks.AddAfter(firstNode, newTask);

                GoToNextTask();
                attackCycleActivated = false;
                
                return false;
            }
           
        }
        else if (taskTransform == null && requestedTasks.Count <= 1)
        {
            GoToNextTask();
            AttackNearestEnemyByTeamColor();

        }
        if (isServer)
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [SERVER] AttackAndCheckIfCanContinueAttackOrSearchNewEnemy " + requestedTasks.Count);
        else
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [CLIENT] AttackAndCheckIfCanContinueAttackOrSearchNewEnemy " + requestedTasks.Count);

        //foreach (var item in requestedTasks)
        //{
        //    Debug.Log("TASK " + item.unitTaskType);
        //}
        //else
        //    GoToNextTask();


        return false;
    }
    internal void AttackNearestEnemyByTeamColor()
    {
        if (isServer)
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [SERVER] AttackNearestEnemyByTeamColor " + requestedTasks.Count);
        else
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [CLIENT] AttackNearestEnemyByTeamColor " + requestedTasks.Count);
        Unit closettEnemy = FindNearestEnemy(enemyTeamTarget);
        if (closettEnemy != null)
        {
            if (isServer)
                Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [SERVER] AttackNearestEnemyByTeamColor - JEST ENEMY " + requestedTasks.Count + "   " + closettEnemy.teamColor);
            else
                Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [CLIENT] AttackNearestEnemyByTeamColor - JEST ENEMY " + requestedTasks.Count + "   " + closettEnemy.teamColor);
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
   // public void RequestToServerToResetTasks()
   // {
   ////     Debug.Log(unit.teamColor + " RequestToServerToResetTasks");
   //     //  if (PlayerController.LocalPlayer.isLocalPlayer)
   //     PlayerController.LocalPlayer.CmdResetTasksUnit(this.GetComponent<NetworkIdentity>());
   // }
    public void RespondFromServerToResetTasks()
    {
        requestedTasks.Clear();
        isOnTask = false;
        currentTask = null;
        unit.SetActiveEnemyDetector(true);
        StopAllCoroutines();
        taskDataForVisualizationList.Clear();
        Debug.Log(unit.teamColor + " RespondFromServerToResetTasks");
    }

    // Go To Position Task
    public virtual void RequestToServerToCreateGoToPositionTask(Vector3 positionPoint)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdCreateGoToPositionTask(this.GetComponent<NetworkIdentity>(), positionPoint);
    }
    public void RespondFromServerToCreateGoToPositionTask(Vector3 positionPoint)
    {
        Debug.Log("RESPOND GO TO POSITION TASK");
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        GoToPositionTask newTask = new(positionPoint);
        requestedTasks.AddLast(newTask);
        taskDataForVisualizationList.Add(new TaskDataForVisualization(positionPoint));
        Debug.Log(taskDataForVisualizationList.Count + " <--");
        DoTask();
        
       // newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));

        Debug.Log(unit.teamColor +" TYLE TASKOW MA " + requestedTasks.Count);
    }
    // Attack Single Entity Task
    public virtual void RequestToServerToCreateAttackEntityTask(TeamColorEnum targetTeam, Transform targetEntity)
    {
        if (isServer)
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [SERVER] RequestToServerToCreateAttackEntityTask " + requestedTasks.Count + "   " + targetTeam);
        else
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [CLIENT] RequestToServerToCreateAttackEntityTask " + requestedTasks.Count + "   " + targetTeam);

        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdCreateAttackEntityTask(this.GetComponent<NetworkIdentity>(), targetTeam, targetEntity);
    }
    public void RespondFromServerToCreateAttackEntityTask(TeamColorEnum targetTeam, Transform targetEntity)
    {
        if (isServer)
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [SERVER] RespondFromServerToCreateAttackEntityTask " + requestedTasks.Count + "   " + targetTeam);
        else
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [CLIENT] RespondFromServerToCreateAttackEntityTask " + requestedTasks.Count + "   " + targetTeam);
        unit.SetActiveEnemyDetector(false);
        AttackTargetTask newTask = new(targetEntity);
        taskDataForVisualizationList.Add(new TaskDataForVisualization(targetEntity.GetComponent<NetworkIdentity>()));
        enemyTeamTarget = targetTeam;
        requestedTasks.AddLast(newTask);
        DoTask();
     //   newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
    //public virtual void RequestToServerToCreateAggressiveApproachTask(Vector3 positionPoint)
    //{
    //    if (PlayerController.LocalPlayer.isLocalPlayer)
    //        PlayerController.LocalPlayer.CmdCreateAggressiveApproachTask(this.GetComponent<NetworkIdentity>(), positionPoint);
    //}
    public void RespondFromServerToCreateAggressiveApproachTask(Vector3 positionPoint)
    {
        Debug.Log("RespondFromServerToCreateAggressiveApproachTask");
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        AggressiveApproachTask newTask = new(positionPoint);
        requestedTasks.AddLast(newTask);
        DoTask();
     //   newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
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
        if (isServer)
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [SERVER] RequestToServerToAttackEntity " + requestedTasks.Count + "   " + targetTransform.GetComponent<Unit>().teamColor);
        else
            Debug.Log(unit.teamColor + "   " + unit.gameObject.name + " [CLIENT] RequestToServerToAttackEntity " + requestedTasks.Count + "   " + targetTransform.GetComponent<Unit>().teamColor);

        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdAttackEntity(this.GetComponent<NetworkIdentity>(), targetTransform);
    }
    public void RespondFromServerToAttackEntity(Transform targetTransform)
    {
        Debug.Log("RespondFromServerToAttackEntity <---");
        taskTransform = targetTransform;
        unit.SetActiveEnemyDetector(false);
        Debug.Log(requestedTasks.Count);
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

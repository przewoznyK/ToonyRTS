using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitTaskManager : NetworkBehaviour
{
    [SerializeField] protected Unit unit;
    protected bool isOnTask;
    protected bool rotateToTaskTransform;
    public LinkedList<UnitTask> requestedTasks = new();
    protected UnitTask currentTask;
    public Transform taskTransform { get; protected set; }
    protected Vector3 taskVector;
    protected TeamColorEnum enemyTeamTarget;
    protected bool attackCycleActivated;
    [HideInInspector]
    public TaskVisualization taskVisualization;
    [HideInInspector]
    public LineRenderer lineRenderer;
    public int animatorSpeedValue;
    public bool respondFromServer;
    private void Start()
    {
        taskVisualization = GetComponent<TaskVisualization>();
        lineRenderer = GetComponent<LineRenderer>();

    }
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
                    taskVector = pos;

                }
                else if (currentTask is AttackTargetTask attackTarget)
                {
                    unit.agent.stoppingDistance = unit.attackRange;
                    taskTransform = attackTarget.targetTransform;
                    
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

            if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget)
            {
                rotateToTaskTransform = true;
                unit.agent.stoppingDistance = unit.attackRange;
                if (taskTransform != null)
                {
                    RequestToServerToMoveUnit(taskTransform.position);
                    if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance && attackCycleActivated == false)
                    {
                        if (unit.isRanged)
                            StartCoroutine(AttackCycle("Shoot"));
                        else
                            StartCoroutine(AttackCycle("Attack"));
                        attackCycleActivated = true;
                        isOnTask = false;

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
        taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks);
        DoTask();
        RequestToServerToChangeAnimatorSpeed(0);
        unit.animator.SetFloat("Speed", animatorSpeedValue);
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
                }
            }
        }
        else 
        {
            requestedTasks.First.Value.EndTask();
            requestedTasks.RemoveFirst();
            DoTask();
            taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks);
        }
        
   
        return false;
    }
    public Transform FindNearestEnemy(TeamColorEnum teamColor)
    {
        Transform nearestEnemy = EntitiesOnMapDatabase.Instance.GetClosestTransformEnemyByTeamColor(teamColor, transform.position, unit.maxEnemySearchingDistance);
        return nearestEnemy;
    }
    #region tasks
    public virtual void GoToPosition(Vector3 point)
    {
        Debug.Log("ide na POZYCJE");
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        GoToPositionTask newTask = new(point);
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
    internal void AttackTarget(Transform target, TeamColorEnum targetTeam)
    {
        unit.SetActiveEnemyDetector(false);
        AttackTargetTask newTask = new(target);
        enemyTeamTarget = targetTeam;
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
    public virtual void GatherResourceTask(GatherableResource resource) { }
    public virtual void BuildConstructionTask(GameObject construction) { }
    #endregion



    public void RequestToServerToChangeAnimatorSpeed(int speedValue)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdChangeAnimatorSpeedUnit(this.GetComponent<NetworkIdentity>(), speedValue);
    }

    public void RequestToServerToMoveUnit(Vector3 targetPos)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdMoveUnit(this.GetComponent<NetworkIdentity>(), targetPos);
    }
    public void RespondFromServerToMoveUnit(Vector3 targetPos)
    {
        unit.agent.SetDestination(targetPos);
        unit.animator.SetFloat("Speed", 1);
    }
    public void RequestToServerToAttackEntity(Transform targetTransform)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdAttackEntity(this.GetComponent<NetworkIdentity>(), targetTransform);
    }
    public void RespondFromServerToAttackEntity(Transform targetTransform)
    {
        taskTransform = targetTransform;
        respondFromServer = true;
    }
    public void UpdateAnimatorSpeedValue(int newValue)
    {
        unit.animator.SetFloat("Speed", newValue);
    }
}

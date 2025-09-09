using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GathererTaskManager : UnitTaskManager
{
    public List<ObjectPrices> gatheredResources { get; private set; } = new();
    [Header("Gatherer Properties")]
    public GatherableResource currentGatherableResource;
    public ResourceTypesEnum currentResourceTypeGathering;
    public int maxCarried = 20;
    public int currentGathered;
    public int reachedPerSecond;
    private bool isGoingToStockPile;
    private bool isGoingToBuildingConstruction;
    Vector3 constructionToBuildPosition;
    public InConstructionBuildingRepresentation currentConstionBuildingTarget { get; private set; }

    private IStockPile stockPile;
    private void Start()
    {
        var allowedTypes = new List<ResourceTypesEnum>
        {
            ResourceTypesEnum.food,
            ResourceTypesEnum.wood,
            ResourceTypesEnum.gold,
            ResourceTypesEnum.stone
        };

        foreach (ResourceTypesEnum type in allowedTypes)
        {
            gatheredResources.Add(new ObjectPrices(type, 0));
        }

    }
    private void Update()
    {
        WorkingTask();
        RotateToTaskTransform();
    }
    public override void DoTask()
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
                RespondFromServerUpdateAnimatorSpeedValue(1);
                    taskVector = pos;
                }
                else if (currentTask is AttackTargetTask attackTarget)
                {
                RespondFromServerToAttackEntity(attackTarget.targetTransform);
                RespondFromServerUpdateAnimatorSpeedValue(1);
                }
                else if (currentTask is AggressiveApproachTask aggressiveApproach)
                {
                    unit.agent.stoppingDistance = unit.attackRange;
                    Unit closetEnemyUnit = DetectUnits(aggressiveApproach.taskPosition);

                    if (closetEnemyUnit != null)
                    {
                    RespondFromServerToCreateAttackEntityTask(closetEnemyUnit.teamColor, closetEnemyUnit.transform);

                    }
                    else
                    RespondFromServerToCreateGoToPositionTask(aggressiveApproach.taskPosition);

                RespondFromServerUpdateAnimatorSpeedValue(1);
                    GoToNextTask();
                    return;
                }
                else if (currentTask is GathererResourceTask gatherResource)
                {
                    currentGatherableResource = gatherResource.gatherableResource;
                    unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                    taskTransform = gatherResource.targetTransform;
                    currentResourceTypeGathering = gatherResource.currentResourceTypeGathering;
                RespondFromServerToMoveUnit(taskTransform.position);
                RespondFromServerUpdateAnimatorSpeedValue(1);
                }
                else if (currentTask is ReturnToStockpileTask returnToStockpile)
                {
                    unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                    taskTransform = null;
                    Vector3 pos = returnToStockpile.taskPosition;

                    taskVector = pos;

                RespondFromServerToMoveUnit(pos);
                RespondFromServerUpdateAnimatorSpeedValue(1);
                    isGoingToStockPile = true;
                }
                else if (currentTask is BuildConstructionTask construction)
                {
                    currentConstionBuildingTarget = construction.constructionBuildingRepresentation;
                    construction.constructionBuildingRepresentation.gatherersBuildingThisConstruction.Add((GathererNew)this.unit);
                    unit.agent.stoppingDistance = unit.attackRange;
                Debug.Log("Change Position");
                RespondFromServerToMoveUnit(construction.constructionPosition);
                RespondFromServerUpdateAnimatorSpeedValue(1);
                    isGoingToBuildingConstruction = true;
                }
                isOnTask = true;
            }
    }
    public override void WorkingTask()
    {
        if (isOnTask)
        {
            if (currentTask.unitTaskType == UnitTaskTypeEnum.GoToPosition)
            {
                RespondFromServerToSetBoolAnimation("Harvest", false);

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
                    RespondFromServerToMoveUnit(taskTransform.position);

                    if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance && attackCycleActivated == false)
                    {
                        if (unit.isRanged)
                            StartCoroutine(AttackCycle("Shoot"));
                        else
                            StartCoroutine(AttackCycle("Attack"));
                        attackCycleActivated = true;
                        RespondFromServerUpdateAnimatorSpeedValue(0);
                        isOnTask = false;
                    }
                }
                else if (taskTransform == null && requestedTasks.Count == 1)
                {
                    GoToNextTask();
                    AttackNearestEnemyByTeamColor();

                }
            }
            else if (currentTask.unitTaskType == UnitTaskTypeEnum.GatherResource)
            {
                if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance)
                {
                    unit.agent.ResetPath();
                    RespondFromServerUpdateAnimatorSpeedValue(0);
                    isOnTask = false;
                    RespondFromServerToSetBoolAnimation("Harvest", true);
                }
            }

            if (isGoingToStockPile)
            {
                if (Vector3.Distance(stockPile.stockPilePosition.position, transform.position) <= unit.agent.stoppingDistance)
                {
                    isGoingToStockPile = false;
                    RespondFromServerUpdateAnimatorSpeedValue(0);
                    var returnObjectPrices = stockPile.AddResourcesToStockPile(gatheredResources);
                    UpdateGatheredResourcesAmount(returnObjectPrices);

                    currentGathered = 0;
                    if (currentGatherableResource)
                    {
                        GatherResourceTask(currentGatherableResource);
                        GoToNextTask();
                    }
                    else
                        GoToNextResource();
                }
            }
            else if (isGoingToBuildingConstruction)
            {
                if (Vector3.Distance(constructionToBuildPosition, transform.position) <= unit.agent.stoppingDistance)
                {
                    unit.agent.ResetPath();
                    RespondFromServerUpdateAnimatorSpeedValue(0);
                    RespondFromServerToSetBoolAnimation("Building", true);
                    isGoingToBuildingConstruction = false;
                }
            }
        }
    }

    public void UpdateGatheredResourcesAmount(List<ObjectPrices> newObjectPrices)
    {
        gatheredResources = newObjectPrices;
    }
    public override void StopBuildingThisConstruction()
    {
        ResetGathererProperties();
    }
    public void ResetGathererProperties()
    {
        RespondFromServerToSetBoolAnimation("Harvest", false);
        isGoingToStockPile = false;
        RespondFromServerToSetBoolAnimation("Building", false);
        GoToNextTask();
    }

    public void GoToNextResource()
    {
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        isOnTask = false;
        RespondFromServerToSetBoolAnimation("Harvest", false);

        if (requestedTasks.Count == 0)
        {
            GatherableResource nextResource = FindNearestResource();
            if (nextResource != null)
                GatherResourceTask(nextResource); 
               
        }
        else DoTask();
    }

    public GatherableResource FindNearestResource()
    {
        GatherableResource[] allResources = FindObjectsOfType<GatherableResource>();

        GatherableResource closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GatherableResource resource in allResources)
        {
            float distance = Vector3.Distance(currentPosition, resource.transform.position);
            if (distance < minDistance && resource.resourceType == currentResourceTypeGathering)
            {
                minDistance = distance;
                closest = resource;
            }
        }

        return closest;
    }

    public bool CheckIfGathererHaveToReturnToStockPile()
    {
        return currentGathered >= maxCarried;
    }

    public void ReturnToStockPile()
    {
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        RespondFromServerToSetBoolAnimation("Harvest", false);

        stockPile = PlayerController.LocalPlayer.stockPileManager.GetClosestStockPile(transform.position);
        if (stockPile != null)
        {
            unit.SetActiveEnemyDetector(false);
            attackCycleActivated = false;
            ReturnToStockpileTask newTask = new(stockPile.stockPilePosition.transform.position);
            requestedTasks.AddLast(newTask);
            DoTask();
       //     newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
        }
    }
    public override void GatherResourceTask(GatherableResource resource)
    {
        unit.SetActiveEnemyDetector(false);
        GathererResourceTask newTask = new(resource);
        requestedTasks.AddLast(newTask);
        DoTask();
    //    newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }

    public override void RequestToServerToBuildConstructionTask(GameObject construction)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdBuildConstructionTask(this.GetComponent<NetworkIdentity>(), construction);
    }
    public void RespondFromServerToBuildConstructionTask(GameObject construction)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        BuildConstructionTask newTask = new(construction);
        requestedTasks.AddLast(newTask);
        DoTask();
      //  newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
}

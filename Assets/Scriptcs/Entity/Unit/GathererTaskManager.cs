using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

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
        unit = GetComponent<Unit>();

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


        if(isGoingToStockPile)
        {
            if (Vector3.Distance(stockPile.stockPilePosition.position, transform.position) <= unit.agent.stoppingDistance)
            {
                isGoingToStockPile = false;
                unit.animator.SetFloat(Unit.Speed, 0f);
                var returnObjectPrices = stockPile.AddResourcesToStockPile(gatheredResources);
                UpdateGatheredResourcesAmount(returnObjectPrices);

                currentGathered = 0;
                if (currentGatherableResource)
                {
                    requestedTasks.First.Value.EndTask();
                    requestedTasks.RemoveFirst();
                    GatherResourceTask(currentGatherableResource);
                    Debug.Log(1);
                }
                else
                    GoToNextResource();
            }
        }
        else if(isGoingToBuildingConstruction)
        {
            if (Vector3.Distance(constructionToBuildPosition, transform.position) <= unit.agent.stoppingDistance)
            {
                unit.agent.ResetPath();
                unit.animator.SetFloat(Unit.Speed, 0f);
                unit.animator.SetBool("Building", true);
                isGoingToBuildingConstruction = false;
            }
        }
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
                    RequestToServerMove(pos);
                    taskVector = pos;
            }
                else if (currentTask is AttackTargetTask attackTarget)
                {
                    unit.agent.stoppingDistance = unit.attackRange;
                    taskTransform = attackTarget.targetTransform;
                    unit.animator.SetFloat(Unit.Speed, 1f);
                }
                else if (currentTask is GathererResourceTask gatherResource)
                {
                    currentGatherableResource = gatherResource.gatherableResource;
                    unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                    taskTransform = gatherResource.targetTransform;
                    currentResourceTypeGathering = gatherResource.currentResourceTypeGathering;
                    unit.agent.SetDestination(taskTransform.position);
                    unit.animator.SetFloat(Unit.Speed, 1f);
                }
                else if (currentTask is ReturnToStockpileTask returnToStockpile)
                {
                    unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                    taskTransform = null;
                    Vector3 pos = returnToStockpile.taskPosition;

                    unit.agent.SetDestination(pos);
                    taskVector = pos;
                    unit.animator.SetFloat(Unit.Speed, 1f);
                    isGoingToStockPile = true;
                }
                else if (currentTask is BuildConstructionTask construction)
                {
                    currentConstionBuildingTarget = construction.constructionBuildingRepresentation;
                    unit.agent.stoppingDistance = unit.attackRange;
                    constructionToBuildPosition = construction.constructionPosition;
                    unit.animator.SetFloat(Unit.Speed, 1f);
                    unit.agent.SetDestination(construction.constructionPosition);
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
                unit.animator.SetBool("Harvest", false);
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
                    unit.agent.SetDestination(taskTransform.position);

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
            else if (currentTask.unitTaskType == UnitTaskTypeEnum.GatherResource)
            {
                if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance)
                {
                    unit.agent.ResetPath();
                    unit.animator.SetFloat(Unit.Speed, 0f);
                    isOnTask = false;
                    unit.animator.SetBool("Harvest", true);
                }
            }
        }
    }

    public void UpdateGatheredResourcesAmount(List<ObjectPrices> newObjectPrices)
    {
        gatheredResources = newObjectPrices;
    }

    public void ResetGathererProperties()
    {
        unit.animator.SetBool("Harvest", false);
        isGoingToStockPile = false;

        unit.animator.SetBool("Building", false);

    }

    public void GoToNextResource()
    {
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        isOnTask = false;
        unit.animator.SetBool("Harvest", false);
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
    internal override void GoToPosition(Vector3 point)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        GoToPositionTask newTask = new(point);
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
    public void ReturnToStockPile()
    {
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        unit.animator.SetBool("Harvest", false);
        stockPile = AccessToClassByTeamColor.instance.GetClosestStockPileByTeamColor(unit.teamColor, unit.transform.position);
        if (stockPile != null)
        {
            unit.SetActiveEnemyDetector(false);
            attackCycleActivated = false;
            ReturnToStockpileTask newTask = new(stockPile.stockPilePosition.transform.position);
            requestedTasks.AddLast(newTask);
            DoTask();
            newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
        }
    }
    public override void GatherResourceTask(GatherableResource resource)
    {
        unit.SetActiveEnemyDetector(false);
        GathererResourceTask newTask = new(resource);
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }

    public override void BuildConstructionTask(GameObject construction)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        BuildConstructionTask newTask = new(construction);
        requestedTasks.AddLast(newTask);
        DoTask();
        newTask.TakeVisualizationTask(taskVisualization.AddNewTaskAndRefreshLineRenderer(requestedTasks));
    }
}

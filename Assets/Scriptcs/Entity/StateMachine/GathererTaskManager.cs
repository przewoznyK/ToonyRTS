
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GathererTaskManager : UnitTaskManager
{
    public List<ObjectPrices> gatheredResources { get; private set; } = new();
    public GatherableResource currentGatherableResource;
    public ResourceTypesEnum currentResourceTypeGathering;
    [SerializeField] private int maxCarried = 20;
    public int currentGathered;
    public int reachedPerSecond;
    private bool isHarvesting;
    private bool isGoingToStockPile;
    private bool isGoingToBuildingConstruction;
    Vector3 constructionToBuildPosition;
    public InConstructionBuildingRepresentation currentConstionBuildingTarget;
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
        if (isOnTask)
        {
            if (currentTask.unitTaskType == UnitTaskTypeEnum.GoToPosition)
            {
                unit.animator.SetBool("Harvest", false);
                if (Vector3.Distance(taskVector, transform.position) <= 2f)
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
                    }
                }
                else
                {
                    unit.agent.ResetPath();
                    unit.animator.SetFloat(Unit.Speed, 0f);
                    isOnTask = false;

                }
            }
            else if(currentTask.unitTaskType == UnitTaskTypeEnum.GatherResource)
            {
        
                if (Vector3.Distance(taskTransform.position, transform.position) <= unit.agent.stoppingDistance)
                {
                    unit.agent.ResetPath();
                    unit.animator.SetFloat(Unit.Speed, 0f);
                    unit.animator.SetBool("Harvest", true);
                    isHarvesting = true;
                    isOnTask = false;
                }
            }
        }

        if (rotateToTaskTransform)
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


        if(isGoingToStockPile)
        {
            if (Vector3.Distance(stockPile.stockPilePosition.position, transform.position) <= unit.agent.stoppingDistance)
            {        
                isGoingToStockPile = false;
                unit.animator.SetFloat(Unit.Speed, 0f);
                var returnObjectPrices = stockPile.AddResourcesToStockPile(gatheredResources);
                UpdateGatheredResourcesAmount(returnObjectPrices);

                currentGathered = 0;
                if (currentGatherableResource._available > 0)
                    GatherResource(currentGatherableResource);
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
                Debug.Log("BUDUJE");
            }
                
        }
    }
    public override void DoTask()
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
            else if(currentTask is GatherResourceTask gatherResource)
            {
                currentGatherableResource = gatherResource.gatherableResource;
                unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                taskTransform = gatherResource.targetTransform;
                currentResourceTypeGathering = gatherResource.currentResourceTypeGathering;
                unit.agent.SetDestination(taskTransform.position);
                unit.animator.SetFloat(Unit.Speed, 1f);
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
    public override void GatherResource(GatherableResource resource)
    {
        attackCycleActivated = false;
        GatherResourceTask newTask = new(resource);
        requestedTasks.AddLast(newTask);
        DoTask();
    }

    internal override void GoToPosition(Vector3 point)
    {
        ResetGathererProperties();
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        GoToPositionTask newTask = new(point);
        requestedTasks.AddLast(newTask);
        DoTask();
    }
    public override void BuildConstructionTask(GameObject construction)
    {
      //  currentConstionBuildingTarget = construction.transform.position;
        ResetGathererProperties();
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        BuildConstructionTask newTask = new(construction);
        requestedTasks.AddLast(newTask);
        DoTask();
    }
    public void UpdateGatheredResourcesAmount(List<ObjectPrices> newObjectPrices)
    {
        gatheredResources = newObjectPrices;
    }

    public void ResetGathererProperties()
    {
        
        unit.animator.SetBool("Harvest", false);
        currentGatherableResource = null;
        isHarvesting = false;
        isGoingToStockPile = false;

        unit.animator.SetBool("Building", false);

    }

    public void GoToNextResource()
    {
        ResetGathererProperties();
        GatherableResource nextResource = FindNearestResource();

        if (nextResource != null)
        {
            GatherResource(nextResource);
        }
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

    public void TurnOffHarvesting()
    {
        isHarvesting = false;
        unit.animator.SetBool("Harvest", false);
    }

    public bool CheckIfGathererHaveToReturnToStockPile()
    {
        return currentGathered >= maxCarried;
    }

    public void ReturnToStockPile()
    {
        isHarvesting = false;
        unit.animator.SetBool("Harvest", false);
        stockPile = AccessToClassByTeamColor.instance.GetClosestStockPileByTeamColor(unit.teamColor, unit.transform.position);
        if (stockPile != null)
        {
            unit.agent.SetDestination(stockPile.stockPilePosition.transform.position);
            unit.animator.SetFloat(Unit.Speed, 1f);
            isGoingToStockPile = true;
        }
    }

    
}

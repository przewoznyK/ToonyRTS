using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Drawing;
using System.Collections;
using System.Linq;
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
        StopGatheringIfActive();
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
                if (attackTarget.targetTransform.TryGetComponent<Building>(out Building building))
                {
                    float buildingRadius = Mathf.Max(building.buildingSize.x, building.buildingSize.y) * 0.5f;
                    float unitRadius = unit.agent.radius;
                    unit.agent.stoppingDistance = buildingRadius + unitRadius;
                }
                RespondFromServerToAttackEntity(attackTarget.targetTransform);
                RespondFromServerToUpdateAnimatorSpeedValue(1);
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
            else if (currentTask is GathererResourceTask gatherResource)
            {
                if (CanReachDestination(gatherResource.targetTransform.position) == false)
                {
                    GoToNextTask();
                    return;
                }
                currentGatherableResource = gatherResource.gatherableResource;
                unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                taskTransform = gatherResource.targetTransform;
                currentResourceTypeGathering = gatherResource.currentResourceTypeGathering;
                RespondFromServerToMoveUnit(taskTransform.position);
                RespondFromServerToUpdateAnimatorSpeedValue(1);
            }
            else if (currentTask is ReturnToStockpileTask returnToStockpile)
            {
                //Vector2 buildingSize = returnToStockpile.buildingSize;
                //float buildingRadius = Mathf.Max(buildingSize.x, buildingSize.y) * 0.5f;
                //float unitRadius = unit.agent.radius; // NavMeshAgent ma swój radius
                //unit.agent.stoppingDistance = buildingRadius + unitRadius - 1;
                Collider stockpileCollider = returnToStockpile.buildingCollider;
                Vector3 closestPoint = stockpileCollider.ClosestPoint(unit.transform.position);

                // Dodaj losowe przesuniêcie w obrêbie "strefy odstawiania"
                float offset = 1.0f; // np. 1 jednostka od œciany
                Vector3 randomOffset = new Vector3(
                    Random.Range(-offset, offset),
                    0,
                    Random.Range(-offset, offset)
                );


                unit.agent.stoppingDistance = 0.5f; // minimalnie przed œcian¹
   
                //     unit.agent.stoppingDistance = unit.defaultStoppingDistance;
                taskTransform = null;
                Vector3 pos = closestPoint + randomOffset;

                taskVector = pos;

                RespondFromServerToMoveUnit(pos);
                RespondFromServerToUpdateAnimatorSpeedValue(1);
                isGoingToStockPile = true;
            }
            else if (currentTask is BuildConstructionTask construction)
            {
                currentConstionBuildingTarget = construction.constructionBuildingRepresentation;
                construction.constructionBuildingRepresentation.gatherersBuildingThisConstruction.Add((GathererNew)this.unit);
                
                Vector2 buildingSize = construction.constructionBuildingRepresentation.buildingSize;
                float buildingRadius = Mathf.Max(buildingSize.x, buildingSize.y) * 0.5f;
                float unitRadius = unit.agent.radius;
                unit.agent.stoppingDistance = buildingRadius + unitRadius;
               
                Debug.Log("BuildConstructionTask Change Position");
                RespondFromServerToMoveUnit(construction.constructionPosition);
                RespondFromServerToUpdateAnimatorSpeedValue(1);
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
                if (Vector3.Distance(taskVector, transform.position) <= unit.agent.stoppingDistance)
                    GoToNextTask();
            }
            else if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget && taskTransform != null)
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
                        RespondFromServerToUpdateAnimatorSpeedValue(0);
                        isOnTask = false;
                    }
                }
            }
            else if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget && taskTransform == null)
            {
                GoToNextTask();
                AttackNearestEnemyByTeamColor();
            }
            else if (currentTask.unitTaskType == UnitTaskTypeEnum.GatherResource)
            {
                if (!unit.agent.pathPending && unit.agent.remainingDistance <= unit.agent.stoppingDistance)
                {
                    if (currentTask.targetTransform != null)
                    {
                        unit.agent.ResetPath();
                        RespondFromServerToUpdateAnimatorSpeedValue(0);
                        isOnTask = false;
                        StartGatheringIfNotActive();
                     //   RespondFromServerToSetBoolAnimation("Harvest", true);

                    }
                    else
                    {
                        if (CheckIfGathererHaveToReturnToStockPile())
                            ReturnToStockPile();
                        else
                            GoToNextResource();
                    }
                }
            }

            if (isGoingToStockPile)
            {
                if (!unit.agent.pathPending && unit.agent.remainingDistance <= unit.agent.stoppingDistance)
                {
                    isGoingToStockPile = false;
                    RespondFromServerToUpdateAnimatorSpeedValue(0);
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
                if (!unit.agent.pathPending && unit.agent.remainingDistance <= unit.agent.stoppingDistance)
                {
                    unit.agent.ResetPath();
                    RespondFromServerToUpdateAnimatorSpeedValue(0);
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

        RespondFromServerToUpdateAnimatorSpeedValue(0);
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        isOnTask = false;
        if (requestedTasks.Count == 0)
        {
            GatherableResource nextResource = FindNearestResource();
            if (nextResource != null)
            {
                GatherResourceTask(nextResource); 
            }
               
        }
        else DoTask();
    }

    public GatherableResource FindNearestResource()
    {
        GatherableResource[] allResources = FindObjectsOfType<GatherableResource>();

        GatherableResource closest = null;
        float minDistance = Mathf.Infinity;

        Vector3 currentPosition;
        if (latestResourcePosition != Vector3.zero)
            currentPosition = latestResourcePosition;
        else currentPosition = transform.position;

        foreach (GatherableResource resource in allResources)
        {
            // sprawdŸ najpierw typ
            if (resource.resourceType != currentResourceTypeGathering)
                continue;

            // sprawdŸ czy mo¿na tam dojœæ
            if (!CanReachDestination(resource.transform.position))
                continue;

            // policz dystans
            float distance = Vector3.Distance(currentPosition, resource.transform.position);

            if (distance < minDistance)
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
        Debug.Log("RETURN TO STOCK PILE");
        requestedTasks.First.Value.EndTask();
        requestedTasks.RemoveFirst();
        RespondFromServerToSetBoolAnimation("Harvest", false);

        stockPile = PlayerController.LocalPlayer.stockPileManager.GetClosestStockPile(unit.teamColor, transform.position);
        if (stockPile != null)
        {
            unit.SetActiveEnemyDetector(false);
            attackCycleActivated = false;
            ReturnToStockpileTask newTask = new(stockPile.stockPilePosition.transform.position, stockPile.stockPileSize, stockPile.stockPileCollider);
            requestedTasks.AddLast(newTask);
            DoTask();
        }
    }
    public override void GatherResourceTask(GatherableResource resource)
    {
        unit.SetActiveEnemyDetector(false);
        GathererResourceTask newTask = new(resource);
        requestedTasks.AddLast(newTask);
        DoTask();
    }

    public override void RespondFromServerToBuildConstructionTask(GameObject construction)
    {
        unit.SetActiveEnemyDetector(false);
        attackCycleActivated = false;
        BuildConstructionTask newTask = new(construction);
        requestedTasks.AddLast(newTask);
        DoTask();
    }
    private Coroutine gatheringCoroutine;
    private Vector3 latestResourcePosition;
    private IEnumerator GatheringCycle()
    {
        while (currentGatherableResource != null)
        {
            unit.animator.SetTrigger("HarvestTrigger");

            yield return new WaitForSeconds(0.8f);
            latestResourcePosition = currentGatherableResource.transform.position;
            // Próba zebrania
            currentGatherableResource.Take(this);
            var obj = gatheredResources.FirstOrDefault(resource => resource.priceType == currentResourceTypeGathering);

            obj.AddValue(1);
            currentGathered = obj.priceValue;
            if(currentGathered >= maxCarried)
            {
                if (CheckIfGathererHaveToReturnToStockPile())
                {
                    StopGatheringIfActive();
                    ReturnToStockPile();
                }
                yield break;
            }

            yield return new WaitForSeconds(1f);

            // Poczekaj 2 sekundy (czas animacji)

            // SprawdŸ czy nadal jest co zbieraæ
            if (currentGatherableResource == null)
            {
                GoToNextResource();
                StopGatheringIfActive();
                yield break; // zakoñcz korutynê od razu
            }

        }
        StopGatheringIfActive();
    }

    private void StartGatheringIfNotActive()
    {
        if (gatheringCoroutine == null)
        {
            rotateToTaskTransform = true;
            gatheringCoroutine = StartCoroutine(GatheringCycle());
        }
    }

    private void StopGatheringIfActive()
    {
        if (gatheringCoroutine != null)
        {
            rotateToTaskTransform = false;
            StopCoroutine(gatheringCoroutine);
            gatheringCoroutine = null;
        }
    }
}

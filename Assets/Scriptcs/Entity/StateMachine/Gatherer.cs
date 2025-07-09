using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Gatherer : Unit
{
    public event Action<int> OnGatheredChanged;
    public ResourceTypesEnum currentResourceTypeGathering;
    [SerializeField] private int _maxCarried = 20;

    StateMachine _stateMachine;
    public int _gatheredResources;

    public GatherableResource TargetResource { get; set; }
    public InConstructionBuildingRepresentation TargetConstructionToBuild { get; set; }

    public IStockPile StockPile { get; set; }
    IState sleep;
    IState moveToSelectedPosition;
    IState moveToSelectedResource;
    private bool gatheringEnabled;
    private bool buildingContructionEnabled;

    private static readonly int Harvest = Animator.StringToHash("Harvest");

    [SerializeField] private float reachedResourceDistance;
    [SerializeField] private float reachedStockpileDistance;
    public int reachedPerSecond;
    public List<ObjectPrices> objectPrices { get; private set; } = new ();

    private void Awake()
    {
        var navMeshAgent = GetComponent<NavMeshAgent>();
      

        sleep = new Sleep();
        var search = new SearchForResource(this);
        moveToSelectedPosition = new MoveToSelectedPosition(this, navMeshAgent, animator);
        moveToSelectedResource = new MoveToSelectedResource(this, navMeshAgent, animator);
        var harvest = new HarvestResource(this, animator, agent);
        var returnToStockpile = new ReturnToStockpile(this, navMeshAgent, animator);
        var placeResourcesInStockpile = new PlaceResourcesInStockpile(this);
    
        _stateMachine = new StateMachine();
        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);


        Func<bool> NoHasTargetPosition() => () => isGoingToPosition == false;
        Func<bool> HasTargetResource() => () => TargetResource != null && NoHasTargetPosition().Invoke();
        Func<bool> NoHasTargetResource() => () => TargetResource == null;
        Func<bool> StuckForOverASecondAndGoingToResource() => () => TimeStuck > 1f && TargetResource == null;
        Func<bool> ReachedResource() => () => TargetResource != null &&
                                              Vector3.Distance(transform.position, TargetResource.transform.position) < reachedResourceDistance;

        Func<bool> TargetIsDepletedAndICanCarryMore() => () => (TargetResource == null || TargetResource.IsDepleted) && !InventoryFull().Invoke();
        Func<bool> InventoryFull() => () => _gatheredResources >= _maxCarried;
        Func<bool> ReachedStockpile() => () => StockPile != null &&
                                               Vector3.Distance(transform.position, StockPile.stockPilePosition.transform.position) < reachedStockpileDistance;
        Func<bool> BackFromStockpileToSelectedResource() => () => _gatheredResources == 0 && HasTargetResource().Invoke();
        Func<bool> BackFromStockpileToSearchTarget() => () => _gatheredResources == 0 && !HasTargetResource().Invoke();

        At(search, moveToSelectedResource, HasTargetResource());
        At(moveToSelectedResource, search, StuckForOverASecondAndGoingToResource());
        At(moveToSelectedResource, harvest, ReachedResource());
        At(harvest, search, TargetIsDepletedAndICanCarryMore());
        At(harvest, returnToStockpile, InventoryFull());
        At(returnToStockpile, placeResourcesInStockpile, ReachedStockpile());
        At(placeResourcesInStockpile, moveToSelectedResource, BackFromStockpileToSelectedResource());
        At(placeResourcesInStockpile, search, BackFromStockpileToSearchTarget());
        At(search, sleep, NoHasTargetResource());

        objectPrices = new List<ObjectPrices>();

        var allowedTypes = new List<ResourceTypesEnum>
        {
            ResourceTypesEnum.food,
            ResourceTypesEnum.wood,
            ResourceTypesEnum.gold,
            ResourceTypesEnum.stone
        };

        foreach (ResourceTypesEnum type in allowedTypes)
        {
            objectPrices.Add(new ObjectPrices(type, 0));
        }
    }

    private void Update()
    {
        if (gatheringEnabled)
            _stateMachine.Tick();

        if (isGoingToPosition)
        {
            if(Vector3.Distance(TargetPosition, transform.position) <= 1f)
            {
                agent.isStopped = false;

                isGoingToPosition = false; 
                animator.SetFloat(Speed, 0f);
                TargetPosition = Vector3.zero;
            }
        }
        if(buildingContructionEnabled)
        {
            if (Vector3.Distance(TargetConstructionToBuild.transform.position, transform.position) <= 3.5f)
            {
                agent.isStopped = false;

                buildingContructionEnabled = false;
                animator.SetFloat(Speed, 0f);
                StartCoroutine(BuildingCycle());
            }
        }
    }
   

    public void TakeFromTarget()
    {
        if (TargetResource.Take())
        {
            _gatheredResources++;
            OnGatheredChanged?.Invoke(_gatheredResources);
        }
     
    }

    public bool Take()
    {
        if (_gatheredResources <= 0)
            return false;

        _gatheredResources--;
        OnGatheredChanged?.Invoke(_gatheredResources);
        return true;
    }

    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            // CHECK TEAM COLOR
            if ((component.GetTeam() & teamColor) == teamColor || (component.GetTeam() == TeamColorEnum.Neutral))
            {
                // GO TO GATHERING RESOURCE
                if (component.GetEntityType() == EntityTypeEnum.resource)
                {
                    gatheringEnabled = true;
                    isGoingToPosition = false;
                    GatherableResource properties = component.GetProperties<GatherableResource>();
                    currentResourceTypeGathering = properties.resourceType;
                    TargetResource = properties;
                    _stateMachine.SetState(moveToSelectedResource);
                    animator.SetFloat(Speed, 1f);
                    GoMeetingPosition(hit.point);
                }
                // GO TO BUILDING CONTRUCTION
                else if (component.GetEntityType() == EntityTypeEnum.contructionToBuild)
                {
                    Debug.Log("CONSTRUCTION");
                    gatheringEnabled = false;
                    buildingContructionEnabled = true;
                    isGoingToPosition = false;
                    InConstructionBuildingRepresentation properties = component.GetProperties<InConstructionBuildingRepresentation>();
                    TargetConstructionToBuild = properties;
                    GoMeetingPosition(hit.transform.position);
             
                }
            }
        }
        if (hit.collider.CompareTag("Ground"))
        {
            gatheringEnabled = false;
            TargetPosition = hit.point;
            isGoingToPosition = true;
            GoMeetingPosition(TargetPosition);
            animator.SetFloat(Speed, 1f);
        }
    }

    public void AddResource(int value)
    {
        var obj = objectPrices.FirstOrDefault(p => p.priceType == currentResourceTypeGathering);
        if (obj != null)
        {
            obj.AddValue(value);
            _gatheredResources = obj.priceValue;
        }
    }

    public void SetNewObjectPricesList(List<ObjectPrices> newObjectPrices)
    {
        objectPrices = newObjectPrices;
    }
    IEnumerator BuildingCycle()
    {
        animator.SetTrigger(Harvest);
        yield return new WaitForSeconds(1f);
        if (!TargetConstructionToBuild)
            yield break;
        TargetConstructionToBuild.WorkOnBuilding(1);
        StartCoroutine(BuildingCycle());
    }

    public void SetBuildingToBuild(GameObject constructionInstantiate)
    {
        gatheringEnabled = false;
        buildingContructionEnabled = true;
        isGoingToPosition = false;
        TargetConstructionToBuild = constructionInstantiate.GetComponent<InConstructionBuildingRepresentation>();
        GoMeetingPosition(constructionInstantiate.transform.position);
    }

    public void SetSleepState()
    {
        _stateMachine.SetState(sleep);
    }

}

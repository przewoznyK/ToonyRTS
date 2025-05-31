using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Gatherer : Unit
{
    public event Action<int> OnGatheredChanged;
    public ResourceTypesEnum currentResourceTypeGathering;
    [SerializeField] private int _maxCarried = 20;

    private StateMachine _stateMachine;
    public int _gatheredResources;

    public GatherableResource TargetResource { get; set; }
    public StockPile StockPile { get; set; }
    IState sleep;
    IState moveToSelectedPosition;
    IState moveToSelectedResource;
    private bool gatheringEnabled;
    private static readonly int Speed = Animator.StringToHash("Speed");

    public List<ObjectPrices> objectPrices { get; private set; } = new ();

    private void Awake()
    {
        var navMeshAgent = GetComponent<NavMeshAgent>();
      

        sleep = new Sleep(this);
        var search = new SearchForResource(this);
        moveToSelectedPosition = new MoveToSelectedPosition(this, navMeshAgent, animator);
        moveToSelectedResource = new MoveToSelectedResource(this, navMeshAgent, animator);
        var harvest = new HarvestResource(this, animator);
        var returnToStockpile = new ReturnToStockpile(this, navMeshAgent, animator);
        var placeResourcesInStockpile = new PlaceResourcesInStockpile(this);
    
        _stateMachine = new StateMachine();
        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);


        Func<bool> NoHasTargetPosition() => () => isGoingToPosition == false;
        Func<bool> HasTargetResource() => () => TargetResource != null && NoHasTargetPosition().Invoke();
        Func<bool> NoHasTargetResource() => () => TargetResource == null;
        Func<bool> StuckForOverASecondAndGoingToResource() => () => TimeStuck > 1f && TargetResource == null;
        Func<bool> ReachedResource() => () => TargetResource != null &&
                                              Vector3.Distance(transform.position, TargetResource.transform.position) < 1.5f;

        Func<bool> TargetIsDepletedAndICanCarryMore() => () => (TargetResource == null || TargetResource.IsDepleted) && !InventoryFull().Invoke();
        Func<bool> InventoryFull() => () => _gatheredResources >= _maxCarried;
        Func<bool> ReachedStockpile() => () => StockPile != null &&
                                               Vector3.Distance(transform.position, StockPile.transform.position) < 1f;
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
        if(gatheringEnabled)
            _stateMachine.Tick();

        if (isGoingToPosition)
        {
            if(Vector3.Distance(TargetPosition, transform.position) <= 1f)
            {
                isGoingToPosition = false; 
                animator.SetFloat(Speed, 0f);
                TargetPosition = Vector3.zero;
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

    public override void PlayerRightMouseButtonCommand(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Resource"))
        {
            gatheringEnabled = true;
            GatherableResource resource = hit.collider.GetComponent<GatherableResource>();

            currentResourceTypeGathering = resource.resourceType;
            TargetResource = resource;
            _stateMachine.SetState(moveToSelectedResource);
            GoMeetingPosition(hit.point);
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
        
            Debug.Log($"Aktualna iloœæ {currentResourceTypeGathering}: {obj.priceValue}");
            _gatheredResources = obj.priceValue;
        }
    }

    public void SetNewObjectPricesList(List<ObjectPrices> newObjectPrices)
    {
        
    }
}

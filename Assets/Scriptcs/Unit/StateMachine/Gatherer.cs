using System;
using UnityEngine;
using UnityEngine.AI;

public class Gatherer : Unit
{
    public event Action<int> OnGatheredChanged;
    public ResourceTypesEnum currentResourceTypeGathering;
    [SerializeField] private int _maxCarried = 20;

    private StateMachine _stateMachine;
    private int _gatheredResources;

    public GatherableResource TargetResource { get; set; }
    public StockPile StockPile { get; set; }
    IState sleep;
    IState moveToSelectedPosition;


    private void Awake()
    {
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var animator = GetComponent<Animator>();

        sleep = new Sleep(this);
        var search = new SearchForResource(this);
        moveToSelectedPosition = new MoveToSelectedPosition(this, navMeshAgent, animator);
        var moveToSelectedResources = new MoveToSelectedResource(this, navMeshAgent, animator);
        var harvest = new HarvestResource(this, animator);
        var returnToStockpile = new ReturnToStockpile(this, navMeshAgent, animator);
        var placeResourcesInStockpile = new PlaceResourcesInStockpile(this);
    
        _stateMachine = new StateMachine();
        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);


        Func<bool> HasTargetPosition() => () => isGoingToPosition == true;
        Func<bool> NoHasTargetPosition() => () => isGoingToPosition == false;
        Func<bool> HasTargetResource() => () => TargetResource != null && NoHasTargetPosition().Invoke();
        Func<bool> NoHasTargetResource() => () => TargetResource == null;
        Func<bool> StuckForOverASecondAndGoingToPosition() => () => TimeStuck > 1f && isGoingToPosition == true;
        Func<bool> StuckForOverASecondAndGoingToResource() => () => TimeStuck > 1f && TargetResource == null;
        Func<bool> ReachedResource() => () => TargetResource != null &&
                                              Vector3.Distance(transform.position, TargetResource.transform.position) < 1.5f;

        Func<bool> TargetIsDepletedAndICanCarryMore() => () => (TargetResource == null || TargetResource.IsDepleted) && !InventoryFull().Invoke();
        Func<bool> InventoryFull() => () => _gatheredResources >= _maxCarried;
        Func<bool> ReachedStockpile() => () => StockPile != null &&
                                               Vector3.Distance(transform.position, StockPile.transform.position) < 1f;
        Func<bool> BackFromStockpileToSelectedResource() => () => _gatheredResources == 0 && HasTargetResource().Invoke();
        Func<bool> BackFromStockpileToSearchTarget() => () => _gatheredResources == 0 && !HasTargetResource().Invoke();

        At(sleep, moveToSelectedPosition, HasTargetPosition());
        At(sleep, moveToSelectedResources, HasTargetResource());
        At(search, moveToSelectedResources, HasTargetResource());
        At(moveToSelectedResources, search, StuckForOverASecondAndGoingToResource());
        At(moveToSelectedPosition, sleep, StuckForOverASecondAndGoingToPosition());
        At(moveToSelectedResources, harvest, ReachedResource());
        At(harvest, search, TargetIsDepletedAndICanCarryMore());
        At(harvest, returnToStockpile, InventoryFull());
        At(returnToStockpile, placeResourcesInStockpile, ReachedStockpile());
        At(placeResourcesInStockpile, moveToSelectedResources, BackFromStockpileToSelectedResource());
        At(placeResourcesInStockpile, search, BackFromStockpileToSearchTarget());
        At(search, sleep, NoHasTargetResource());
        _stateMachine.SetState(sleep);
    }

    private void Update() => _stateMachine.Tick();

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
            GatherableResource resource = hit.collider.GetComponent<GatherableResource>();
            currentResourceTypeGathering = resource.resourceType;
            TargetResource = resource;
        }
        if (hit.collider.CompareTag("Ground"))
        {
            TargetPosition = hit.point;
            isGoingToPosition = true;
            _stateMachine.SetState(moveToSelectedPosition);
        }

    }
}

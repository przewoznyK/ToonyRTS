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

    public GatherableResource Target { get; set; }
    public StockPile StockPile { get; set; }

    private void Awake()
    {
        var navMeshAgent = GetComponent<NavMeshAgent>();
        var animator = GetComponent<Animator>();

        var sleep = new Sleep(this);
        var search = new SearchForResource(this);
        var moveToSelected = new MoveToSelectedResource(this, navMeshAgent, animator);
        var harvest = new HarvestResource(this, animator);
        var returnToStockpile = new ReturnToStockpile(this, navMeshAgent, animator);
        var placeResourcesInStockpile = new PlaceResourcesInStockpile(this);

        _stateMachine = new StateMachine();
        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
        
        Func<bool> HasTarget() => () => Target != null;
        Func<bool> StuckForOverASecond() => () => moveToSelected.TimeStuck > 1f;
        Func<bool> ReachedResource() => () => Target != null &&
                                              Vector3.Distance(transform.position, Target.transform.position) < 1.5f;

        Func<bool> TargetIsDepletedAndICanCarryMore() => () => (Target == null || Target.IsDepleted) && !InventoryFull().Invoke();
        Func<bool> InventoryFull() => () => _gatheredResources >= _maxCarried;
        Func<bool> ReachedStockpile() => () => StockPile != null &&
                                               Vector3.Distance(transform.position, StockPile.transform.position) < 1f;

        At(sleep, moveToSelected, HasTarget());
        At(search, moveToSelected, HasTarget());
        At(moveToSelected, search, StuckForOverASecond());
        At(moveToSelected, harvest, ReachedResource());
        At(harvest, search, TargetIsDepletedAndICanCarryMore());
        At(harvest, returnToStockpile, InventoryFull());
        At(returnToStockpile, placeResourcesInStockpile, ReachedStockpile());
        At(placeResourcesInStockpile, search, () => _gatheredResources == 0);

        _stateMachine.SetState(sleep);
    }

    private void Update() => _stateMachine.Tick();

    public void TakeFromTarget()
    {
        if (Target.Take())
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
        Debug.Log("ODPALAM");
        if (hit.collider.CompareTag("Resource"))
        {
            GatherableResource resource = hit.collider.GetComponent<GatherableResource>();
            currentResourceTypeGathering = resource.resourceType;
            Target = resource;
        }

    }
}

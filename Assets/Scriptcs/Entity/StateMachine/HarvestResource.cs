using UnityEngine;
using UnityEngine.AI;

internal class HarvestResource : IState
{
    private readonly Gatherer _gatherer;
    private readonly Animator _animator;
    private readonly NavMeshAgent agent;
    private float _resourcesPerSecond = 3;

    private float _nextTakeResourceTime;
    private static readonly int Harvest = Animator.StringToHash("Harvest");

    public HarvestResource(Gatherer gatherer, Animator animator, NavMeshAgent agent)
    {
        _gatherer = gatherer;
        _animator = animator;
        this.agent = agent;
    }

    public void Tick()
    {
        if (_gatherer.TargetResource != null)
        {
            if (_nextTakeResourceTime <= Time.time)
            {
                _nextTakeResourceTime = Time.time + (1f / _resourcesPerSecond);
                _gatherer.AddResource(_gatherer.reachedPerSecond);
                _animator.SetTrigger(Harvest);
            }
        }
    }

    public void OnEnter()
    {
        agent.isStopped = true;
    }

    public void OnExit()
    {
        agent.isStopped = false;
    }
}
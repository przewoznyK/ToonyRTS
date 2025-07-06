using UnityEngine;
using UnityEngine.AI;

internal class MoveToSelectedResource : IState
{
    private readonly Gatherer _gatherer;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private Vector3 _lastPosition = Vector3.zero;



    public MoveToSelectedResource(Gatherer gatherer, NavMeshAgent navMeshAgent, Animator animator)
    {
        _gatherer = gatherer;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }

    public void Tick()
    {
        if (Vector3.Distance(_gatherer.transform.position, _lastPosition) <= 0f)
            _gatherer.TimeStuck += Time.deltaTime;

        _lastPosition = _gatherer.transform.position;
    }

    public void OnEnter()
    {
        _gatherer.TimeStuck = 0f;
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(_gatherer.TargetResource.transform.position);
        _animator.SetFloat(Speed, 1f);
    }

    public void OnExit()
    {
        _animator.SetFloat(Speed, 0f);
    }
}
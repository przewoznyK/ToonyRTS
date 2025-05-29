using UnityEngine;
using UnityEngine.AI;

public class MoveToSelectedPosition : IState
{
    private readonly Gatherer _gatherer;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private Vector3 _lastPosition = Vector3.zero;
    public Vector3 selectedPosition;
    public float TimeStuck;

    public MoveToSelectedPosition(Gatherer gatherer, NavMeshAgent navMeshAgent, Animator animator)
    {
        _gatherer = gatherer;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }

    public void Tick()
    {
        if (Vector3.Distance(_gatherer.transform.position, _lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPosition = _gatherer.transform.position;
    }

    public void OnEnter()
    {
        Debug.Log("IDE DO POZYCJI");
        _gatherer.isGoingToPosition = true;
        TimeStuck = 0f;
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(_gatherer.TargetPosition);
        _animator.SetFloat(Speed, 1f);
    }

    public void OnExit()
    {
        Debug.Log("Koncze DO POZYCJI");
        _gatherer.isGoingToPosition = false;

        _navMeshAgent.enabled = false;
        _animator.SetFloat(Speed, 0f);
        _gatherer.TargetPosition = Vector3.zero;
    }
}

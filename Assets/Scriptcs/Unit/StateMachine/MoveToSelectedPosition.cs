using UnityEngine;
using UnityEngine.AI;

public class MoveToSelectedPosition : IState
{
    private readonly Unit unit;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private Vector3 _lastPosition = Vector3.zero;
    public Vector3 selectedPosition;
    public float TimeStuck;

    public MoveToSelectedPosition(Unit unit, NavMeshAgent navMeshAgent, Animator animator)
    {
        this.unit = unit;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }

    public void Tick()
    {
        if (Vector3.Distance(unit.transform.position, _lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPosition = unit.transform.position;
    }

    public void OnEnter()
    {
        Debug.Log("IDE DO POZYCJI");
        unit.isGoingToPosition = true;
        TimeStuck = 0f;
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(unit.TargetPosition);
        _animator.SetFloat(Speed, 1f);
    }

    public void OnExit()
    {
        Debug.Log("Koncze DO POZYCJI");
        unit.isGoingToPosition = false;

        _navMeshAgent.enabled = false;
        _animator.SetFloat(Speed, 0f);
        unit.TargetPosition = Vector3.zero;
    }
}

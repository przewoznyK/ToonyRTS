using System.Collections;
using UnityEngine;
using UnityEngine.AI;

internal class ReturnToStockpile : IState
{
    private readonly Gatherer _gatherer;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;

    private static readonly int Speed = Animator.StringToHash("Speed");

    public ReturnToStockpile(Gatherer gatherer, NavMeshAgent navMeshAgent, Animator animator)
    {
        _gatherer = gatherer;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }

    public void Tick()
    {
    }

    public void OnEnter()
    {
        IStockPile stockPile = AccessToClassByTeamColor.instance.GetClosestStockPileByTeamColor(_gatherer.teamColor, _gatherer.transform.position);
        //Debug.Log(stockPile);
        if (stockPile != null)
        {
            _gatherer.StockPile = stockPile;
            _navMeshAgent.SetDestination(stockPile.stockPilePosition.transform.position);
            _animator.SetFloat(Speed, 1f);

        }
        else
            _gatherer.SetSleepState();

    }

    public void OnExit()
    {
        _animator.SetFloat(Speed, 0f);
    }
}
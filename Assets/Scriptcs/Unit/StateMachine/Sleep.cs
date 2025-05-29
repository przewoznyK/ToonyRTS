using System.Linq;
using UnityEngine;

public class Sleep : IState
{
    private readonly Gatherer _gatherer;

    public Sleep(Gatherer gatherer)
    {
        _gatherer = gatherer;
    }
    public void Tick()
    {
    }

    public void OnEnter() { }
    public void OnExit() { }
}
using UnityEngine;

public class UnitAutoPatrolling : MonoBehaviour
{
    Unit unit;
    public Vector3 destinatedPosition;
    private void Start()
    {
        unit = GetComponent<Unit>();
        unit.agent.SetDestination(destinatedPosition);
    }


}

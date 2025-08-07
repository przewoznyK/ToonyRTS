using UnityEngine;

public class RemoveEntity : MonoBehaviour
{
    public static RemoveEntity Instance;
    GridData gridData;

    internal void Init(GridData gridData)
    {
        Instance = this;
        this.gridData = gridData;
    }

    public void RemoveEntityFromGame(Building building)
    {
        if(building.isStockPile)
        {
            IStockPile stockPile = building.GetComponent<IStockPile>();
            AccessToClassByTeamColor.Instance.RemoveStockPileFromGlobalList(building.teamColor, stockPile);
        }
        gridData.RemoveObjectAt(building.positionToOccupy);
    }

    public void RemoveEntityFromGame(Unit unit)
    {
        AccessToClassByTeamColor.Instance.GetControlledUnitsByTeamColor(unit.teamColor).RemoveUnit(unit);
        Destroy(unit.gameObject);
    }
}

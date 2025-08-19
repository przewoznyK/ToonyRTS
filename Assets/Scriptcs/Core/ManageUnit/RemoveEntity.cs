using UnityEngine;

public class RemoveEntity : MonoBehaviour
{
    public static RemoveEntity Instance;
    GridDataNetwork gridData;

    internal void Init(GridDataNetwork gridData)
    {
        Instance = this;
        this.gridData = gridData;
    }

    public void RemoveEntityFromGame(Building building)
    {
        if(building.isStockPile)
        {
            IStockPile stockPile = building.GetComponent<IStockPile>();
            PlayerController.LocalPlayer.stockPileManager.stockPiles.Remove(stockPile);
        }
        gridData.RemoveObjectAt(building.positionToOccupy);
    }

    public void RemoveEntityFromGame(Unit unit)
    {
        PlayerController.LocalPlayer.controlledUnits.RemoveUnit(unit);
        Destroy(unit.gameObject);
    }
}

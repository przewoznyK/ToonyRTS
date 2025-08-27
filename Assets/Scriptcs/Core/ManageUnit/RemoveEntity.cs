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
        Debug.Log(building.positionToOccupy.Count + " < --- ");
        gridData.RequestToServerToRemoveObjectFromGridData(building.positionToOccupy);
        PlayerController.LocalPlayer.CmdRemoveGameObject(building.gameObject);
    }

    public void RemoveEntityFromGame(Unit unit)
    {
        PlayerController.LocalPlayer.controlledUnits.RemoveUnit(unit);
        PlayerController.LocalPlayer.CmdRemoveGameObject(unit.gameObject);
    }
}

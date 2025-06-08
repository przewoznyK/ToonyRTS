using System;
using UnityEngine;

public class PlayerRemoveEntity : MonoBehaviour
{
    GridData gridData;
    TeamColorEnum teamColor;

    internal void Init(TeamColorEnum teamColor, GridData gridData)
    {
        this.gridData = gridData;
        this.teamColor = teamColor;
    }

    public void RemoveEntity(Building building)
    {
        if(building.isStockPile)
        {
            IStockPile stockPile = building.GetComponent<IStockPile>();
            AccessToClassByTeamColor.instance.RemoveStockPileFromGlobalList(teamColor, stockPile);
        }
    }

    public void RemoveEntity(Unit unit)
    {
        AccessToClassByTeamColor.instance.GetControlledUnitsByTeamColor(teamColor).RemoveUnit(unit);
        Destroy(unit.gameObject);
    }


}

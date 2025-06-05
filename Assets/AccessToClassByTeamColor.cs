using System.Collections.Generic;
using UnityEngine;

public class AccessToClassByTeamColor : MonoBehaviour
{
    public static AccessToClassByTeamColor instance;
    Dictionary<TeamColorEnum, PlayerResources> PlayersResourcesManagerGlobalList = new();
    Dictionary<TeamColorEnum, ControlledUnits> ControlledUnitsManagerGlobalList = new();
    Dictionary<TeamColorEnum, List<IStockPile>> StockPileGlobalList = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Player Resource Manager List
    public void AddPlayerResourceManagerToGlobalList(TeamColorEnum teamColorEnum,PlayerResources playerResources)
    {
        PlayersResourcesManagerGlobalList.Add(teamColorEnum, playerResources);
    }

    public PlayerResources GetPlayerResourcesManagerByTeamColor(TeamColorEnum teamColorEnum)
    {
        return PlayersResourcesManagerGlobalList[teamColorEnum];
    }

    // Controlled Units List
    public void AddControlledUnitsManagerToGlobalList(TeamColorEnum teamColorEnum, ControlledUnits controlledUnits)
    {
        ControlledUnitsManagerGlobalList.Add(teamColorEnum, controlledUnits);
    }

    public ControlledUnits GetControlledUnitsByTeamColor(TeamColorEnum teamColorEnum)
    {
        return ControlledUnitsManagerGlobalList[teamColorEnum];
    }

    // StockPile List
    public void AddStockPileToGlobalList(TeamColorEnum teamColorEnum, IStockPile stockPile)
    {
        if (!StockPileGlobalList.ContainsKey(teamColorEnum))
        {
            StockPileGlobalList[teamColorEnum] = new List<IStockPile>();
        }

        StockPileGlobalList[teamColorEnum].Add(stockPile);
    }

    public IStockPile GetClosestStockPileByTeamColor(TeamColorEnum teamColorEnum, Vector3 fromPosition)
    {
        if (!StockPileGlobalList.TryGetValue(teamColorEnum, out var stockPiles) || stockPiles.Count == 0)
            return null;

        IStockPile closest = null;
        float minDistance = float.MaxValue;

        foreach (var stockPile in stockPiles)
        {
            float distance = Vector3.Distance(fromPosition, stockPile.stockPilePosition.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = stockPile;
            }
        }
        return closest;
    }

}

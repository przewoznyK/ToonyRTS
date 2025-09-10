using System.Collections.Generic;
using UnityEngine;

public class PlayerStockPileList
{
    private readonly Dictionary<TeamColorEnum, List<IStockPile>> stockPilesByTeam = new();


    public void AddStockPileByTeam(TeamColorEnum teamColor, IStockPile stockPile)
    {
        if (!stockPilesByTeam.ContainsKey(teamColor))
            stockPilesByTeam[teamColor] = new List<IStockPile>();

        Debug.Log("DODAJE STOCKPILE " + teamColor);
        stockPilesByTeam[teamColor].Add(stockPile);
    }

    public void RemoveStockPileByTeam(TeamColorEnum teamColor, IStockPile stockPile)
    {
        if (stockPile == null) return;

        if (stockPilesByTeam.TryGetValue(teamColor, out var list))
            list.Remove(stockPile);
    }

    public IStockPile GetClosestStockPile(TeamColorEnum teamColor, Vector3 fromPosition)
    {
        var stockPiles = stockPilesByTeam[teamColor];
        if (stockPiles.Count == 0) return null;

        IStockPile closest = null;
        float minDistance = float.MaxValue;

        foreach (var stockPile in stockPiles)
        {
            if (stockPile == null) continue;

            float distance = Vector3.Distance(fromPosition, stockPile.stockPilePosition.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = stockPile;
            }
        }
        return closest;
    }

}

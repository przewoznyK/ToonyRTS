using System.Collections.Generic;
using UnityEngine;

public class PlayerStockPileList
{
    public List<IStockPile> stockPiles = new();

    public IStockPile GetClosestStockPile(Vector3 fromPosition)
    {
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

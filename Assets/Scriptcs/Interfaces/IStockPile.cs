using System.Collections.Generic;
using UnityEngine;
public interface IStockPile
{
    Transform stockPilePosition { get; }
    public List<ObjectPrices> AddResourcesToStockPile(List<ObjectPrices> objectPrices);
}
